# user.py
from dataclasses import dataclass
from typing import Protocol, Optional, Dict
import secrets
import time


class UserStorage(Protocol):
    def get_user(self, username: str) -> Optional[Dict]: ...
    def save_user(self, record: Dict) -> None: ...
    def exists(self, username: str) -> bool: ...


@dataclass
class User:
    username: str
    email: str
    password_hash: str
    created_at: Optional[float] = None
    last_login: Optional[float] = None
    failed_attempts: int = 0
    locked_until: Optional[float] = None

    def __post_init__(self):
        if self.created_at is None:
            self.created_at = time.time()

    def save(self, storage: UserStorage) -> None:
        storage.save_user({
            "username": self.username,
            "email": self.email,
            "password_hash": self.password_hash,
            "created_at": self.created_at,
            "last_login": self.last_login,
            "failed_attempts": self.failed_attempts,
            "locked_until": self.locked_until,
        })

    @classmethod
    def load(cls, storage: UserStorage, username: str) -> Optional["User"]:
        rec = storage.get_user(username)
        if rec is None:
            return None
        return cls(**rec)

    @classmethod
    def exists(cls, storage: UserStorage, username: str) -> bool:
        return storage.exists(username)

    def is_locked(self) -> bool:
        """Проверка, заблокирован ли пользователь"""
        if self.locked_until and time.time() < self.locked_until:
            return True
        return False

    def record_failed_attempt(self):
        """Запись неудачной попытки входа"""
        self.failed_attempts += 1
        if self.failed_attempts >= 5:
            # Блокировка на 15 минут
            self.locked_until = time.time() + 900

    def record_successful_login(self):
        """Запись успешного входа"""
        self.last_login = time.time()
        self.failed_attempts = 0
        self.locked_until = None


class InMemoryUserStorage:
    """Учебное хранилище на словаре."""
    def __init__(self) -> None:
        self._db: Dict[str, Dict] = {}

    def get_user(self, username: str) -> Optional[Dict]:
        return self._db.get(username)

    def save_user(self, record: Dict) -> None:
        # Делаем копию, чтобы избежать изменений извне
        self._db[record["username"]] = dict(record)

    def exists(self, username: str) -> bool:
        return username in self._db