import re

def validate_password(password: str) -> bool:
    """
    Валидация сложности пароля.
    """
    if len(password) < 8:
        raise ValueError("Пароль должен быть минимум 8 символов")
    
    if not re.search(r"[A-Z]", password):
        raise ValueError("Пароль должен содержать заглавную букву")
    
    if not re.search(r"[a-z]", password):
        raise ValueError("Пароль должен содержать строчную букву")
    
    if not re.search(r"\d", password):
        raise ValueError("Пароль должен содержать цифру")
    
    if not re.search(r"[!@#$%^&*(),.?\":{}|<>]", password):
        raise ValueError("Пароль должен содержать спецсимвол")
    
    return True