DROP TABLE IF EXISTS orders_import_lines;
CREATE TABLE orders_import_lines (
    id serial PRIMARY KEY,
    source_file text NOT NULL,
    line_no int NOT NULL,
    raw_line text NOT NULL,
    imported_at timestamptz default now(),
    note text
);

INSERT INTO orders_import_lines (source_file, line_no, raw_line, note) VALUES
('marketplace_A_2025_11.csv', 1, 'Order#1001; Customer: Olga Petrova <olga.petrova@example.com>; +7 (921) 555-12-34; Items: SKU:AB-123-XY x1', 'order row'),
('marketplace_A_2025_11.csv', 2, 'Order#1002; Customer: Ivan <ivan@@example..com>; 8-921-5551234; Items: SKU:zx9999 x2', 'order row'),
('newsletter_upload.csv', 10, 'john.doe@domain.com; +44 7700 900123; tags: promo, holiday', 'marketing upload'),

('pricing_feed.csv', 3, 'product: ZX-11; price: "1,299.99" USD', 'price row'),
('pricing_feed.csv', 4, 'product: Y-200; price: "2 500,00" EUR', 'price row'),

('catalog_tags.csv', 1, 'tags: electronics, mobile,  accessories', 'tags row'),
('catalog_tags.csv', 2, 'tags: home,kitchen', 'tags row'),

('orders_dirty.csv', 5, '"Smith, John","12 Baker St, Apt 4","1,200.00","SKU: AB-123-XY"', 'dirty csv'),

('processor_log.txt', 100, 'INFO: Processing order 1001', 'log'),
('processor_log.txt', 101, 'warning: price parse failed for line 4', 'log'),
('processor_log.txt', 102, 'Error: invalid phone for order 1002', 'log'),
('processor_log.txt', 103, 'error: missing sku in items list', 'log'),

('marketplace_A_2025_11.csv', 20, 'Customer: bad@-domain.com; +7 921 ABC-12-34; Items: SKU: 12-AB-!!', 'trap-invalid-email-phone-sku'),
('orders_dirty.csv', 6, '"O''Connor, Liam","New York, NY","500"', 'dirty csv with apostrophe');

SELECT 
    id,
    source_file,
    line_no,
    raw_line
FROM orders_import_lines
WHERE raw_line ~* 
    '[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}'
ORDER BY id;


SELECT 
    id,
    source_file,
    line_no,
    raw_line
FROM orders_import_lines
WHERE raw_line !~* 
    '[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}'
ORDER BY id;

SELECT 
    id,
    source_file,
    line_no,
    -- Извлекаем первый email
    (regexp_match(
        raw_line,
        '([A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,})'
    ))[1] AS email
FROM orders_import_lines
WHERE raw_line ~* 
    '[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}'
ORDER BY id;

SELECT 
    id,
    source_file,
    line_no,
    unnest(regexp_matches(
        raw_line,
        '[A-Za-z]{1,3}-[0-9]{2,3}-[A-Za-z]{2}|[A-Za-z]{1,3}-[0-9]{2,3}|[a-z]{2}[0-9]{4}',
        'g'
    )) AS sku
FROM orders_import_lines
WHERE raw_line ~ '[A-Za-z]{1,3}-[0-9]{2,3}-[A-Za-z]{2}|[A-Za-z]{1,3}-[0-9]{2,3}|[a-z]{2}[0-9]{4}'
ORDER BY id, sku;


SELECT 
    id,
    source_file,
    line_no,
    raw_line,
    (regexp_match(raw_line, '[\+\d\(\)\-\s]{7,}'))[1] AS raw_phone,
    regexp_replace(
        COALESCE((regexp_match(raw_line, '[\+\d\(\)\-\s]{7,}'))[1], ''),
        '[^0-9]',
        '',
        'g'
    ) AS normalized_phone
FROM orders_import_lines
WHERE raw_line ~ '[\+\d\(\)\-\s]{7,}'
ORDER BY id;

SELECT 
    id,
    source_file,
    line_no,
    raw_line,
    (regexp_match(raw_line, '"([0-9\s\,\.]+)"'))[1] AS raw_price,

    CASE 
        WHEN (regexp_match(raw_line, '"([0-9\s\,\.]+)"'))[1] IS NOT NULL THEN
            REPLACE(
                REPLACE(
                    (regexp_match(raw_line, '"([0-9\s\,\.]+)"'))[1],
                    ' ',
                    ''
                ),
                ',',
                '.'
            )::NUMERIC(10,2)
        ELSE NULL
    END AS normalized_price
FROM orders_import_lines
WHERE raw_line ~ '"([0-9\s\,\.]+)"'
ORDER BY id;

SELECT 
    id,
    source_file,
    line_no,
    raw_line,
    trim(substring(raw_line from 'tags:(.*)')) AS tags_string,
    array_remove(
        array(
            SELECT trim(unnest)
            FROM unnest(
                regexp_split_to_array(
                    trim(substring(raw_line from 'tags:(.*)')),
                    ','
                )
            )
            WHERE trim(unnest) != ''
        ),
        NULL
    ) AS tags_array
FROM orders_import_lines
WHERE raw_line ~ 'tags:'
ORDER BY id;

CREATE OR REPLACE FUNCTION parse_csv_line(line text)
RETURNS text[] AS $$
DECLARE
    result text[] := '{}';
    field text := '';
    in_quotes boolean := false;
    c char;
    i int;
BEGIN
    FOR i IN 1..length(line) LOOP
        c := substr(line, i, 1);
        
        IF c = '"' THEN
            in_quotes := NOT in_quotes;
        ELSIF c = ',' AND NOT in_quotes THEN
            result := array_append(result, field);
            field := '';
        ELSE
            field := field || c;
        END IF;
    END LOOP;

    result := array_append(result, field);
    
    RETURN result;
END;
$$ LANGUAGE plpgsql;

WITH csv_lines AS (
    SELECT 
        id,
        source_file,
        line_no,
        raw_line
    FROM orders_import_lines
    WHERE source_file = 'orders_dirty.csv'
)
SELECT 
    id,
    source_file,
    line_no,
    parse_csv_line(raw_line) AS parsed_fields,
    (parse_csv_line(raw_line))[1] AS field_1,
    (parse_csv_line(raw_line))[2] AS field_2,
    (parse_csv_line(raw_line))[3] AS field_3,
    (parse_csv_line(raw_line))[4] AS field_4
FROM csv_lines
ORDER BY id;


SELECT 
    id,
    source_file,
    line_no,
    unnest(regexp_split_to_array(raw_line, ',(?=(?:[^"]*"[^"]*")*[^"]*$)')) AS field
FROM orders_import_lines
WHERE source_file = 'orders_dirty.csv'
ORDER BY id;


SELECT 
    id,
    source_file,
    line_no,
    raw_line
FROM orders_import_lines
WHERE source_file = 'processor_log.txt'
    AND raw_line ~* 'error'
ORDER BY id;

SELECT 
    id,
    source_file,
    line_no,
    raw_line AS original_line,
    regexp_replace(raw_line, 'error', 'ERROR', 'gi') AS modified_line
FROM orders_import_lines
WHERE source_file = 'processor_log.txt'
ORDER BY id;

SELECT 
    source_file,
    COUNT(*) AS total_lines,
    COUNT(CASE WHEN raw_line ~* '[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}' THEN 1 END) AS with_emails,
    COUNT(CASE WHEN raw_line ~ '[\+\d\(\)\-\s]{7,}' THEN 1 END) AS with_phones,
    COUNT(CASE WHEN raw_line ~ 'price:' THEN 1 END) AS with_prices,
    COUNT(CASE WHEN raw_line ~ 'tags:' THEN 1 END) AS with_tags
FROM orders_import_lines
GROUP BY source_file
ORDER BY source_file;

DROP FUNCTION IF EXISTS parse_csv_line(text);