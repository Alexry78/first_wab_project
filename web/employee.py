class Employee:
    def __init__(self, position, salary, experience):
        self.position = position
        self.salary = salary
        self.experience = experience
    
    def promote(self, years=1):
        self.experience += years
        self.salary = round(self.salary * (1 + years * 0.1), 2)
    
    def __str__(self):
        return f"{self.position} | Зарплата: {self.salary} руб. | Стаж: {self.experience} лет"


# ЭТО ЕДИНСТВЕННОЕ ПРАВИЛЬНОЕ НАПИСАНИЕ
if __name__ == "__main__":
    emp = Employee("Python Developer", 100000, 2)
    print("Было:", emp)
    
    emp.promote()
    print("Через 1 год:", emp)
    
    emp.promote(3)
    print("Через 3 года:", emp)
