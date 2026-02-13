import hashlib
import time
from passlib.context import CryptContext
from validation import validate_password
from user import User, UserStorage

# Настройка контекста для Argon2 с поддержкой MD5 для миграции
pwd_context = CryptContext(
    schemes=["argon2", "md5"],
    deprecated=["md5"],
    
    # Настройки Argon2 для безопасности
    argon2__time_cost=2,      # количество итераций
    argon2__memory_cost=102400,  # 100 MB памяти
    argon2__parallelism=8,    # параллельные потоки
    argon2__hash_len=32,      # длина хеша
)

# Простой rate limiting (в реальном проекте использовать Redis)
_login_attempts = {}  # username -> (attempts, first_attempt_time)

def _check_rate_limit(username: str) -> bool:
    """
    Проверка rate limiting: не более 5 попыток за 15 минут
    """
    now = time.time()
    if username in _login_attempts:
        attempts, first_time = _login_attempts[username]
        if now - first_time > 900:  # 15 минут
            # Сбрасываем счетчик
            _login_attempts[username] = (1, now)
            return True
        elif attempts >= 5:
            return False
        else:
            _login_attempts[username] = (attempts + 1, first_time)
            return True
    else:
        _login_attempts[username] = (1, now)
        return True

def _record_login_attempt(username: str, success: bool):
    """
    Запись успешного входа (сбрасывает счетчик)
    """
    if success and username in _login_attempts:
        del _login_attempts[username]


def register_user(storage: UserStorage, username: str, email: str, password: str) -> User:
    """
    Создает пользователя с Argon2 хешем пароля.
    """
    if User.exists(storage, username):
        raise ValueError("Пользователь с таким username уже существует")

    # Валидация пароля
    validate_password(password)

    # Хешируем пароль Argon2 (с солью автоматически)
    password_hash = pwd_context.hash(password)
    
    user = User(username=username, email=email, password_hash=password_hash)
    user.save(storage)
    
    # Безопасно затираем пароль в памяти (насколько это возможно в Python)
    # В реальном проекте использовать secrets.compare_digest и bytearray
    password = None
    
    return user


def verify_credentials(storage: UserStorage, username: str, password: str) -> bool:
    """
    Проверяет учетные данные с автоматической миграцией на Argon2.
    """
    # Rate limiting
    if not _check_rate_limit(username):
        time.sleep(1)  # Дополнительная задержка
        return False

    user = User.load(storage, username)
    if user is None:
        return False

    # Проверка пароля (автоматически определяет алгоритм)
    try:
        is_valid = pwd_context.verify(password, user.password_hash)
    except Exception:
        is_valid = False

    if is_valid:
        # Миграция: если пароль был в MD5, обновляем до Argon2
        if user.password_hash.startswith('md5$') or len(user.password_hash) == 32:
            # Перехешируем пароль Argon2
            user.password_hash = pwd_context.hash(password)
            user.save(storage)
            print(f"Миграция пароля для {username} на Argon2 выполнена")
        
        # Запись успешного входа
        _record_login_attempt(username, True)
        
        # Безопасно затираем пароль
        password = None
        
        return True
    else:
        return False


def change_password(storage: UserStorage, username: str, old_password: str, new_password: str) -> bool:
    """
    Смена пароля с проверкой старого.
    """
    if not verify_credentials(storage, username, old_password):
        return False
    
    validate_password(new_password)
    
    user = User.load(storage, username)
    user.password_hash = pwd_context.hash(new_password)
    user.save(storage)
    
    return True