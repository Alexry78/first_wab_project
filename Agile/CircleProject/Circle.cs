using System;

namespace CircleProject
{
    /// <summary>
    /// Класс, представляющий круг
    /// </summary>
    public class Circle
    {
        // Атрибут - радиус круга
        private double _radius;

        /// <summary>
        /// Конструктор класса Circle
        /// </summary>
        /// <param name="radius">Радиус круга</param>
        public Circle(double radius)
        {
            _radius = radius;
        }

        /// <summary>
        /// Свойство для доступа к радиусу
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set 
            { 
                if (value > 0)
                    _radius = value; 
                else
                    throw new ArgumentException("Радиус должен быть положительным числом");
            }
        }

        /// <summary>
        /// Метод для вычисления площади круга
        /// </summary>
        /// <returns>Площадь круга</returns>
        public double CalculateArea()
        {
            return Math.PI * _radius * _radius;
        }

        /// <summary>
        /// Метод для вычисления длины окружности
        /// </summary>
        /// <returns>Длина окружности</returns>
        public double CalculateCircumference()
        {
            return 2 * Math.PI * _radius;
        }

        /// <summary>
        /// Переопределенный метод ToString() для строкового представления круга
        /// </summary>
        /// <returns>Строковое представление круга</returns>
        public override string ToString()
        {
            return $"Круг с радиусом {_radius:F2}, площадью {CalculateArea():F2} и длиной окружности {CalculateCircumference():F2}";
        }
    }
}