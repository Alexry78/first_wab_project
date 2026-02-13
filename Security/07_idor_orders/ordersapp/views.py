from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth import authenticate, login, logout
from django.contrib import messages
from django.contrib.auth.decorators import login_required
from django.views.decorators.http import require_POST, require_GET
from django.http import HttpResponseForbidden, Http404
from .forms import LoginForm
from .models import Order, Invoice

def index(request):
    """Главная страница"""
    my_lists = [
        ("/secure/order/list/", "Order: мои объекты"),
        ("/secure/invoice/list/", "Invoice: мои объекты")
    ]
    return render(request, "index.html", {
        "my_lists": my_lists, 
        "domain_desc": "Заказы/Счета — защищенная версия (IDOR исправлены)"
    })

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
                messages.success(request, "Вы успешно вошли")
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


@login_required
@require_GET
def order_list(request):
    """Безопасный список заказов - ТОЛЬКО СВОИ"""
    objs = Order.objects.filter(owner=request.user).order_by("-id")
    return render(request, "ordersapp/order_list.html", {
        "objects": objs,
        "title": "Мои заказы"
    })

@login_required
def order_detail_secure(request, obj_id):
    """Безопасный просмотр заказа - ТОЛЬКО ДЛЯ ВЛАДЕЛЬЦА"""
    obj = get_object_or_404(Order, id=obj_id, owner=request.user)
    return render(request, "ordersapp/order_detail.html", {
        "obj": obj, 
        "mode": "secure"
    })

@login_required
def order_detail_vuln_path(request, obj_id):
    """Перенаправляем с уязвимого маршрута на защищенный"""
    return redirect('order_detail_secure', obj_id=obj_id)

@login_required
@require_POST
def order_update_vuln(request, obj_id):
    """Безопасное обновление заказа - ТОЛЬКО ДЛЯ ВЛАДЕЛЬЦА"""
    # Критическая проверка владельца
    obj = get_object_or_404(Order, id=obj_id, owner=request.user)
    
    if 'title' in request.POST:
        obj.title = request.POST['title']
        obj.save()
        messages.success(request, f"Заказ #{obj.id} обновлен")
    else:
        messages.error(request, "Поле title не передано")
    
    return redirect("order_list")


@login_required
@require_GET
def invoice_list(request):
    """Безопасный список счетов - ТОЛЬКО СВОИ"""
    objs = Invoice.objects.filter(owner=request.user).order_by("-id")
    return render(request, "ordersapp/invoice_list.html", {
        "objects": objs,
        "title": "Мои счета"
    })


@login_required
def invoice_detail_secure(request, obj_id):
    """Безопасный просмотр счета - ТОЛЬКО ДЛЯ ВЛАДЕЛЬЦА"""
    obj = get_object_or_404(Invoice, id=obj_id, owner=request.user)
    return render(request, "ordersapp/invoice_detail.html", {
        "obj": obj, 
        "mode": "secure"
    })

@login_required
def invoice_detail_vuln_path(request, obj_id):
    """Перенаправляем с уязвимого маршрута на защищенный"""
    return redirect('invoice_detail_secure', obj_id=obj_id)

@login_required
@require_POST
def invoice_update_vuln(request, obj_id):
    """Безопасное обновление счета - ТОЛЬКО ДЛЯ ВЛАДЕЛЬЦА"""
    obj = get_object_or_404(Invoice, id=obj_id, owner=request.user)
    
    if 'title' in request.POST:
        obj.title = request.POST['title']
        obj.save()
        messages.success(request, f"Счет #{obj.id} обновлен")
    else:
        messages.error(request, "Поле title не передано")
    
    return redirect("invoice_list")