import time

def rate_limit(calls, per_seconds):
    """Декоратор для ограничения частоты вызовов"""
    def decorator(func):
        call_times = []
        
        def wrapper(*args, **kwargs):
            nonlocal call_times
            current_time = time.time()
            
            # Удаляем старые вызовы
            call_times = [t for t in call_times if current_time - t < per_seconds]
            
            # Проверяем лимит
            if len(call_times) >= calls:
                raise RuntimeError(f"Лимит {calls} вызовов за {per_seconds}с превышен")
            
            call_times.append(current_time)
            return func(*args, **kwargs)
        
        return wrapper
    return decorator

@rate_limit(calls=2, per_seconds=1.0)
def ping():
    return "pong"

# ТЕСТ 1: Первые 2 вызова - успех
print("Тест 1: Первые 2 вызова")
print(ping())  # pong
print(ping())  # pong

# ТЕСТ 2: 3-й вызов - ошибка
print("\nТест 2: 3-й вызов сразу")
try:
    print(ping())
except RuntimeError as e:
    print(f"Ошибка: {e}")

# ТЕСТ 3: Ждем и снова вызываем
print("\nТест 3: Ждем 1 секунду...")
time.sleep(1)
print(ping())  # pong
print(ping())  # pong
