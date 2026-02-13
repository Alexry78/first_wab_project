using System;

namespace RestaurantProject
{
    
    public class SimpleOrder : IOrder
    {
       
        private string _customer = string.Empty;
        private int _totalCents;

        /// <param name="customer">Имя клиента</param>
        /// <param name="totalCents">Сумма заказа в центах</param>
        public SimpleOrder(string customer, int totalCents)
        {
            Customer = customer;
            TotalCents = totalCents;
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

        public void ApplyDiscount(int percent)
        {
            // Вызываем метод расширения
            this.DefaultApplyDiscount(percent);
        }

      
        public override string ToString()
        {
            return $"Заказ для {Customer}: {TotalCents / 100.0:F2} руб. (Простой клиент)";
        }
    }
}