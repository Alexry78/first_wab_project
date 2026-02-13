
DROP TABLE IF EXISTS film_credit CASCADE;
DROP TABLE IF EXISTS film_info CASCADE;
DROP TABLE IF EXISTS film CASCADE;
DROP TABLE IF EXISTS director CASCADE;


CREATE TABLE director (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL UNIQUE,  
    country VARCHAR(100)                
);


CREATE TABLE film (
    id SERIAL PRIMARY KEY,
    title VARCHAR(300) NOT NULL,
    release_year INTEGER NOT NULL,
    primary_director_id INTEGER NOT NULL,
    

    CONSTRAINT film_release_year_check 
        CHECK (release_year BETWEEN 1900 AND EXTRACT(YEAR FROM CURRENT_DATE)),
    

    CONSTRAINT film_primary_director_fk 
        FOREIGN KEY (primary_director_id) 
        REFERENCES director(id)
        ON DELETE RESTRICT  


CREATE INDEX idx_film_primary_director ON film(primary_director_id);


CREATE TABLE film_info (
    film_id INTEGER PRIMARY KEY,  
    duration_minutes INTEGER NOT NULL,
    rating VARCHAR(5) NOT NULL,
    budget_usd DECIMAL(15,2),
    

    CONSTRAINT film_info_duration_check 
        CHECK (duration_minutes > 0),
    
    CONSTRAINT film_info_rating_check 
        CHECK (rating IN ('G', 'PG', 'PG-13', 'R', 'NC-17')),
    
  
    CONSTRAINT film_info_film_fk 
        FOREIGN KEY (film_id) 
        REFERENCES film(id)
        ON DELETE CASCADE  


CREATE TABLE film_credit (
    film_id INTEGER NOT NULL,
    director_id INTEGER NOT NULL,
    role VARCHAR(50) NOT NULL,
    
 
    CONSTRAINT film_credit_pk 
        PRIMARY KEY (film_id, director_id, role),
    
  
    CONSTRAINT film_credit_film_fk 
        FOREIGN KEY (film_id) 
        REFERENCES film(id)
        ON DELETE CASCADE, 

    CONSTRAINT film_credit_director_fk 
        FOREIGN KEY (director_id) 
        REFERENCES director(id)
        ON DELETE RESTRICT,  
    -
    CONSTRAINT film_credit_role_check 
        CHECK (role IN ('director', 'co-director', 'producer', 'screenwriter', 
                        'executive producer', 'assistant director'))
);

CREATE INDEX idx_film_credit_film ON film_credit(film_id);
CREATE INDEX idx_film_credit_director ON film_credit(director_id);

INSERT INTO director (name, country) VALUES
('Кристофер Нолан', 'Великобритания'),
('Квентин Тарантино', 'США'),
('Дени Вильнёв', 'Канада'),
('Грета Гервиг', 'США'),
('Пон Джун-хо', 'Южная Корея'),
('Мартин Скорсезе', 'США');

INSERT INTO film (title, release_year, primary_director_id) VALUES
('Начало', 2010, 1),
('Интерстеллар', 2014, 1),
('Криминальное чтиво', 1994, 2),
('Бесславные ублюдки', 2009, 2),
('Дюна', 2021, 3),
('Прибытие', 2016, 3),
('Барби', 2023, 4),
('Паразиты', 2019, 5),
('Волк с Уолл-стрит', 2013, 6);

INSERT INTO film_info (film_id, duration_minutes, rating, budget_usd) VALUES
(1, 148, 'PG-13', 160000000),
(2, 169, 'PG-13', 165000000),
(3, 154, 'R', 8000000),
(4, 153, 'R', 70000000),
(5, 155, 'PG-13', 165000000),
(6, 116, 'PG-13', 47000000),
(7, 114, 'PG-13', 145000000),
(8, 132, 'R', 11400000),
(9, 180, 'R', 100000000);

INSERT INTO film_credit (film_id, director_id, role) VALUES
-- Начало (дополнительные режиссёры)
(1, 2, 'executive producer'),  -- Тарантино как исполнительный продюсер
-- Криминальное чтиво (дополнительные)
(3, 1, 'producer'),            -- Нолан как продюсер
-- Дюна (дополнительные)
(5, 4, 'co-director'),         -- Гервиг как со-режиссёр
(5, 6, 'screenwriter'),        -- Скорсезе как сценарист
-- Паразиты (дополнительные)
(8, 3, 'executive producer'),  -- Вильнёв как исполнительный продюсер
(8, 1, 'producer'),            -- Нолан как продюсер
-- Барби (дополнительные)
(7, 5, 'co-director'),         -- Пон Джун-хо как со-режиссёр
-- Волк с Уолл-стрит (дополнительные)
(9, 2, 'executive producer');  -- Тарантино как исполнительный продюсер

SELECT 
    f.title AS "Фильм",
    f.release_year AS "Год",
    d.name AS "Основной режиссёр",
    d.country AS "Страна"
FROM film f
JOIN director d ON f.primary_director_id = d.id
ORDER BY f.release_year DESC;

SELECT 
    f.title AS "Фильм",
    fi.duration_minutes AS "Длительность",
    fi.rating AS "Рейтинг",
    fi.budget_usd AS "Бюджет ($)"
FROM film f
JOIN film_info fi ON f.id = fi.film_id
ORDER BY fi.budget_usd DESC;

SELECT 
    f.title AS "Фильм",
    d.name AS "Участник",
    fc.role AS "Роль",
    f.release_year AS "Год фильма"
FROM film_credit fc
JOIN film f ON fc.film_id = f.id
JOIN director d ON fc.director_id = d.id
ORDER BY f.title, fc.role;

SELECT 
    f.title AS "Фильм",
    f.release_year AS "Год",
    main_d.name AS "Основной режиссёр",
    fi.duration_minutes AS "Длительность (мин)",
    fi.rating AS "Рейтинг",
    fi.budget_usd AS "Бюджет ($)",
    COUNT(DISTINCT fc.director_id) AS "Дополнительных участников"
FROM film f
JOIN director main_d ON f.primary_director_id = main_d.id
JOIN film_info fi ON f.id = fi.film_id
LEFT JOIN film_credit fc ON f.id = fc.film_id
GROUP BY f.id, f.title, f.release_year, main_d.name, 
         fi.duration_minutes, fi.rating, fi.budget_usd
ORDER BY f.release_year DESC;

SELECT 
    'Режиссёров' AS "Категория", COUNT(*) AS "Количество" FROM director
UNION ALL
SELECT 'Фильмов', COUNT(*) FROM film
UNION ALL
SELECT 'Записей в film_info', COUNT(*) FROM film_info
UNION ALL
SELECT 'Доп. кредитов', COUNT(*) FROM film_credit;