import sqlite3
from datetime import datetime
from typing import List, Dict, Optional, Tuple

class TemperaturesDB:
    """Класс для работы с базой данных температур"""
    
    def __init__(self, db_path: str = "temperatures.db"):
        """Инициализация подключения к БД"""
        self.db_path = db_path
        self.connection = None
        self.cursor = None
        
    def connect(self):
        """Установка соединения с БД"""
        self.connection = sqlite3.connect(self.db_path)
        self.connection.row_factory = sqlite3.Row  # Для доступа по именам колонок
        self.cursor = self.connection.cursor()
        
    def disconnect(self):
        """Закрытие соединения с БД"""
        if self.connection:
            self.connection.close()
            
    def create_tables(self, schema_file: str = "temperatures_schema.sql"):
        """Создание таблиц из SQL файла и добавление начальных данных"""
        try:
            self.connect()
            with open(schema_file, 'r', encoding='utf-8') as f:
                schema = f.read()
            self.cursor.executescript(schema)
            self.connection.commit()
            print("Таблицы успешно созданы и заполнены тестовыми данными")
        except Exception as e:
            print(f"Ошибка при создании таблиц: {e}")
        finally:
            self.disconnect()
            
    def add_temperature(self, city_name: str, date: str, temperature: float) -> Optional[int]:
        """
        Добавление измерения температуры
        :param city_name: Название города
        :param date: Дата измерения (YYYY-MM-DD)
        :param temperature: Температура в градусах Цельсия (-60 до +60)
        :return: ID добавленной записи или None при ошибке
        """
        # Проверка температуры
        if temperature < -60 or temperature > 60:
            print(f"Ошибка: температура {temperature} должна быть от -60 до +60")
            return None
            
        # Проверка формата даты
        try:
            datetime.strptime(date, '%Y-%m-%d')
        except ValueError:
            print("Ошибка: дата должна быть в формате YYYY-MM-DD")
            return None
            
        try:
            self.connect()
            self.cursor.execute("""
                INSERT INTO city_temperatures (city_name, measurement_date, temperature_celsius)
                VALUES (?, ?, ?)
            """, (city_name, date, temperature))
            self.connection.commit()
            return self.cursor.lastrowid
        except sqlite3.IntegrityError:
            print(f"Ошибка: для города {city_name} на дату {date} уже есть измерение")
            return None
        except Exception as e:
            print(f"Ошибка при добавлении измерения: {e}")
            return None
        finally:
            self.disconnect()
            
    def get_all_measurements(self) -> List[Dict]:
        """Получение всех измерений"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM city_temperatures 
                ORDER BY measurement_date DESC, city_name
            """)
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении измерений: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_city_measurements(self, city_name: str) -> List[Dict]:
        """Получение всех измерений для конкретного города"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM city_temperatures 
                WHERE city_name = ? 
                ORDER BY measurement_date DESC
            """, (city_name,))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении измерений для города {city_name}: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_measurements_by_date(self, date: str) -> List[Dict]:
        """Получение измерений за конкретную дату"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM city_temperatures 
                WHERE measurement_date = ? 
                ORDER BY temperature_celsius DESC
            """, (date,))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении измерений за дату {date}: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_temperature_range(self, city_name: str) -> Optional[Dict]:
        """Получение минимальной и максимальной температуры для города"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT 
                    MIN(temperature_celsius) as min_temp,
                    MAX(temperature_celsius) as max_temp,
                    AVG(temperature_celsius) as avg_temp,
                    COUNT(*) as measurements_count
                FROM city_temperatures 
                WHERE city_name = ?
            """, (city_name,))
            row = self.cursor.fetchone()
            return dict(row) if row else None
        except Exception as e:
            print(f"Ошибка при получении диапазона температур: {e}")
            return None
        finally:
            self.disconnect()
            
    def get_hottest_cities(self, date: str, limit: int = 5) -> List[Dict]:
        """Получение самых жарких городов за дату"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT city_name, temperature_celsius 
                FROM city_temperatures 
                WHERE measurement_date = ? 
                ORDER BY temperature_celsius DESC 
                LIMIT ?
            """, (date, limit))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении самых жарких городов: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_coldest_cities(self, date: str, limit: int = 5) -> List[Dict]:
        """Получение самых холодных городов за дату"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT city_name, temperature_celsius 
                FROM city_temperatures 
                WHERE measurement_date = ? 
                ORDER BY temperature_celsius ASC 
                LIMIT ?
            """, (date, limit))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении самых холодных городов: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_average_temperature_by_city(self) -> List[Dict]:
        """Получение средней температуры по каждому городу"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT 
                    city_name,
                    ROUND(AVG(temperature_celsius), 2) as avg_temperature,
                    MIN(measurement_date) as first_measurement,
                    MAX(measurement_date) as last_measurement,
                    COUNT(*) as measurements_count
                FROM city_temperatures 
                GROUP BY city_name
                ORDER BY avg_temperature DESC
            """)
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении средних температур: {e}")
            return []
        finally:
            self.disconnect()
            
    def delete_measurement(self, measurement_id: int) -> bool:
        """Удаление измерения по ID"""
        try:
            self.connect()
            self.cursor.execute("DELETE FROM city_temperatures WHERE id = ?", (measurement_id,))
            self.connection.commit()
            return self.cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка при удалении измерения: {e}")
            return False
        finally:
            self.disconnect()
            
    def update_temperature(self, measurement_id: int, new_temperature: float) -> bool:
        """Обновление температуры в измерении"""
        if new_temperature < -60 or new_temperature > 60:
            print(f"Ошибка: температура {new_temperature} должна быть от -60 до +60")
            return False
            
        try:
            self.connect()
            self.cursor.execute("""
                UPDATE city_temperatures 
                SET temperature_celsius = ? 
                WHERE id = ?
            """, (new_temperature, measurement_id))
            self.connection.commit()
            return self.cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка при обновлении температуры: {e}")
            return False
        finally:
            self.disconnect()


# Пример использования
if __name__ == "__main__":
    db = TemperaturesDB()
    
    # Создание таблиц и добавление начальных данных
    print("=== ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ ===")
    db.create_tables()
    
    # Просмотр всех измерений
    print("\n=== ВСЕ ИЗМЕРЕНИЯ ===")
    measurements = db.get_all_measurements()
    for m in measurements:
        print(f"{m['measurement_date']} - {m['city_name']}: {m['temperature_celsius']}°C")
    
    # Просмотр измерений для конкретного города
    print("\n=== ИЗМЕРЕНИЯ ДЛЯ МОСКВЫ ===")
    moscow = db.get_city_measurements("Москва")
    for m in moscow:
        print(f"{m['measurement_date']}: {m['temperature_celsius']}°C")
    
    # Самая высокая и низкая температура
    print("\n=== ДИАПАЗОН ТЕМПЕРАТУР ДЛЯ МОСКВЫ ===")
    range_data = db.get_temperature_range("Москва")
    if range_data:
        print(f"Мин: {range_data['min_temp']}°C")
        print(f"Макс: {range_data['max_temp']}°C")
        print(f"Средняя: {range_data['avg_temp']:.1f}°C")
        print(f"Измерений: {range_data['measurements_count']}")
    
    # Самые жаркие города
    print("\n=== САМЫЕ ЖАРКИЕ ГОРОДА 2025-02-13 ===")
    hot = db.get_hottest_cities("2025-02-13")
    for h in hot:
        print(f"{h['city_name']}: {h['temperature_celsius']}°C")
    
    # Самые холодные города
    print("\n=== САМЫЕ ХОЛОДНЫЕ ГОРОДА 2025-02-13 ===")
    cold = db.get_coldest_cities("2025-02-13")
    for c in cold:
        print(f"{c['city_name']}: {c['temperature_celsius']}°C")
    
    # Средние температуры по городам
    print("\n=== СРЕДНИЕ ТЕМПЕРАТУРЫ ПО ГОРОДАМ ===")
    averages = db.get_average_temperature_by_city()
    for a in averages:
        print(f"{a['city_name']}: {a['avg_temperature']}°C (измерений: {a['measurements_count']})")
    
    # Добавление нового измерения
    print("\n=== ДОБАВЛЕНИЕ НОВОГО ИЗМЕРЕНИЯ ===")
    new_id = db.add_temperature("Екатеринбург", "2025-02-13", -15.5)
    if new_id:
        print(f"Добавлено измерение с ID: {new_id}")
    
    # Попытка добавить измерение с температурой вне диапазона
    print("\n=== ПОПЫТКА ДОБАВИТЬ НЕКОРРЕКТНУЮ ТЕМПЕРАТУРУ ===")
    db.add_temperature("Тестовый", "2025-02-13", 100.0)
    
    # Попытка добавить дубликат (город + дата)
    print("\n=== ПОПЫТКА ДОБАВИТЬ ДУБЛИКАТ ===")
    db.add_temperature("Москва", "2025-02-13", -5.5)
    
    # Обновление температуры
    print("\n=== ОБНОВЛЕНИЕ ТЕМПЕРАТУРЫ ===")
    if measurements:
        first_id = measurements[0]['id']
        if db.update_temperature(first_id, -10.0):
            print(f"Температура для ID {first_id} обновлена")
    
    # Проверка после обновления
    print("\n=== ВСЕ ИЗМЕРЕНИЯ ПОСЛЕ ОБНОВЛЕНИЯ ===")
    measurements = db.get_all_measurements()
    for m in measurements[:5]:  # Покажем только первые 5
        print(f"{m['measurement_date']} - {m['city_name']}: {m['temperature_celsius']}°C")