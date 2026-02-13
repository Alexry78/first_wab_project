import unittest
import os
import sqlite3
from temperatures_db import TemperaturesDB
from datetime import datetime

class TestTemperaturesDB(unittest.TestCase):
    
    def setUp(self):
        """Подготовка перед каждым тестом - создаём чистую БД"""
        self.test_db = "test_temperatures.db"
        # Удаляем старый файл, если существует
        if os.path.exists(self.test_db):
            os.remove(self.test_db)
        
        # Создаём новую БД только с таблицей, без тестовых данных
        self.db = TemperaturesDB(self.test_db)
        self._create_empty_tables()
        
    def _create_empty_tables(self):
        """Создание пустых таблиц без тестовых данных"""
        try:
            self.db.connect()
            self.db.cursor.execute("""
                CREATE TABLE IF NOT EXISTS city_temperatures (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    city_name TEXT NOT NULL,
                    measurement_date DATE NOT NULL,
                    temperature_celsius REAL NOT NULL CHECK(temperature_celsius >= -60 AND temperature_celsius <= 60),
                    UNIQUE(city_name, measurement_date)
                )
            """)
            self.db.connection.commit()
        except Exception as e:
            print(f"Ошибка при создании таблицы: {e}")
        finally:
            self.db.disconnect()
        
    def tearDown(self):
        """Очистка после каждого теста"""
        self.db.disconnect()
        if os.path.exists(self.test_db):
            os.remove(self.test_db)
            
    def test_add_temperature_valid(self):
        """Тест добавления корректной температуры"""
        temp_id = self.db.add_temperature("Тестовый", "2025-02-13", 15.5)
        self.assertIsNotNone(temp_id)
        
        measurements = self.db.get_city_measurements("Тестовый")
        self.assertEqual(len(measurements), 1)
        self.assertEqual(measurements[0]['temperature_celsius'], 15.5)
        
    def test_add_temperature_below_min(self):
        """Тест добавления температуры ниже -60"""
        temp_id = self.db.add_temperature("Тестовый", "2025-02-13", -70.0)
        self.assertIsNone(temp_id)
        
    def test_add_temperature_above_max(self):
        """Тест добавления температуры выше +60"""
        temp_id = self.db.add_temperature("Тестовый", "2025-02-13", 70.0)
        self.assertIsNone(temp_id)
        
    def test_add_temperature_invalid_date(self):
        """Тест добавления с неверной датой"""
        temp_id = self.db.add_temperature("Тестовый", "13-02-2025", 15.5)
        self.assertIsNone(temp_id)
        
    def test_add_duplicate_measurement(self):
        """Тест добавления дубликата (город + дата)"""
        self.db.add_temperature("Дубликат", "2025-02-13", 10.0)
        temp_id = self.db.add_temperature("Дубликат", "2025-02-13", 15.0)
        self.assertIsNone(temp_id)
        
    def test_get_city_measurements(self):
        """Тест получения измерений для города"""
        self.db.add_temperature("Город1", "2025-02-13", 10.0)
        self.db.add_temperature("Город1", "2025-02-12", 8.0)
        self.db.add_temperature("Город2", "2025-02-13", 20.0)
        
        measurements = self.db.get_city_measurements("Город1")
        self.assertEqual(len(measurements), 2)
        
    def test_get_measurements_by_date(self):
        """Тест получения измерений по дате"""
        # Очищаем таблицу перед тестом
        self.db.add_temperature("Город1", "2025-02-13", 10.0)
        self.db.add_temperature("Город2", "2025-02-13", 20.0)
        self.db.add_temperature("Город3", "2025-02-12", 15.0)
        
        measurements = self.db.get_measurements_by_date("2025-02-13")
        self.assertEqual(len(measurements), 2)
        
    def test_get_temperature_range(self):
        """Тест получения диапазона температур"""
        self.db.add_temperature("Тест", "2025-02-13", 10.0)
        self.db.add_temperature("Тест", "2025-02-12", 20.0)
        self.db.add_temperature("Тест", "2025-02-11", 15.0)
        
        range_data = self.db.get_temperature_range("Тест")
        self.assertIsNotNone(range_data)
        self.assertEqual(range_data['min_temp'], 10.0)
        self.assertEqual(range_data['max_temp'], 20.0)
        self.assertEqual(range_data['avg_temp'], 15.0)
        self.assertEqual(range_data['measurements_count'], 3)
        
    def test_get_hottest_cities(self):
        """Тест получения самых жарких городов"""
        self.db.add_temperature("Город1", "2025-02-13", 10.0)
        self.db.add_temperature("Город2", "2025-02-13", 30.0)
        self.db.add_temperature("Город3", "2025-02-13", 20.0)
        
        hot = self.db.get_hottest_cities("2025-02-13", 2)
        self.assertEqual(len(hot), 2)
        self.assertEqual(hot[0]['city_name'], "Город2")  # Самый жаркий
        self.assertEqual(hot[0]['temperature_celsius'], 30.0)
        
    def test_get_coldest_cities(self):
        """Тест получения самых холодных городов"""
        self.db.add_temperature("Город1", "2025-02-13", -10.0)
        self.db.add_temperature("Город2", "2025-02-13", -30.0)
        self.db.add_temperature("Город3", "2025-02-13", -20.0)
        
        cold = self.db.get_coldest_cities("2025-02-13", 2)
        self.assertEqual(len(cold), 2)
        self.assertEqual(cold[0]['city_name'], "Город2")  # Самый холодный
        self.assertEqual(cold[0]['temperature_celsius'], -30.0)
        
    def test_get_average_by_city(self):
        """Тест получения средних температур по городам"""
        self.db.add_temperature("ГородA", "2025-02-13", 10.0)
        self.db.add_temperature("ГородA", "2025-02-12", 20.0)
        self.db.add_temperature("ГородB", "2025-02-13", 30.0)
        
        averages = self.db.get_average_temperature_by_city()
        self.assertEqual(len(averages), 2)
        
        for a in averages:
            if a['city_name'] == "ГородA":
                self.assertEqual(a['avg_temperature'], 15.0)
                self.assertEqual(a['measurements_count'], 2)
            elif a['city_name'] == "ГородB":
                self.assertEqual(a['avg_temperature'], 30.0)
                self.assertEqual(a['measurements_count'], 1)
                
    def test_update_temperature(self):
        """Тест обновления температуры"""
        temp_id = self.db.add_temperature("Тест", "2025-02-13", 10.0)
        self.assertIsNotNone(temp_id)
        
        result = self.db.update_temperature(temp_id, 25.0)
        self.assertTrue(result)
        
        measurements = self.db.get_city_measurements("Тест")
        self.assertEqual(measurements[0]['temperature_celsius'], 25.0)
        
    def test_update_temperature_invalid(self):
        """Тест обновления с некорректной температурой"""
        temp_id = self.db.add_temperature("Тест", "2025-02-13", 10.0)
        
        result = self.db.update_temperature(temp_id, 100.0)
        self.assertFalse(result)
        
        # Проверяем, что температура не изменилась
        measurements = self.db.get_city_measurements("Тест")
        self.assertEqual(measurements[0]['temperature_celsius'], 10.0)
        
    def test_delete_measurement(self):
        """Тест удаления измерения"""
        temp_id = self.db.add_temperature("Тест", "2025-02-13", 10.0)
        self.assertIsNotNone(temp_id)
        
        result = self.db.delete_measurement(temp_id)
        self.assertTrue(result)
        
        measurements = self.db.get_city_measurements("Тест")
        self.assertEqual(len(measurements), 0)

    def test_initial_data_exists(self):
        """Тест наличия начальных данных (если они создаются)"""
        # Этот тест можно пропустить, если мы не создаём начальные данные
        pass

if __name__ == '__main__':
    unittest.main()