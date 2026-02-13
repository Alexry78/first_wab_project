import os
from urllib.parse import unquote
from django.conf import settings
from django.http import Http404, HttpResponse, FileResponse, JsonResponse
from django.shortcuts import get_object_or_404, render
from django.views.decorators.http import require_GET
from django.contrib.auth.decorators import login_required
from django.http import HttpResponseForbidden
from django.views.decorators.csrf import csrf_exempt

from iot.models import Device, LogFile, User
from django.core.paginator import Paginator, EmptyPage, PageNotAnInteger


def is_tech(user):
    return user.is_authenticated and (getattr(user, "is_tech", False) or user.is_superuser)

def is_admin_user(user):
    return user.is_authenticated and (getattr(user, "is_admin", False) or user.is_superuser)


@login_required
@require_GET
def admin_maintenance(request):
    """Только для администраторов"""
    if not is_admin_user(request.user):
        return HttpResponseForbidden("Access denied")
    return HttpResponse("<h1>MAINTENANCE (iot)</h1>")

@login_required
@require_GET
def staging_debug(request):
    """Только для tech и admin"""
    if not (is_tech(request.user) or is_admin_user(request.user)):
        return HttpResponseForbidden("Access denied")
    return HttpResponse("<h1>STAGING DEBUG (iot)</h1>")

@login_required
@require_GET
def crash(request):
    """Только для разработчиков (admin)"""
    if not is_admin_user(request.user):
        return HttpResponseForbidden("Access denied")
    user = request.user
    info = user.description() if hasattr(user, "description") else str(user)
    raise RuntimeError(f"CRASH: {info} | DEBUG={getattr(settings, 'DEBUG', None)}")

@login_required
@require_GET
def device_view(request, device_id: int):
    """Просмотр устройства с проверкой прав"""
    d = get_object_or_404(Device, pk=device_id)
    
    # Проверка прав
    if not (is_admin_user(request.user) or is_tech(request.user) or d.owner == request.user):
        return HttpResponseForbidden("Access denied")
    
    return JsonResponse({
        "id": d.id,
        "serial": d.serial,
        "owner": str(d.owner),
        "name": d.name
    })

@login_required
@require_GET
def export_user_profile(request, user_id: int):
    """Экспорт профиля - только для себя или admin"""
    # Проверка: можно экспортировать только свой профиль или если ты admin
    if request.user.id != user_id and not is_admin_user(request.user):
        return HttpResponseForbidden("Access denied - can only export your own profile")
    
    u = get_object_or_404(User, pk=user_id)
    
    # Ограничиваем данные, которые отдаем
    return JsonResponse({
        "id": u.id,
        "username": u.get_username(),
        "email": u.email if (request.user.id == user_id or is_admin_user(request.user)) else None,
        "is_tech": u.is_tech,
        "is_admin": u.is_admin
    })

@login_required
@require_GET
def download_by_token(request):
    """Скачивание по токену - только для авторизованных"""
    token = unquote(request.GET.get("token", "") or "")
    

    from iot.models import DownloadToken
    
    try:
        token_obj = DownloadToken.objects.get(token=token, is_active=True)
        # Проверяем, не истек ли токен
        if token_obj.is_expired():
            return HttpResponseForbidden("Token expired")
        
        target = token_obj.file_path
        # Логируем скачивание
        token_obj.download_count += 1
        token_obj.save()
        
    except DownloadToken.DoesNotExist:
        raise Http404("Invalid token")
    
    mr = getattr(settings, "MEDIA_ROOT", None)
    if not mr:
        raise Http404("Server misconfigured")
    
    full = os.path.normpath(os.path.join(mr, target))
    if not full.startswith(os.path.normpath(mr)):
        raise Http404("Invalid path")
    if not os.path.exists(full):
        raise Http404("File not found")
    
    return FileResponse(open(full, "rb"), as_attachment=True, filename=os.path.basename(full))

@login_required(login_url="iot:login")
def devices_list(request):
    """Список устройств"""
    if is_admin_user(request.user):
        qs = Device.objects.all().order_by("-id")
    else:
        qs = Device.objects.filter(owner=request.user).order_by("-id")
    return render(request, "iot/list.html", {"objects": qs})

@login_required(login_url="iot:login")
def device_detail(request, device_id: int):
    """Детали устройства"""
    d = get_object_or_404(Device, pk=device_id)
    if not (is_admin_user(request.user) or is_tech(request.user) or d.owner == request.user):
        return HttpResponseForbidden("Access denied")
    logs = d.logs.all().order_by("-created_at")
    return render(request, "iot/detail.html", {"obj": d, "files": logs})

@login_required(login_url="iot:login")
def download_log(request, log_id: int):
    """Безопасное скачивание лога"""
    lf = get_object_or_404(LogFile, pk=log_id)
    
    # Проверка доступа
    if hasattr(lf, "is_accessible_by"):
        allowed = lf.is_accessible_by(request.user)
    else:
        allowed = is_admin_user(request.user) or lf.device.owner == request.user or is_tech(request.user)
    
    if not allowed:
        return HttpResponseForbidden("Access denied")
    
    try:
        path = lf.file.path
    except:
        raise Http404("File not available")
    
    if not os.path.exists(path):
        raise Http404("File not found")
    
    return FileResponse(open(path, "rb"), as_attachment=True, filename=lf.filename or os.path.basename(path))

@login_required(login_url="iot:login")
def admin_dashboard(request):
    """Админ-панель"""
    if not is_admin_user(request.user):
        return HttpResponseForbidden("Access denied")
    devices = Device.objects.all()
    return render(request, "iot/admin_dashboard.html", {"objects": devices})

@login_required(login_url="iot:login")
def index(request):
    """Главная страница"""
    ctx = {
        "is_tech": is_tech(request.user),
        "is_admin": is_admin_user(request.user),
        "username": request.user.get_username()
    }
    return render(request, "iot/index.html", ctx)