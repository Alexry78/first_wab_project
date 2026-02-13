import unittest
import os
from clinic_db import ClinicDB

class TestClinicDB(unittest.TestCase):
    
    def setUp(self):
        """Подготовка перед каждым тестом"""
        self.test_db = "test_clinic.db"
        self.db = ClinicDB(self.test_db)
        self.db.create_tables()
        
    def tearDown(self):
        """Очистка после каждого теста"""
        self.db.disconnect()
        if os.path.exists(self.test_db):
            os.remove(self.test_db)
            
    def test_add_patient(self):
        """Тест добавления пациента"""
        patient_id = self.db.add_patient("Тестов Тест", "2000-01-01", "M")
        self.assertIsNotNone(patient_id)
        
        patient = self.db.get_patient(patient_id)
        self.assertIsNotNone(patient)
        self.assertEqual(patient['full_name'], "Тестов Тест")
        self.assertEqual(patient['gender'], "M")
        
    def test_add_patient_invalid_gender(self):
        """Тест добавления пациента с неверным полом"""
        patient_id = self.db.add_patient("Тестов Тест", "2000-01-01", "X")
        self.assertIsNone(patient_id)
        
    def test_add_patient_invalid_date(self):
        """Тест добавления пациента с неверной датой"""
        patient_id = self.db.add_patient("Тестов Тест", "01-01-2000", "M")
        self.assertIsNone(patient_id)
        
    def test_add_doctor(self):
        """Тест добавления доктора"""
        doctor_id = self.db.add_doctor("Доктор Тест", "Терапевт", "+7-111-222-33-44")
        self.assertIsNotNone(doctor_id)
        
        doctor = self.db.get_doctor(doctor_id)
        self.assertIsNotNone(doctor)
        self.assertEqual(doctor['name'], "Доктор Тест")
        
    def test_add_duplicate_phone(self):
        """Тест добавления доктора с существующим телефоном"""
        self.db.add_doctor("Доктор 1", "Терапевт", "+7-111-111-11-11")
        doctor_id = self.db.add_doctor("Доктор 2", "Хирург", "+7-111-111-11-11")
        self.assertIsNone(doctor_id)
        
    def test_search_patients(self):
        """Тест поиска пациентов"""
        self.db.add_patient("Иванов Иван", "1990-01-01", "M")
        self.db.add_patient("Петров Петр", "1991-01-01", "M")
        
        results = self.db.search_patients_by_name("Иван")
        self.assertEqual(len(results), 1)
        self.assertEqual(results[0]['full_name'], "Иванов Иван")
        
    def test_update_patient(self):
        """Тест обновления пациента"""
        patient_id = self.db.add_patient("Старое Имя", "2000-01-01", "M")
        
        result = self.db.update_patient(patient_id, full_name="Новое Имя")
        self.assertTrue(result)
        
        patient = self.db.get_patient(patient_id)
        self.assertEqual(patient['full_name'], "Новое Имя")
        
    def test_delete_patient(self):
        """Тест удаления пациента"""
        patient_id = self.db.add_patient("Тестов Тест", "2000-01-01", "M")
        
        result = self.db.delete_patient(patient_id)
        self.assertTrue(result)
        
        patient = self.db.get_patient(patient_id)
        self.assertIsNone(patient)
        
    def test_get_doctors_by_specialization(self):
        """Тест поиска докторов по специализации"""
        self.db.add_doctor("Кардиолог 1", "Кардиолог", "+7-111-111-11-11")
        self.db.add_doctor("Кардиолог 2", "Кардиолог", "+7-222-222-22-22")
        self.db.add_doctor("Терапевт", "Терапевт", "+7-333-333-33-33")
        
        cardiologists = self.db.get_doctors_by_specialization("Кардиолог")
        self.assertEqual(len(cardiologists), 2)
        
    def test_get_doctor_by_phone(self):
        """Тест поиска доктора по телефону"""
        self.db.add_doctor("Доктор Тест", "Терапевт", "+7-999-999-99-99")
        
        doctor = self.db.get_doctor_by_phone("+7-999-999-99-99")
        self.assertIsNotNone(doctor)
        self.assertEqual(doctor['name'], "Доктор Тест")

if __name__ == '__main__':
    unittest.main()