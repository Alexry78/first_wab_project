from books_db import BooksDB
import random
from datetime import datetime, timedelta

def generate_test_data():
    """Генерация тестовых данных для таблицы books"""
    
    books_data = [
        # ---------- Книги для SELECT (fantasy + Dragon + 2010-2020) ----------
        ("Dragon and the Fantasy World", "J.R.R. Tolkien", "Fantasy", 15.99, "2015-06-15"),
        ("Dragon's Fantasy Quest", "Christopher Paolini", "Fantasy", 18.50, "2018-03-22"),
        ("Dragon Heart: Fantasy Edition", "Robin Hobb", "Fantasy", 22.00, "2012-11-10"),
        ("Dragon Fantasy Chronicles", "Anne McCaffrey", "Fantasy", 14.95, "2019-08-30"),
        ("Dragon and Fantasy Tales", "Ursula K. Le Guin", "Fantasy", 16.75, "2014-04-18"),
        
        # Контрпримеры для SELECT (подходят частично)
        ("Dragon Science Book", "Carl Sagan", "Science", 25.00, "2016-07-20"),  # не fantasy
        ("The Last Fantasy", "Terry Brooks", "Fantasy", 17.99, "2018-09-12"),   # не начинается на Dragon
        ("Dragon Fantasy", "George R.R. Martin", "Fantasy", 19.99, "2008-05-05"), # дата до 2010
        
        # ---------- Книги для UPDATE (Science Fiction, цена 9.99-19.99, без Box Set) ----------
        ("Foundation", "Isaac Asimov", "Science Fiction", 12.99, "1951-06-01"),
        ("Dune", "Frank Herbert", "Science Fiction", 15.50, "1965-08-01"),
        ("Neuromancer", "William Gibson", "Science Fiction", 14.95, "1984-07-01"),
        ("Snow Crash", "Neal Stephenson", "Science Fiction", 16.99, "1992-06-01"),
        ("The Martian", "Andy Weir", "Science Fiction", 11.99, "2011-09-27"),
        ("Ender's Game", "Orson Scott Card", "Science Fiction", 13.50, "1985-01-15"),
        
        # Контрпримеры для UPDATE
        ("Dune Box Set", "Frank Herbert", "Science Fiction", 29.99, "2020-08-01"),  # цена > 19.99
        ("Foundation Box Set", "Isaac Asimov", "Science Fiction", 8.99, "2021-06-01"), # цена < 9.99
        ("1984", "George Orwell", "Science Fiction", 12.99, "1949-06-08"),  # подходит по цене, но жанр? (это антиутопия)
        ("The Box Set Collection", "Various", "Reference", 45.00, "2019-01-01"),  # другой жанр
        
        # ---------- Книги для DELETE (Reference + до 2000 + Sample в названии) ----------
        ("Sample Reference Guide", "John Doe", "Reference", 35.00, "1995-03-10"),
        ("Scientific Sample Reference", "Jane Smith", "Reference", 42.50, "1988-11-22"),
        ("Reference Manual Sample", "Bob Johnson", "Reference", 28.75, "1999-12-31"),
        ("Sample Reference Book", "Alice Brown", "Reference", 39.99, "1975-07-15"),
        ("The Complete Reference Sample", "Charlie Wilson", "Reference", 55.00, "1998-09-05"),
        
        # Контрпримеры для DELETE
        ("Modern Reference 2020", "Eve Adams", "Reference", 49.99, "2020-01-01"),  # дата после 2000
        ("Sample Fiction Book", "Frank Miller", "Fiction", 14.99, "1990-04-20"),   # не Reference
        ("Reference Handbook", "Grace Lee", "Reference", 32.00, "1995-08-15"),     # нет Sample в названии
        
        # ---------- Дополнительные книги разных жанров ----------
        ("The Mystery of the Lost Key", "Agatha Christie", "Mystery", 9.99, "1934-01-01"),
        ("Pride and Prejudice", "Jane Austen", "Romance", 7.99, "1813-01-28"),
        ("The Shining", "Stephen King", "Horror", 11.99, "1977-01-28"),
        ("The Da Vinci Code", "Dan Brown", "Thriller", 10.99, "2003-03-18"),
        ("Sapiens", "Yuval Noah Harari", "Non-fiction", 16.99, "2011-01-01"),
        ("Clean Code", "Robert C. Martin", "Programming", 42.99, "2008-08-01"),
        ("The Pragmatic Programmer", "David Thomas", "Programming", 39.99, "1999-10-20"),
        ("Introduction to Algorithms", "Thomas H. Cormen", "Reference", 89.99, "2009-07-31"),
        ("The Art of Computer Programming", "Donald Knuth", "Reference", 120.00, "1968-01-01"),
        ("Dragon Programming Guide", "James Gosling", "Programming", 54.99, "2015-04-15"),
    ]
    
    return books_data

def main():
    db = BooksDB()
    
    # Создание таблиц
    print("Создание таблиц...")
    db.create_tables()
    
    # Заполнение данными
    print("Заполнение тестовыми данными...")
    books = generate_test_data()
    count = db.add_books_bulk(books)
    print(f"Добавлено книг: {count}")
    
    # Проверка
    all_books = db.get_all_books()
    print(f"\nВсего книг в базе: {len(all_books)}")
    
    # Вывод нескольких примеров
    print("\nПервые 5 книг:")
    for book in all_books[:5]:
        print(f"  {book['title']} | {book['author']} | {book['genre']} | ${book['price']:.2f}")

if __name__ == "__main__":
    main()