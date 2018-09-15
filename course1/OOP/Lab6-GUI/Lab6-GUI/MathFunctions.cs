using System;
using System.Collections.Generic;
using System.Linq;

namespace MathFunctions
{
    // Абстрактный класс математических функций
    public abstract class MathFunction
    {
        public abstract double solveFor(double x);
        public abstract string getInfo();
    }

    /*
    * Описание наследующих классов 
    * График для проверки функций - https://www.desmos.com/calculator/4gsilibcxn
    */

    // Класс функции параболы
    public class Parabola : MathFunction
    {
        private double p;

        // Методы параболы - y^2 = 2px
        // Без ограничения p > 0
        public Parabola(double p)
        {
            this.p = p;
        }
        public override double solveFor(double x)
        {
            return Math.Sqrt(2 * p * x);
        }
        public override string getInfo() {
            return "y^2 = 2*px";
        }
    }

    // Класс функции гиберболы
    public class Hyperbola : MathFunction
    {
        private double a, b;

        // Методы гиперболы - x^2/a^2 - y^2/b^2 = 1
        public Hyperbola(double a, double b)
        {
            this.a = a;
            this.b = b;
        }
        public override double solveFor(double x)
        {
            return Math.Sqrt(((x * x) / (a * a) - 1) * b * b);
        }
        public override string getInfo()
        {
            return "x^2/a^2 - y^2/b^2 = 1";
        }
    }

    // Класс функции эллипса
    public class Ellipse : MathFunction
    {
        private double a, b;

        // Методы эллипса - x^2/a^2 + y^2/b^2 = 1
        // Без ограничения a > b
        public Ellipse(double a, double b)
        {
            this.a = a;
            this.b = b;
        }
        public override double solveFor(double x)
        {
            return Math.Sqrt((1 - (x * x) / (a * a)) * b * b);
        }
        public override string getInfo()
        {
            return "x^2/a^2 + y^2/b^2 = 1";
        }
    }

    /* Класс для хранения результатов выполнения функций */
    public class Series
    {
        private MathFunction func; // Объект функции
        public List<Tuple<double, double>> data;  // Массив результатов

        public Series()
        {
            data = new List<Tuple<double, double>>();
        }

        // Конструктор принимает объект одного из классов математических функций
        public Series(MathFunction func)
        {
            data = new List<Tuple<double, double>>();
            this.func = func;
        }

        // Добавляет y = f(x) в массив
        public Tuple<double, double> store(double x)
        {
            data.Add(new Tuple<double, double>( x, func.solveFor(x) ));
            return data.Last();
        }

        // Убирает все сохраненные результаты
        public void purge()
        {
            data.Clear();
        }

        // Устанавливает функцию и очищает хранилище
        public void setFunction(MathFunction func)
        {
            purge();
            this.func = func;
        }

        // Возвращает текстовую версию текущей функции
        public string getInfo()
        {
            return func.getInfo();
        }

    }
}
