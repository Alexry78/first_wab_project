using System;

namespace RestaurantProject
{
    public class MemberOrder : IOrder, ILoyalty
    {

        private string _customer = string.Empty;
        private int _totalCents;
        private int _loyaltyPoints;
        private int _minBillCents;

        /// <param name="customer">Имя клиента</param>
        /// <param name="totalCents">Сумма заказа в центах</param>
        /// <param name="minBillCents">Минимальная сумма чека после скидки</param>
        public MemberOrder(string customer, int totalCents, int minBillCents)
        {
            Customer = customer;
            TotalCents = totalCents;
            MinBillCents = minBillCents;
            LoyaltyPoints = 0; // Начальное количество баллов
        }

        public string Customer
        {
            get { return _customer; }
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя клиента не может быть пустым");
                _customer = value; 
            }
        }

        public int TotalCents
        {
            get { return _totalCents; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Сумма заказа не может быть отрицательной");
                _totalCents = value; 
            }
        }

        public int LoyaltyPoints
        {
            get { return _loyaltyPoints; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Бонусные баллы не могут быть отрицательными");
                _loyaltyPoints = value; 
            }
        }

        public int MinBillCents
        {
            get { return _minBillCents; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentException("Минимальная сумма чека не может быть отрицательной");
                _minBillCents = value; 
            }
        }

        /// <param name="percent">Процент скидки</param>
        public void ApplyDiscount(int percent)
        {
            if (percent < 0 || percent > 100)
            {
                Console.WriteLine("Ошибка: процент скидки должен быть от 0 до 100");
                return;
            }

            int discountAmount = TotalCents * percent / 100;
            int newTotal = TotalCents - discountAmount;

            if (newTotal < MinBillCents)
            {
                int oldDiscountAmount = discountAmount;
                discountAmount = TotalCents - MinBillCents;
                newTotal = MinBillCents;
                Console.WriteLine($"Скидка {percent}% слишком велика. Сумма ограничена минимальным чеком {MinBillCents / 100.0:F2} руб.");
            }

            TotalCents = newTotal;
            Console.WriteLine($"Применена скидка {percent}%. Новая сумма: {TotalCents / 100.0:F2} руб.");

            int bonusPoints = discountAmount / 10;
            if (bonusPoints > 0)
            {
                AddPoints(bonusPoints);
                Console.WriteLine($"Начислено {bonusPoints} бонусных баллов за использование скидки!");
            }
        }

        /// <param name="amount">Количество добавляемых баллов</param>
        public void AddPoints(int amount)
        {
            if (amount < 0)
            {
                Console.WriteLine("Ошибка: количество баллов для добавления не может быть отрицательным");
                return;
            }

            LoyaltyPoints += amount;
            Console.WriteLine($"Добавлено {amount} бонусных баллов. Всего баллов: {LoyaltyPoints}");
        }

        /// <param name="points">Количество баллов для списания</param>
        /// <returns>Успешность операции</returns>
        public bool PayWithPoints(int points)
        {
            if (points <= 0)
            {
                Console.WriteLine("Ошибка: количество баллов должно быть положительным");
                return false;
            }

            if (points > LoyaltyPoints)
            {
                Console.WriteLine($"Недостаточно баллов. Доступно: {LoyaltyPoints}, запрошено: {points}");
                return false;
            }


            int rublesFromPoints = points;
            int centsFromPoints = rublesFromPoints * 100;

            if (centsFromPoints > TotalCents)
            {
                Console.WriteLine($"Сумма баллов ({points}) превышает стоимость заказа ({TotalCents / 100.0:F2} руб.)");
                return false;
            }

            LoyaltyPoints -= points;
            TotalCents -= centsFromPoints;

            Console.WriteLine($"Оплачено {points} баллами. Остаток по заказу: {TotalCents / 100.0:F2} руб.");
            Console.WriteLine($"Остаток баллов: {LoyaltyPoints}");
            return true;
        }


        public override string ToString()
        {
            return $"Заказ для {Customer}: {TotalCents / 100.0:F2} руб. (Участник, баллы: {LoyaltyPoints}, мин. чек: {MinBillCents / 100.0:F2} руб.)";
        }
    }
}