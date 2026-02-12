class Student:
    def __init__(self, name, group, grade):
        self.name = name
        self.group = group
        self.grade = grade
    
    def __str__(self):
        return f"{self.name} | {self.group} | Балл: {self.grade}"

class Faculty:
    def __init__(self, name, spec, teachers):
        self.name = name
        self.specialization = spec
        self.teachers_count = teachers
        self.students = []
    
    def add_student(self, student):
        self.students.append(student)
    
    def avg_grade(self):
        return sum(s.grade for s in self.students) / len(self.students) if self.students else 0
    
    def __str__(self):
        return f"{self.name} | Студентов: {len(self.students)} | Средний балл: {self.avg_grade():.2f}"

class University:
    def __init__(self, name, address, budget):
        self.name = name
        self.address = address
        self.budget = budget
        self.faculties = []
    
    def add_faculty(self, faculty):
        self.faculties.append(faculty)
    
    def total_students(self):
        return sum(len(f.students) for f in self.faculties)
    
    def __str__(self):
        return f"{self.name}\nАдрес: {self.address}\nБюджет: {self.budget} руб.\nФакультетов: {len(self.faculties)}\nСтудентов: {self.total_students()}"

# ДЕМОНСТРАЦИЯ
if __name__ == "__main__":
    # Университет
    uni = University("МГУ", "Москва", 150_000_000)
    
    # Факультеты
    it = Faculty("ИТ", "Программирование", 45)
    econ = Faculty("Экономика", "Финансы", 38)
    
    # Добавляем факультеты
    uni.add_faculty(it)
    uni.add_faculty(econ)
    
    # Студенты
    it.add_student(Student("Иванов Иван", "ИТ-101", 4.8))
    it.add_student(Student("Петрова Анна", "ИТ-101", 4.5))
    it.add_student(Student("Сидоров Петр", "ИТ-102", 3.9))
    
    econ.add_student(Student("Васильева Елена", "ЭК-201", 4.6))
    econ.add_student(Student("Николаев Дмитрий", "ЭК-201", 4.1))
    
    # Вывод
    print(uni)
    print("\nФакультеты:")
    for f in uni.faculties:
        print(f"  {f}")
        for s in f.students:
            print(f"    • {s}")