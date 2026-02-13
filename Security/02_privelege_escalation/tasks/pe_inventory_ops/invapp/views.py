from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth import authenticate, login, logout
from django.contrib.auth.models import User
from django.contrib import messages
from django.contrib.auth.decorators import login_required
from django.http import HttpResponseForbidden
from django.views.decorators.http import require_POST, require_GET
from django.views.decorators.csrf import csrf_protect
from .forms import LoginForm
from .models import Profile, Item

# ============ ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ============

def _get_profile(user):
    """Получить или создать профиль пользователя"""
    return Profile.objects.get_or_create(user=user)[0]

def _is_admin(user):
    """Проверка, является ли пользователь администратором"""
    if not user.is_authenticated:
        return False
    return _get_profile(user).role == 'admin'

def _is_moderator(user):
    """Проверка, является ли пользователь модератором"""
    if not user.is_authenticated:
        return False
    return _get_profile(user).role in ['admin', 'moderator']

# ============ ДЕКОРАТОРЫ ДЛЯ ПРОВЕРКИ РОЛЕЙ ============

def admin_required(view_func):
    """Декоратор: только для администраторов"""
    def wrapper(request, *args, **kwargs):
        if not request.user.is_authenticated:
            messages.error(request, "Требуется авторизация")
            return redirect('login')
        if not _is_admin(request.user):
            messages.error(request, "Требуются права администратора")
            return HttpResponseForbidden("Доступ запрещен: требуется роль администратора")
        return view_func(request, *args, **kwargs)
    return wrapper

def moderator_required(view_func):
    """Декоратор: для администраторов и модераторов"""
    def wrapper(request, *args, **kwargs):
        if not request.user.is_authenticated:
            messages.error(request, "Требуется авторизация")
            return redirect('login')
        if not _is_moderator(request.user):
            messages.error(request, "Требуются права модератора или администратора")
            return HttpResponseForbidden("Доступ запрещен: требуется роль модератора или администратора")
        return view_func(request, *args, **kwargs)
    return wrapper

# ============ ПУБЛИЧНЫЕ СТРАНИЦЫ ============

@require_GET
def index(request):
    """Главная страница"""
    my_lists = [("/secure/items/", "Мои объекты")]
    
    # Показываем ссылки ТОЛЬКО если пользователь авторизован и имеет права
    actions = []
    if request.user.is_authenticated:
        if _is_admin(request.user):
            actions = [
                ("/secure/delete_user/", "Управление пользователями (безопасно)"),
                ("/secure/admin_panel/", "Админ-панель (безопасно)"),
                ("/secure/promote_user/", "Управление ролями (безопасно)"),
            ]
        elif _is_moderator(request.user):
            actions = [
                ("/secure/admin_panel/", "Панель модератора (безопасно)"),
            ]
    
    return render(request, "index.html", {
        "my_lists": my_lists, 
        "actions": actions,
        "domain_desc": "Блог: исправленные уязвимости повышения привилегий",
        "user_role": _get_profile(request.user).role if request.user.is_authenticated else None
    })

# ============ АУТЕНТИФИКАЦИЯ ============

def login_view(request):
    """Авторизация"""
    if request.method == "POST":
        form = LoginForm(request.POST)
        if form.is_valid():
            user = authenticate(
                request, 
                username=form.cleaned_data["username"], 
                password=form.cleaned_data["password"]
            )
            if user:
                login(request, user)
                messages.success(request, f"Добро пожаловать, {user.username}!")
                return redirect("index")
            messages.error(request, "Неверные данные")
    else:
        form = LoginForm()
    return render(request, "login.html", {"form": form})

def logout_view(request):
    """Выход"""
    logout(request)
    messages.info(request, "Вы вышли из системы")
    return redirect("index")

# ============ ОБЪЕКТЫ (ITEMS) ============

@login_required
@require_GET
def items_list(request):
    """Список своих объектов"""
    items = Item.objects.filter(owner=request.user).order_by("-id")
    return render(request, "items_list.html", {"items": items})

# ============ УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ ============

@login_required
@admin_required
@require_POST
@csrf_protect
def delete_user_secure(request, user_id):
    """Безопасное удаление пользователя (только админ, нельзя удалить себя)"""
    # Запрещаем удалять самого себя
    if request.user.id == user_id:
        messages.error(request, "Нельзя удалить свою учетную запись")
        return redirect("index")
    
    target = get_object_or_404(User, id=user_id)
    username = target.username
    target.delete()
    messages.success(request, f"Пользователь {username} успешно удален")
    return redirect("index")

# ============ АДМИН-ПАНЕЛЬ ============

@login_required
@moderator_required
@require_GET
def admin_panel_secure(request):
    """Безопасная админ-панель (для админов и модераторов)"""
    context = {
        "mode": "secure",
        "role": _get_profile(request.user).role,
        "users": User.objects.all().order_by("-date_joined")[:10] if _is_admin(request.user) else None
    }
    return render(request, "admin_panel.html", context)

# ============ УПРАВЛЕНИЕ РОЛЯМИ ============

@login_required
@admin_required
@require_POST
@csrf_protect
def promote_user_secure(request, user_id):
    """Безопасное повышение роли (только админ, валидация ролей)"""
    # Запрещаем менять роль администратора
    target = get_object_or_404(User, id=user_id)
    target_profile = _get_profile(target)
    
    # Запрещаем админу понижать самого себя
    if request.user.id == user_id:
        messages.error(request, "Нельзя изменить свою роль")
        return redirect("index")
    
    # Валидация роли — ТОЛЬКО из предопределенного списка!
    allowed_roles = ["user", "moderator"]
    new_role = request.POST.get("role", "user")
    
    if new_role not in allowed_roles:
        messages.error(request, f"Недопустимая роль: {new_role}")
        return redirect("index")
    
    # Сохраняем новую роль
    old_role = target_profile.role
    target_profile.role = new_role
    target_profile.save()
    
    messages.success(
        request, 
        f"Роль пользователя {target.username} изменена: {old_role} → {new_role}"
    )
    return redirect("index")

# ============ УСТАРЕВШИЕ/УЯЗВИМЫЕ МАРШРУТЫ ============
# ❌ ВСЕ НИЖЕСЛЕДУЮЩИЕ ФУНКЦИИ УДАЛЯЕМ!
# delete_user_vuln, admin_panel_vuln, promote_user_vuln