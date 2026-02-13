-- Создание таблицы books с ограничениями целостности
CREATE TABLE IF NOT EXISTS books (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL CHECK(length(trim(title)) > 0),
    author TEXT NOT NULL CHECK(length(trim(author)) > 0),
    genre TEXT NOT NULL CHECK(length(trim(genre)) > 0),
    price DECIMAL(10,2) NOT NULL CHECK(price >= 0 AND price <= 10000),
    published_date DATE NOT NULL CHECK(published_date >= '1900-01-01' AND published_date <= date('now')),
    -- Дополнительные ограничения для качества данных
    CHECK(title GLOB '*[a-zA-Z0-9]*'),  -- Название должно содержать буквы или цифры
    CHECK(author GLOB '*[a-zA-Z]*')      -- Автор должен содержать буквы
);

-- Индексы для ускорения поиска
CREATE INDEX idx_books_genre ON books(genre);
CREATE INDEX idx_books_title ON books(title);
CREATE INDEX idx_books_author ON books(author);
CREATE INDEX idx_books_price ON books(price);
CREATE INDEX idx_books_published ON books(published_date);
CREATE INDEX idx_books_genre_price ON books(genre, price);