import sqlite3
from datetime import datetime
from typing import List, Tuple, Optional

class ClinicDB:
    """Класс для работы с базой данных клиники"""
    
    def __init__(self, db_path: str = "clinic.db"):
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
            
    def create_tables(self, schema_file: str = "clinic_schema.sql"):
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
            
    # ---------- Работа с пациентами ----------
    
    def add_patient(self, full_name: str, birth_date: str, gender: str) -> Optional[int]:
        """
        Добавление нового пациента
        :param full_name: Полное имя
        :param birth_date: Дата рождения (YYYY-MM-DD)
        :param gender: Пол ('M' или 'F')
        :return: ID добавленного пациента или None при ошибке
        """
        if gender not in ['M', 'F']:
            print("Ошибка: пол должен быть 'M' или 'F'")
            return None
            
        try:
            # Проверка формата даты
            datetime.strptime(birth_date, '%Y-%m-%d')
        except ValueError:
            print("Ошибка: дата должна быть в формате YYYY-MM-DD")
            return None
            
        try:
            self.connect()
            self.cursor.execute("""
                INSERT INTO patients (full_name, birth_date, gender)
                VALUES (?, ?, ?)
            """, (full_name, birth_date, gender))
            self.connection.commit()
            return self.cursor.lastrowid
        except Exception as e:
            print(f"Ошибка при добавлении пациента: {e}")
            return None
        finally:
            self.disconnect()
            
    def get_all_patients(self) -> List[dict]:
        """Получение списка всех пациентов"""
        try:
            self.connect()
            self.cursor.execute("SELECT * FROM patients ORDER BY full_name")
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении списка пациентов: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_patient(self, patient_id: int) -> Optional[dict]:
        """Получение информации о пациенте по ID"""
        try:
            self.connect()
            self.cursor.execute("SELECT * FROM patients WHERE id = ?", (patient_id,))
            row = self.cursor.fetchone()
            return dict(row) if row else None
        except Exception as e:
            print(f"Ошибка при получении пациента: {e}")
            return None
        finally:
            self.disconnect()
            
    def search_patients_by_name(self, name_pattern: str) -> List[dict]:
        """Поиск пациентов по имени"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM patients 
                WHERE full_name LIKE ? 
                ORDER BY full_name
            """, (f'%{name_pattern}%',))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при поиске пациентов: {e}")
            return []
        finally:
            self.disconnect()
            
    def update_patient(self, patient_id: int, **kwargs) -> bool:
        """
        Обновление информации о пациенте
        :param patient_id: ID пациента
        :param kwargs: поля для обновления (full_name, birth_date, gender)
        :return: True при успехе, False при ошибке
        """
        allowed_fields = {'full_name', 'birth_date', 'gender'}
        updates = []
        values = []
        
        for key, value in kwargs.items():
            if key in allowed_fields:
                if key == 'gender' and value not in ['M', 'F']:
                    print("Ошибка: пол должен быть 'M' или 'F'")
                    return False
                if key == 'birth_date':
                    try:
                        datetime.strptime(value, '%Y-%m-%d')
                    except ValueError:
                        print("Ошибка: дата должна быть в формате YYYY-MM-DD")
                        return False
                updates.append(f"{key} = ?")
                values.append(value)
                
        if not updates:
            print("Нет полей для обновления")
            return False
            
        values.append(patient_id)
        
        try:
            self.connect()
            self.cursor.execute(f"""
                UPDATE patients 
                SET {', '.join(updates)}
                WHERE id = ?
            """, values)
            self.connection.commit()
            return self.cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка при обновлении пациента: {e}")
            return False
        finally:
            self.disconnect()
            
    def delete_patient(self, patient_id: int) -> bool:
        """Удаление пациента по ID"""
        try:
            self.connect()
            self.cursor.execute("DELETE FROM patients WHERE id = ?", (patient_id,))
            self.connection.commit()
            return self.cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка при удалении пациента: {e}")
            return False
        finally:
            self.disconnect()
            
    # ---------- Работа с докторами ----------
    
    def add_doctor(self, name: str, specialization: str, phone: str) -> Optional[int]:
        """
        Добавление нового доктора
        :param name: Имя доктора
        :param specialization: Специализация
        :param phone: Телефон (должен быть уникальным)
        :return: ID добавленного доктора или None при ошибке
        """
        try:
            self.connect()
            self.cursor.execute("""
                INSERT INTO doctors (name, specialization, phone)
                VALUES (?, ?, ?)
            """, (name, specialization, phone))
            self.connection.commit()
            return self.cursor.lastrowid
        except sqlite3.IntegrityError:
            print(f"Ошибка: телефон {phone} уже существует")
            return None
        except Exception as e:
            print(f"Ошибка при добавлении доктора: {e}")
            return None
        finally:
            self.disconnect()
            
    def get_all_doctors(self) -> List[dict]:
        """Получение списка всех докторов"""
        try:
            self.connect()
            self.cursor.execute("SELECT * FROM doctors ORDER BY name")
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при получении списка докторов: {e}")
            return []
        finally:
            self.disconnect()
            
    def get_doctor(self, doctor_id: int) -> Optional[dict]:
        """Получение информации о докторе по ID"""
        try:
            self.connect()
            self.cursor.execute("SELECT * FROM doctors WHERE id = ?", (doctor_id,))
            row = self.cursor.fetchone()
            return dict(row) if row else None
        except Exception as e:
            print(f"Ошибка при получении доктора: {e}")
            return None
        finally:
            self.disconnect()
            
    def get_doctors_by_specialization(self, specialization: str) -> List[dict]:
        """Поиск докторов по специализации"""
        try:
            self.connect()
            self.cursor.execute("""
                SELECT * FROM doctors 
                WHERE specialization LIKE ? 
                ORDER BY name
            """, (f'%{specialization}%',))
            return [dict(row) for row in self.cursor.fetchall()]
        except Exception as e:
            print(f"Ошибка при поиске докторов: {e}")
            return []
        finally:
            self.disconnect()
            
    def update_doctor(self, doctor_id: int, **kwargs) -> bool:
        """
        Обновление информации о докторе
        :param doctor_id: ID доктора
        :param kwargs: поля для обновления (name, specialization, phone)
        :return: True при успехе, False при ошибке
        """
        allowed_fields = {'name', 'specialization', 'phone'}
        updates = []
        values = []
        
        for key, value in kwargs.items():
            if key in allowed_fields:
                updates.append(f"{key} = ?")
                values.append(value)
                
        if not updates:
            print("Нет полей для обновления")
            return False
            
        values.append(doctor_id)
        
        try:
            self.connect()
            self.cursor.execute(f"""
                UPDATE doctors 
                SET {', '.join(updates)}
                WHERE id = ?
            """, values)
            self.connection.commit()
            return self.cursor.rowcount > 0
        except sqlite3.IntegrityError:
            print("Ошибка: телефон должен быть уникальным")
            return False
        except Exception as e:
            print(f"Ошибка при обновлении доктора: {e}")
            return False
        finally:
            self.disconnect()
            
    def delete_doctor(self, doctor_id: int) -> bool:
        """Удаление доктора по ID"""
        try:
            self.connect()
            self.cursor.execute("DELETE FROM doctors WHERE id = ?", (doctor_id,))
            self.connection.commit()
            return self.cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка при удалении доктора: {e}")
            return False
        finally:
            self.disconnect()
            
    def get_doctor_by_phone(self, phone: str) -> Optional[dict]:
        """Поиск доктора по телефону"""
        try:
            self.connect()
            self.cursor.execute("SELECT * FROM doctors WHERE phone = ?", (phone,))
            row = self.cursor.fetchone()
            return dict(row) if row else None
        except Exception as e:
            print(f"Ошибка при поиске доктора: {e}")
            return None
        finally:
            self.disconnect()


# Пример использования
if __name__ == "__main__":
    db = ClinicDB()
    
    # Создание таблиц
    db.create_tables()
    
    # Добавление пациентов
    print("\n=== Добавление пациентов ===")
    id1 = db.add_patient("Иванов Иван Иванович", "1990-05-15", "M")
    id2 = db.add_patient("Петрова Анна Сергеевна", "1985-08-22", "F")
    print(f"Добавлены пациенты с ID: {id1}, {id2}")
    
    # Добавление докторов
    print("\n=== Добавление докторов ===")
    doc1 = db.add_doctor("Смирнов А.А.", "Терапевт", "+7-123-456-78-90")
    doc2 = db.add_doctor("Козлова Е.В.", "Кардиолог", "+7-098-765-43-21")
    print(f"Добавлены доктора с ID: {doc1}, {doc2}")
    
    # Попытка добавить доктора с существующим телефоном
    print("\n=== Попытка добавить доктора с существующим телефоном ===")
    doc3 = db.add_doctor("Петров П.П.", "Хирург", "+7-123-456-78-90")
    
    # Просмотр всех пациентов
    print("\n=== Все пациенты ===")
    patients = db.get_all_patients()
    for p in patients:
        print(f"ID: {p['id']}, {p['full_name']}, {p['birth_date']}, {p['gender']}")
    
    # Просмотр всех докторов
    print("\n=== Все доктора ===")
    doctors = db.get_all_doctors()
    for d in doctors:
        print(f"ID: {d['id']}, {d['name']}, {d['specialization']}, {d['phone']}")
    
    # Поиск докторов по специализации
    print("\n=== Поиск кардиологов ===")
    cardiologists = db.get_doctors_by_specialization("Кардиолог")
    for d in cardiologists:
        print(f"{d['name']} - {d['phone']}")