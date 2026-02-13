from django.db import models
from django.contrib.auth.models import User
from django.core.exceptions import ValidationError

class Profile(models.Model):
    ROLE_CHOICES = [
        ('user', 'Пользователь'),
        ('moderator', 'Модератор'),
        ('admin', 'Администратор'),
    ]
    
    user = models.OneToOneField(User, on_delete=models.CASCADE, related_name='profile')
    role = models.CharField(max_length=20, choices=ROLE_CHOICES, default='user')
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    
    def __str__(self):
        return f"{self.user.username} - {self.get_role_display()}"
    
    def clean(self):
        """Валидация роли"""
        if self.role not in dict(self.ROLE_CHOICES):
            raise ValidationError(f"Недопустимая роль: {self.role}")
    
    class Meta:
        verbose_name = "Профиль"
        verbose_name_plural = "Профили"
        permissions = [
            ("can_view_admin_panel", "Может просматривать админ-панель"),
            ("can_manage_users", "Может управлять пользователями"),
            ("can_manage_roles", "Может управлять ролями"),
        ]

class Item(models.Model):
    owner = models.ForeignKey(User, on_delete=models.CASCADE, related_name='items')
    title = models.CharField(max_length=200)
    created_at = models.DateTimeField(auto_now_add=True)
    
    def __str__(self):
        return f"{self.title} (владелец: {self.owner.username})"
    
    class Meta:
        ordering = ['-created_at']