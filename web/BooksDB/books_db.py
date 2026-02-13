import sqlite3
from datetime import datetime
from typing import List, Dict, Optional, Tuple

class BooksDB:
    """Класс для работы с базой данных книг"""
    
    def __init__(self, db_path: str = "books.db"):
        """Инициализация подключения к БД"""
        self.db_path = db_path
        self.connection = None
        self.cursor = None
        
    def connect(self):
        """Установка соединения с БД"""
        self.connection = sqlite3.connect(self.db_path)
        self.connection.row_factory = sqlite3.Row
        self.cursor = self.connection.cursor()
        
    def disconnect(self):
        """Закрытие соединения с БД"""
        if self.connection:
            self.connection.close()
            
    def create_tables(self, schema_file: str = "books_schema.sql"):
        """Создание таблиц из SQL файла"""
        try:
            self.connect()
            with open(schema_file, 'r', encoding='utf-8') as f:
                schema = f.read()
            self.cursor.executescript(schema)
            self.connection.commit()
            print("Таблицы успешно созданы")
        except Exception as e:
            print(f"Ошибка при создании таблиц: {e}")
        finally:
            self.disconnect()
            
    def add_book(self, title: str, author: str, genre: str, price: float, published_date: str) -> Optional[int]:
        """
        Добавление новой книги
        """
        # Валидация
        if not title or not title.strip():
            print("Ошибка: название не может быть пустым")
            return None
            
        if not author or not author.strip():
            print("Ошибка: автор не может быть пустым")
            return None
            
        if not genre or not genre.strip():
            print("Ошибка: жанр не может быть пустым")
            return None
            
        if price < 0 or price > 10000:
            print("Ошибка: цена должна быть от 0 до 10000")
            return None
            
        try:
            pub_date = datetime.strptime(published_date, '%Y-%m-%d')
            if pub_date.year < 1900 or pub_date > datetime.now():
                print("Ошибка: дата должна быть от 1900 года до текущей")
                return None
        except ValueError:
            print("Ошибка: дата должна быть в формате YYYY-MM-DD")
            return None
            
        try:
            self.connect()
            self.cursor.execute("""
                INSERT INTO books (title, author, genre, price, published_date)
                VALUES (?, ?, ?, ?, ?)
            """, (title.strip(), author.strip(), genre.strip(), price, published_date))
            self.connection.commit()
            return self.cursor.lastrowid
        except sqlite3.IntegrityError as e:
            print(f"Ошибка целостности: {e}")
            return None
        except Exception as e:
            print(f"Ошибка при добавлении книги: {e}")
            return None
        finally:
            self.disconnect()
            
    def add_books_bulk(self, books: List[Tuple]) -> int:
        """Массовое добавление книг"""
        count = 0
        for book in books:
            if self.add_book(*book):
                count += 1
        return count
            
    def execute_query(self, query: str, params: tuple = ()) -> List[Dict]:
        """Выполнение произвольного запроса"""
        try:
            self.connect()
            self.cursor.execute(query, params)
            self.connection.commit()
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при выполнении запроса: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_all_books(self) -> List[Dict]:
        """Получение всех книг"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM books 
                ORDER BY published_date DESC, title
            """)
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении книг: {e}")
            return []
        finally:
            self.disconnect()
            
    # ---------- Задание a) SELECT ----------
    
    def select_fantasy_dragon_books(self) -> List[Dict]:
        """
        SELECT: Книги жанра fantasy, название начинается на Dragon,
        опубликованные между 2010-01-01 и 2020-12-31
        """
        query = """
            SELECT id, title, author, genre, price, published_date
            FROM books
            WHERE LOWER(genre) LIKE '%fantasy%'
                AND LOWER(title) LIKE 'dragon%'
                AND published_date BETWEEN '2010-01-01' AND '2020-12-31'
            ORDER BY title
        """
        return self.execute_query(query)
        
    # ---------- Задание b) UPDATE ----------
    
    def update_sci_fi_prices(self) -> int:
        """
        UPDATE: Увеличить цену на 15% для книг жанра Science Fiction
        с ценой от 9.99 до 19.99, название не содержит 'Box Set'
        """
        # Сначала посмотрим, сколько книг будет обновлено
        check_query = """
            SELECT COUNT(*) as count
            FROM books
            WHERE LOWER(genre) LIKE '%science fiction%'
                AND price BETWEEN 9.99 AND 19.99
                AND LOWER(title) NOT LIKE '%box set%'
        """
        
        try:
            self.connect()
            # Проверяем количество
            self.cursor.execute(check_query)
            count = self.cursor.fetchone()[0]
            
            # Выполняем обновление
            update_query = """
                UPDATE books
                SET price = ROUND(price * 1.15, 2)
                WHERE LOWER(genre) LIKE '%science fiction%'
                    AND price BETWEEN 9.99 AND 19.99
                    AND LOWER(title) NOT LIKE '%box set%'
            """
            self.cursor.execute(update_query)
            self.connection.commit()
            
            print(f"Обновлено книг: {count}")
            return count
            
        except Exception as e:
            print(f"Ошибка при обновлении: {e}")
            return 0
        finally:
            self.disconnect()
            
    # ---------- Задание c) DELETE ----------
    
    def delete_reference_sample_books(self) -> int:
        """
        DELETE: Удалить книги жанра Reference, опубликованные до 2000-01-01,
        название содержит 'Sample'
        """
        # Сначала посмотрим, сколько книг будет удалено
        check_query = """
            SELECT COUNT(*) as count
            FROM books
            WHERE LOWER(genre) LIKE '%reference%'
                AND published_date < '2000-01-01'
                AND LOWER(title) LIKE '%sample%'
        """
        
        try:
            self.connect()
            # Проверяем количество
            self.cursor.execute(check_query)
            count = self.cursor.fetchone()[0]
            
            # Выполняем удаление
            delete_query = """
                DELETE FROM books
                WHERE LOWER(genre) LIKE '%reference%'
                    AND published_date < '2000-01-01'
                    AND LOWER(title) LIKE '%sample%'
            """
            self.cursor.execute(delete_query)
            self.connection.commit()
            
            print(f"Удалено книг: {count}")
            return count
            
        except Exception as e:
            print(f"Ошибка при удалении: {e}")
            return 0
        finally:
            self.disconnect()
            
    def print_results(self, title: str, results: List[Dict]):
        """Красивый вывод результатов"""
        print(f"\n{'='*60}")
        print(f" {title}")
        print(f"{'='*60}")
        
        if not results:
            print(" Нет результатов")
            return
            
        for i, book in enumerate(results, 1):
            print(f"{i:2}. {book['title']} | {book['author']} | {book['genre']} | ${book['price']:.2f} | {book['published_date']}")