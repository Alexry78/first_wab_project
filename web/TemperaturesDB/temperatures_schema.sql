-- Создание таблицы для хранения температуры в городах
CREATE TABLE IF NOT EXISTS city_temperatures (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    city_name TEXT NOT NULL,
    measurement_date DATE NOT NULL,
    temperature_celsius REAL NOT NULL CHECK(temperature_celsius >= -60 AND temperature_celsius <= 60),
    UNIQUE(city_name, measurement_date)  -- В один день для города одно измерение
);

-- Индекс для ускорения поиска по городам и датам
CREATE INDEX idx_city_date ON city_temperatures(city_name, measurement_date);
CREATE INDEX idx_temperature ON city_temperatures(temperature_celsius);

-- Добавление пяти записей с измерениями
INSERT INTO city_temperatures (city_name, measurement_date, temperature_celsius) VALUES
    ('Москва', '2025-02-13', -5.5),
    ('Санкт-Петербург', '2025-02-13', -8.2),
    ('Сочи', '2025-02-13', 12.5),
    ('Новосибирск', '2025-02-13', -22.0),
    ('Калининград', '2025-02-13', -3.0);

-- Добавим ещё несколько записей для демонстрации
INSERT INTO city_temperatures (city_name, measurement_date, temperature_celsius) VALUES
    ('Москва', '2025-02-12', -7.0),
    ('Санкт-Петербург', '2025-02-12', -10.5),
    ('Сочи', '2025-02-12', 11.0),
    ('Новосибирск', '2025-02-12', -25.5),
    ('Калининград', '2025-02-12', -4.5);