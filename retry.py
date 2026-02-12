def retry3(func):
    """Декоратор для повторного запуска функции до 3 раз при ошибках"""
    def wrapper(*args, **kwargs):
        last_exception = None
        for attempt in range(3):
            try:
                return func(*args, **kwargs)
            except Exception as e:
                last_exception = e
                print(f"Попытка {attempt + 1}: {e}")
        # Если дошли сюда - все 3 попытки провалились
        raise last_exception
    return wrapper


# ТЕСТ 1: Успех со 2-й попытки
print("=" * 50)
print("ТЕСТ 1: Успех после 2-х неудач")
print("=" * 50)

i = 0

@retry3
def flaky():
    global i
    i += 1
    if i < 3:
        raise ValueError("fail")
    return "success"

print(f"Результат: {flaky()}")  # success
print(f"Всего попыток: {i}\n")


# ТЕСТ 2: Все 3 попытки неудачны
print("=" * 50)
print("ТЕСТ 2: 3 неудачи - последнее исключение")
print("=" * 50)

j = 0

@retry3
def always_fails():
    global j
    j += 1
    raise ValueError(f"Ошибка #{j}")

try:
    always_fails()
except Exception as e:
    print(f"Поймано исключение: {e}")
print(f"Всего попыток: {j}\n")


# ТЕСТ 3: Успех с первого раза
print("=" * 50)
print("ТЕСТ 3: Успех с первой попытки")
print("=" * 50)

@retry3
def always_works():
    return "успех"

print(f"Результат: {always_works()}\n")


# ТЕСТ 4: Функция с аргументами
print("=" * 50)
print("ТЕСТ 4: Функция с аргументами")
print("=" * 50)

counter = 0

@retry3
def divide(a, b):
    global counter
    counter += 1
    if b == 0:
        raise ZeroDivisionError("Деление на ноль!")
    return a / b

# Успех
print(f"10 / 2 = {divide(10, 2)}")
print(f"Попыток: {counter}\n")

counter = 0
# Неудача - деление на ноль
try:
    divide(10, 0)
except Exception as e:
    print(f"Ошибка: {e}")
print(f"Попыток: {counter}")
