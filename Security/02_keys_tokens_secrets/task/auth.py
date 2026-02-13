from typing import Any, Tuple
import crypto
import user
import storage
from datetime import datetime

def record_token(payload: dict[str, Any]) -> None:
    """Сохраняет токен в хранилище для возможности отзыва"""
    db = storage.load_tokens()
    db["tokens"].append({
        "jti": payload["jti"],
        "sub": payload["sub"],
        "typ": payload["typ"],
        "exp": payload["exp"],
        "revoked": False
    })
    storage.save_tokens(db)

def revoke_by_jti(jti: str) -> None:
    """Отзывает токен по jti"""
    db = storage.load_tokens()
    for token in db["tokens"]:
        if token["jti"] == jti:
            token["revoked"] = True
            break
    storage.save_tokens(db)

def is_revoked(jti: str) -> bool:
    """Проверяет, отозван ли токен"""
    db = storage.load_tokens()
    for token in db["tokens"]:
        if token["jti"] == jti:
            return token.get("revoked", False)
    return False

def is_expired(exp: int) -> bool:
    """Проверяет, истек ли токен"""
    return datetime.now().timestamp() > exp

def login(username: str, password: str) -> Tuple[str, str]:
    # Найти пользователя
    u = user.get_user(username)
    if not u:
        raise ValueError("Invalid credentials")
    
    # Проверить пароль
    if not user.verify_password(u, password):
        raise ValueError("Invalid credentials")
    
    # Выпустить токены
    access_token, access_payload = crypto.issue_access(username)
    refresh_token, refresh_payload = crypto.issue_refresh(username)
    
    # Сохранить токены
    record_token(access_payload)
    record_token(refresh_payload)
    
    return access_token, refresh_token

def verify_access(access: str) -> dict[str, Any]:
    payload = crypto.decode(access)
    
    # Проверить тип токена
    if payload.get("typ") != "access":
        raise ValueError("Not an access token")
    
    # Проверить отзыв
    if is_revoked(payload["jti"]):
        raise ValueError("Token revoked")
    
    # Проверить срок действия
    if is_expired(payload["exp"]):
        raise ValueError("Token expired")
    
    return payload

def refresh_pair(refresh_token: str) -> Tuple[str, str]:
    # Декодировать refresh token
    payload = crypto.decode(refresh_token)
    
    # Проверить тип
    if payload.get("typ") != "refresh":
        raise ValueError("Not a refresh token")
    
    # Проверить отзыв
    if is_revoked(payload["jti"]):
        raise ValueError("Token revoked")
    
    # Проверить срок действия
    if is_expired(payload["exp"]):
        raise ValueError("Refresh token expired")
    
    # Отозвать старый refresh token
    revoke_by_jti(payload["jti"])
    
    # Выпустить новую пару
    username = payload["sub"]
    return login(username, "")  # пароль не нужен, так как уже аутентифицированы

def revoke(token: str) -> None:
    payload = crypto.decode(token)
    revoke_by_jti(payload["jti"])

def introspect(token: str) -> dict[str, Any]:
    try:
        payload = crypto.decode(token)
        active = (not is_revoked(payload["jti"])) and (not is_expired(payload["exp"]))
        return {
            "active": active,
            "sub": payload.get("sub"),
            "typ": payload.get("typ"),
            "exp": payload.get("exp"),
            "jti": payload.get("jti"),
        }
    except Exception as e:
        return {"active": False, "error": str(e)}