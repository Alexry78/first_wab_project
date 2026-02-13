-- Создание таблицы patients
CREATE TABLE IF NOT EXISTS patients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    full_name TEXT NOT NULL,
    birth_date DATE NOT NULL,
    gender TEXT NOT NULL CHECK(gender IN ('M', 'F'))
);

-- Создание таблицы doctors
CREATE TABLE IF NOT EXISTS doctors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    specialization TEXT NOT NULL,
    phone TEXT UNIQUE NOT NULL
);

-- Индексы для ускорения поиска
CREATE INDEX idx_patients_name ON patients(full_name);
CREATE INDEX idx_doctors_specialization ON doctors(specialization);
CREATE INDEX idx_doctors_phone ON doctors(phone);