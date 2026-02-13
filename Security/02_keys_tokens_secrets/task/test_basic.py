import pytest
from crypto import _require_secret, JWT_SECRET

def test_secret_exists():
    assert JWT_SECRET is not None
    assert len(JWT_SECRET) > 0

def test_require_secret_raises():
    import crypto
    old_secret = crypto.JWT_SECRET
    crypto.JWT_SECRET = None
    with pytest.raises(RuntimeError):
        crypto._require_secret()
    crypto.JWT_SECRET = old_secret