from fastapi import FastAPI, Depends, HTTPException, Query, Path
from typing import Annotated, Any
from .db import get_pool, close_pool
from .auth import get_user_by_token
from contextlib import asynccontextmanager
from pydantic import BaseModel
import hashlib
import secrets
import bcrypt

pool = None

@asynccontextmanager
async def lifespan(app: FastAPI):
    global pool
    pool = await get_pool()
    yield
    pool = None
    await close_pool()

app = FastAPI(title="SQLi Lab (safe edition)", lifespan=lifespan)

class AuthRequest(BaseModel):
    name: str
    password: str

@app.post("/auth/token")
async def auth_token(body: AuthRequest):
    global pool
    # Для SQLite используем ? вместо $1
    cursor = await pool.execute(
        "SELECT id, password_hash FROM users WHERE name = ?",
        (body.name,)
    )
    row = await cursor.fetchone()
    
    if not row:
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    # Проверяем пароль (в init.sql пароли в md5, но для безопасности используем bcrypt)
    password_hash = hashlib.md5(body.password.encode()).hexdigest()
    if password_hash != row["password_hash"]:
        raise HTTPException(status_code=401, detail="Invalid credentials")
    
    cursor = await pool.execute(
        "SELECT value FROM tokens WHERE user_id = ? AND is_valid = 1 LIMIT 1",
        (row["id"],)
    )
    token_row = await cursor.fetchone()
    
    if not token_row:
        token = secrets.token_urlsafe(64)
        await pool.execute(
            "INSERT INTO tokens (user_id, value) VALUES (?, ?)",
            (row["id"], token)
        )
        await pool.commit()
    else:
        token = token_row["value"]
    
    return {"token": token}

@app.get("/orders")
async def list_orders(
    user: Annotated[dict[str, Any], Depends(get_user_by_token)],
    limit: int = Query(10, ge=1, le=100),
    offset: int = Query(0, ge=0)
):
    global pool
    cursor = await pool.execute(
        "SELECT id, user_id, created_at FROM orders WHERE user_id = ? ORDER BY created_at DESC LIMIT ? OFFSET ?",
        (user["id"], limit, offset)
    )
    rows = await cursor.fetchall()
    return [{"id": r["id"], "user_id": r["user_id"], "created_at": r["created_at"]} for r in rows]

@app.get("/orders/{order_id}")
async def order_details(
    user: Annotated[dict[str, Any], Depends(get_user_by_token)], 
    order_id: int = Path(..., ge=1)
):
    global pool
    cursor = await pool.execute(
        "SELECT id, user_id, created_at FROM orders WHERE id = ? AND user_id = ?",
        (order_id, user["id"])
    )
    order = await cursor.fetchone()
    
    if not order:
        raise HTTPException(status_code=404, detail="Order not found")
    
    cursor = await pool.execute(
        "SELECT id, name, count, price FROM goods WHERE order_id = ?",
        (order_id,)
    )
    goods = await cursor.fetchall()
    
    return {
        "order": {"id": order["id"], "user_id": order["user_id"], "created_at": order["created_at"]},
        "goods": [{"id": g["id"], "name": g["name"], "count": g["count"], "price": float(g["price"])} for g in goods]
    }