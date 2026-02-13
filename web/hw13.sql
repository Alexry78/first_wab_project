SELECT 
    h.id AS hotel_id,
    h.name AS hotel_name,
    h.stars,
    c.name AS city_name,
    co.name AS country_name,
    co.region
FROM hotel h
INNER JOIN city c ON h.city_id = c.id
INNER JOIN country co ON c.country_id = co.id
ORDER BY co.name, c.name, h.name;

SELECT 
    t.id AS tourist_id,
    t.name AS tourist_name,
    t.birth_year,
    c.name AS visited_city,
    tc.visited_at
FROM tourist t
INNER JOIN tourist_city tc ON t.id = tc.tourist_id
INNER JOIN city c ON tc.city_id = c.id
ORDER BY t.name, tc.visited_at;


SELECT 
    c.id AS city_id,
    c.name AS city_name,
    h.id AS hotel_id,
    h.name AS hotel_name,
    h.stars
FROM city c
LEFT JOIN hotel h ON c.id = h.city_id
ORDER BY c.name, h.name;

SELECT 
    t.id AS tourist_id,
    t.name AS tourist_name,
    t.birth_year,
    COUNT(tc.city_id) AS cities_visited_count
FROM tourist t
LEFT JOIN tourist_city tc ON t.id = tc.tourist_id
GROUP BY t.id, t.name, t.birth_year
ORDER BY cities_visited_count DESC, t.name;


SELECT 
    co.id AS country_id,
    co.name AS country_name,
    co.region,
    c.id AS city_id,
    c.name AS city_name,
    c.population
FROM city c
RIGHT JOIN country co ON c.country_id = co.id
ORDER BY co.name, c.name;

SELECT 
    rt.id AS room_type_id,
    rt.title AS room_type,
    rt.max_guests,
    COUNT(hr.hotel_id) AS hotels_count
FROM hotel_room hr
RIGHT JOIN room_type rt ON hr.room_type_id = rt.id
GROUP BY rt.id, rt.title, rt.max_guests
ORDER BY hotels_count DESC, rt.title;

SELECT 
    COALESCE(c.name, 'Нет города') AS city_name,
    COALESCE(h.name, 'Нет отеля') AS hotel_name,
    CASE 
        WHEN c.id IS NULL THEN 'Отель без города'
        WHEN h.id IS NULL THEN 'Город без отелей'
        ELSE 'Город с отелем'
    END AS relationship
FROM city c
FULL JOIN hotel h ON c.id = h.city_id
ORDER BY city_name, hotel_name;

SELECT 
    COALESCE(rt.title, 'Нет типа') AS room_type,
    COALESCE(h.name, 'Нет отеля') AS hotel_name,
    hr.rooms_available,
    CASE 
        WHEN hr.hotel_id IS NOT NULL THEN 'Тип есть в отеле'
        WHEN h.id IS NOT NULL AND rt.id IS NOT NULL AND hr.hotel_id IS NULL THEN 'Тип отсутствует в отеле'
        WHEN h.id IS NULL THEN 'Отель без типа'
        WHEN rt.id IS NULL THEN 'Тип без отеля'
    END AS status
FROM room_type rt
CROSS JOIN hotel h
FULL JOIN hotel_room hr ON h.id = hr.hotel_id AND rt.id = hr.room_type_id
ORDER BY rt.title, h.name;


SELECT 
    co.name AS country_name,
    co.region,
    rt.title AS room_type,
    rt.max_guests
FROM country co
CROSS JOIN room_type rt
ORDER BY co.name, rt.title;

SELECT DISTINCT
    c.name AS city_name,
    h.year_opened
FROM city c
CROSS JOIN hotel h
WHERE h.year_opened IS NOT NULL
ORDER BY c.name, h.year_opened;


SELECT 
    c.id AS city_id,
    c.name AS city_name,
    hotel_data.hotel_id,
    hotel_data.hotel_name,
    hotel_data.total_rooms
FROM city c
LEFT JOIN LATERAL (
    SELECT 
        h.id AS hotel_id,
        h.name AS hotel_name,
        COALESCE(SUM(hr.rooms_available), 0) AS total_rooms
    FROM hotel h
    LEFT JOIN hotel_room hr ON h.id = hr.hotel_id
    WHERE h.city_id = c.id
    GROUP BY h.id, h.name
    ORDER BY total_rooms DESC
    LIMIT 1
) hotel_data ON true
WHERE hotel_data.hotel_id IS NOT NULL OR c.id IS NOT NULL
ORDER BY c.name;

SELECT 
    t.id AS tourist_id,
    t.name AS tourist_name,
    last_visit.city_id,
    last_visit.city_name,
    last_visit.visited_at
FROM tourist t
LEFT JOIN LATERAL (
    SELECT 
        c.id AS city_id,
        c.name AS city_name,
        tc.visited_at
    FROM tourist_city tc
    JOIN city c ON tc.city_id = c.id
    WHERE tc.tourist_id = t.id
    ORDER BY tc.visited_at DESC
    LIMIT 1
) last_visit ON true
ORDER BY t.name;


SELECT 
    c1.id AS city1_id,
    c1.name AS city1_name,
    c2.id AS city2_id,
    c2.name AS city2_name,
    co.name AS country_name
FROM city c1
INNER JOIN city c2 ON c1.country_id = c2.country_id AND c1.id < c2.id
INNER JOIN country co ON c1.country_id = co.id
ORDER BY co.name, c1.name, c2.name;

SELECT 
    t1.id AS tourist1_id,
    t1.name AS tourist1_name,
    t1.birth_year,
    t2.id AS tourist2_id,
    t2.name AS tourist2_name
FROM tourist t1
INNER JOIN tourist t2 ON t1.birth_year = t2.birth_year AND t1.id < t2.id
WHERE t1.birth_year IS NOT NULL
ORDER BY t1.birth_year, t1.name, t2.name;


SELECT 
    c.id,
    c.name AS city,
    COUNT(h.id) AS hotels_count,
    COALESCE(SUM(hotel_rooms.total_rooms), 0) AS total_rooms_in_city
FROM city c
LEFT JOIN hotel h ON c.id = h.city_id
LEFT JOIN LATERAL (
    SELECT SUM(rooms_available) AS total_rooms
    FROM hotel_room hr
    WHERE hr.hotel_id = h.id
) hotel_rooms ON true
GROUP BY c.id, c.name
ORDER BY hotels_count DESC, total_rooms_in_city DESC;

SELECT 
    t.id,
    t.name,
    COUNT(DISTINCT tc.city_id) AS cities_visited,
    COUNT(b.id) AS bookings_count,
    COALESCE(SUM(b.total_price), 0) AS total_spent
FROM tourist t
LEFT JOIN tourist_city tc ON t.id = tc.tourist_id
LEFT JOIN booking b ON t.id = b.tourist_id
GROUP BY t.id, t.name
ORDER BY total_spent DESC, cities_visited DESC;

SELECT 
    rt.title,
    rt.max_guests,
    COUNT(DISTINCT hr.hotel_id) AS hotels_with_this_type,
    SUM(hr.rooms_available) AS total_rooms_available,
    COUNT(b.id) AS bookings_count
FROM room_type rt
LEFT JOIN hotel_room hr ON rt.id = hr.room_type_id
LEFT JOIN booking b ON rt.id = b.room_type_id
GROUP BY rt.id, rt.title, rt.max_guests
ORDER BY bookings_count DESC, total_rooms_available DESC;