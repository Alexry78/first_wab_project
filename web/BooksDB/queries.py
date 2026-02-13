from books_db import BooksDB

def main():
    db = BooksDB()
    
    print("\n" + "="*70)
    print(" ЗАДАНИЕ: РАБОТА С ТАБЛИЦЕЙ BOOKS")
    print("="*70)
    
    # ---------- Задание a) SELECT ----------
    print("\n" + "-"*70)
    print(" a) SELECT: Книги fantasy, название на 'Dragon', 2010-2020")
    print("-"*70)
    
    fantasy_dragon = db.select_fantasy_dragon_books()
    db.print_results("РЕЗУЛЬТАТЫ SELECT (ожидаем 5 книг)", fantasy_dragon)
    
    # ---------- Задание b) UPDATE ----------
    print("\n" + "-"*70)
    print(" b) UPDATE: Увеличение цены на 15% для Science Fiction")
    print("    (цена 9.99-19.99, название без 'Box Set')")
    print("-"*70)
    
    # Покажем книги до обновления
    before = db.execute_query("""
        SELECT id, title, author, genre, price, published_date
        FROM books
        WHERE LOWER(genre) LIKE '%science fiction%'
            AND price BETWEEN 9.99 AND 19.99
            AND LOWER(title) NOT LIKE '%box set%'
        ORDER BY price
    """)
    
    print("\nКниги ДО обновления:")
    for book in before:
        print(f"  {book['title']:25} | ${book['price']:6.2f} | {book['genre']}")
    
    # Выполняем обновление
    updated = db.update_sci_fi_prices()
    print(f"\nОбновлено книг: {updated}")
    
    # Покажем книги после обновления
    after = db.execute_query("""
        SELECT id, title, author, genre, price, published_date
        FROM books
        WHERE LOWER(genre) LIKE '%science fiction%'
            AND id IN ({})
        ORDER BY price
    """.format(','.join(str(b['id']) for b in before)))
    
    print("\nКниги ПОСЛЕ обновления:")
    for book in after:
        print(f"  {book['title']:25} | ${book['price']:6.2f} | {book['genre']}")
    
    # ---------- Задание c) DELETE ----------
    print("\n" + "-"*70)
    print(" c) DELETE: Удаление книг Reference до 2000 с 'Sample' в названии")
    print("-"*70)
    
    # Покажем книги до удаления
    to_delete = db.execute_query("""
        SELECT id, title, author, genre, price, published_date
        FROM books
        WHERE LOWER(genre) LIKE '%reference%'
            AND published_date < '2000-01-01'
            AND LOWER(title) LIKE '%sample%'
        ORDER BY published_date
    """)
    
    print("\nКниги ДО удаления (будут удалены):")
    for book in to_delete:
        print(f"  {book['title']:30} | {book['published_date']} | {book['genre']}")
    
    # Выполняем удаление
    deleted = db.delete_reference_sample_books()
    print(f"\nУдалено книг: {deleted}")
    
    # Проверим, что они действительно удалены
    after_delete = db.execute_query("""
        SELECT COUNT(*) as count
        FROM books
        WHERE LOWER(genre) LIKE '%reference%'
            AND published_date < '2000-01-01'
            AND LOWER(title) LIKE '%sample%'
    """)
    
    print(f"Осталось таких книг после удаления: {after_delete[0]['count'] if after_delete else 0}")
    
    # ---------- Итог ----------
    print("\n" + "="*70)
    print(" ИТОГОВОЕ СОСТОЯНИЕ БАЗЫ ДАННЫХ")
    print("="*70)
    
    all_books = db.get_all_books()
    
    # Статистика по жанрам
    from collections import Counter
    genres = Counter(book['genre'] for book in all_books)
    
    print(f"\nВсего книг в базе: {len(all_books)}")
    print("\nРаспределение по жанрам:")
    for genre, count in sorted(genres.items()):
        print(f"  {genre:20}: {count}")
    
    print("\nПервые 10 книг:")
    for book in all_books[:10]:
        print(f"  {book['published_date']} | {book['title'][:40]:40} | ${book['price']:6.2f}")

if __name__ == "__main__":
    main()