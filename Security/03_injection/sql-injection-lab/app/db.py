import aiosqlite
import os
from dotenv import load_dotenv

load_dotenv()

DATABASE_URL = os.getenv("DATABASE_URL", "sqlite+aiosqlite:///./sqli_lab.db")
_db_connection = None

async def get_pool():
    """Возвращает соединение с SQLite (совместимо с интерфейсом asyncpg)"""
    global _db_connection
    if _db_connection is None:
        _db_connection = await aiosqlite.connect("./sqli_lab.db")
        _db_connection.row_factory = aiosqlite.Row
    return _db_connection

async def close_pool():
    """Закрывает соединение с SQLite"""
    global _db_connection
    if _db_connection:
        await _db_connection.close()
        _db_connection = None