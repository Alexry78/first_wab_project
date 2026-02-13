from fastapi import HTTPException, Header
from .db import get_pool

async def get_user_by_token(authorization: str | None = Header(None)):
    if not authorization:
        raise HTTPException(status_code=401, detail="Missing Authorization header")
    if not authorization.lower().startswith("bearer "):
        raise HTTPException(status_code=401, detail="Invalid Authorization header")
    token_value = authorization[7:]
    pool = await get_pool()
    # Для SQLite используем ? вместо $1
    cursor = await pool.execute(
        "SELECT u.id, u.name FROM tokens t JOIN users u ON u.id = t.user_id WHERE t.value = ? AND t.is_valid = 1",
        (token_value,)
    )
    row = await cursor.fetchone()
    if not row:
        raise HTTPException(status_code=401, detail="Invalid token")
    return {"id": row["id"], "name": row["name"]}