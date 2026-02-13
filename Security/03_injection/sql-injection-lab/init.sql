DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS goods;
DROP TABLE IF EXISTS tokens;

CREATE TABLE users (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  password_hash TEXT NOT NULL
);

CREATE TABLE orders (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE goods (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  order_id INTEGER NOT NULL,
  count INTEGER NOT NULL,
  price NUMERIC NOT NULL,
  FOREIGN KEY (order_id) REFERENCES orders(id)
);

CREATE TABLE tokens (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  value TEXT NOT NULL,
  user_id INTEGER NOT NULL,
  is_valid BOOLEAN NOT NULL DEFAULT 1,
  FOREIGN KEY (user_id) REFERENCES users(id)
);

INSERT INTO users (id, name, password_hash) VALUES
  (1, 'alice', 'e6d21a69daa4a8ebca755c1d5808b85b'),
  (2, 'bob', '530c81a25a362791fff683c511c2edd8'),
  (3, 'eva', 'f8c87688d560489d977c7414ca146d37');

INSERT INTO orders (id, user_id) VALUES (1, 1);
INSERT INTO orders (id, user_id) VALUES (2, 2);
INSERT INTO orders (id, user_id) VALUES (3, 3);
INSERT INTO orders (id, user_id) VALUES (4, 1);

INSERT INTO goods (id, name, order_id, count, price) VALUES 
  (1, 'widget', 1, 3, 9.99),
  (2, 'widget', 1, 4, 10.99),
  (3, 'widget', 2, 5, 1.99),
  (4, 'widget', 2, 6, 2.99),
  (5, 'widget', 3, 1, 3.99),
  (6, 'widget', 3, 2, 4.99),
  (7, 'widget', 4, 3, 5.99),
  (8, 'widget', 4, 4, 6.99);

INSERT INTO tokens (value, user_id, is_valid) VALUES 
  ('secrettokenAlice', 1, 1),
  ('secrettokenBob', 2, 1),
  ('secrettokenEva', 3, 1);