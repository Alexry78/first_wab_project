class Character:
    def __init__(self, name, hp):
        self.character_name = name
        self.health_points = hp

class Warrior(Character):
    def __init__(self, name, hp, strength):
        super().__init__(name, hp)
        self.strength = strength
    
    def attack_description(self):
        return f"{self.character_name} атакует противника силой удара {self.strength} единиц."

class Wizard(Character):
    def __init__(self, name, hp, magic_power):
        super().__init__(name, hp)
        self.magic_power = magic_power
    
    def attack_description(self):
        return f"{self.character_name} накладывает заклинание с мощностью магии {self.magic_power} единиц."

# Тест
if __name__ == "__main__":
    warrior = Warrior("Артас", 100, 75)
    wizard = Wizard("Джинна", 80, 95)
    
    print(warrior.attack_description())
    print(wizard.attack_description())
