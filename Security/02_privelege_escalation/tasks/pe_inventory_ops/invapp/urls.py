from django.urls import path
from . import views

urlpatterns = [
    path('', views.index, name='index'),
    path('login/', views.login_view, name='login'),
    path('logout/', views.logout_view, name='logout'),

    # ✅ БЕЗОПАСНЫЕ МАРШРУТЫ (ТОЛЬКО ИХ ОСТАВЛЯЕМ)
    path('secure/items/', views.items_list, name='items_list'),
    path('secure/delete_user/<int:user_id>/', views.delete_user_secure, name='delete_user_secure'),
    path('secure/admin_panel/', views.admin_panel_secure, name='admin_panel_secure'),
    path('secure/promote_user/<int:user_id>/', views.promote_user_secure, name='promote_user_secure'),
]

# ❌ УЯЗВИМЫЕ МАРШРУТЫ УДАЛЕНЫ:
# 'vuln/delete_user/'
# 'vuln/admin_panel/' 
# 'vuln/promote_user/'