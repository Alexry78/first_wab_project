from django.core.management.base import BaseCommand
from django.contrib.auth.models import User
from invapp.models import Profile, Item

class Command(BaseCommand):
    help = 'Создает демо-пользователей и предметы'
    
    def handle(self, *args, **kwargs):
        # Создаем админа
        admin = User.objects.create_user('adminroot', password='adminroot123')
        Profile.objects.update_or_create(user=admin, defaults={'role': 'admin'})
        
        # Создаем модератора
        mod = User.objects.create_user('mod', password='modpass123')
        Profile.objects.update_or_create(user=mod, defaults={'role': 'moderator'})
        
        # Создаем обычного пользователя
        dev = User.objects.create_user('dev', password='devpass123')
        Profile.objects.update_or_create(user=dev, defaults={'role': 'user'})
        
        # Создаем предметы
        Item.objects.create(owner=admin, title="Админский предмет")
        Item.objects.create(owner=mod, title="Модераторский предмет")
        Item.objects.create(owner=dev, title="Пользовательский предмет")
        
        self.stdout.write(self.style.SUCCESS('Демо-данные созданы!'))