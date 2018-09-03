using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5_CSharp
{
    // Абстрактный класс математических функций
    abstract class MathFunction
    {
        public abstract double solveFor(double x);
    }

    /*
    * Описание наследующих классов 
    * График для проверки функций - https://www.desmos.com/calculator/4gsilibcxn
    */

    // Класс функции параболы
    class Parabola : MathFunction
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
    }

    // Класс функции гиберболы
    class Hyperbola : MathFunction
    {
        private double a, b;
        
        // Методы гиперболы - x^2/a^2 - y^2/b^2 = 1
        public Hyperbola(double a, double b) {
            this.a = a;
            this.b = b;
        }
        public override double solveFor(double x)
            {
                return Math.Sqrt(((x * x) / (a * a) - 1) * b * b);
            }
    }

    // Класс функции эллипса
    class Ellipse : MathFunction
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
    }

    /* Класс для хранения результатов выполнения функций */
    class Series
    {
        private MathFunction func; // Объект функции
        public List<double> data;  // Массив результатов

        // Конструктор принимает объект одного из классов математических функций
        public Series(MathFunction func)
        {
            data = new List<double>();
            this.func = func;
        }

        // Добавляет y = f(x) в массив
        public void store(double x)
        {
            data.Add(func.solveFor(x));
        }

        // Убирает все сохраненные результаты
        public void purge()
        {
            data.Clear();
        }

        // Выводит все результаты
        public void log()
        {
            if(data.Count() == 0)
            {
                Console.WriteLine("Empty");
                return;
            }
            data.ForEach(y => Console.WriteLine(y));
        }

    }

    /* Тестовые вызовы */
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ellipse:");
            var e = new Series(new Ellipse(-3, 2.5));
            e.store(2);
            e.store(10);
            e.store(-1);
            e.store(-3);
            e.log();
            e.purge();
            e.log();

            Console.WriteLine("Parabola:");
            var p = new Series(new Parabola(1));
            p.store(2);
            p.store(10);
            p.store(-1);
            p.store(-3);
            p.log();

            Console.WriteLine("Hyperbola:");
            var h = new Series(new Hyperbola(2, 1));
            h.store(2);
            h.store(10);
            h.store(-1);
            h.store(3);
            h.log();
        }
    }
}
