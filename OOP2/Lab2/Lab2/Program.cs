using System;
using System.Collections.Generic;

namespace Lab2 {

    class Program {
        static void Main(string[] args) {

            // Создаем полицейский участок
            var pd = new PoliceDepartment();

            // Нанимаем копов
            pd.HireCop("John Doughnut", 1);
            pd.HireCop("Mike Copson", 5);
            pd.HireCop("Edward Gunman", 4);
            pd.HireCop("Shaggy Rogers", 1);
            pd.HireCop("Richard Blackman", 2);
            pd.HireCop("Dale Cooper", 9);

            // Выводим начальный отчет
            pd.PrintReport();

            while (true) {
                // Решаем дела по энтеру
                while (Console.ReadKey().Key == ConsoleKey.Enter) pd.SolveCase();

                // Выводим отчет по любой другой клавише
                pd.PrintReport();

                // Выходим по повторному нажатию любой другой клавиши
                if (Console.ReadKey().Key != ConsoleKey.Enter) break;
            }
        }
    }

    // Класс полицейского участка
    public class PoliceDepartment {

        // Генератор случайных чисел для нахождения случайного копа
        private Random rnd = new Random();

        // Список копов
        private List<Cop> cops = new List<Cop> { };

        // Кол-во решенных дел
        private int solvedCases = 0;

        // Кол-во нерешенных дел
        private int unsolvedCases = 0;

        // Добавляет нового копа с указанным именем и рангом в список
        public void HireCop(string name, int rank) {
            var cop = new Cop(name, rank);
            cops.Add(cop);
            SortCops();
        }

        // Возвращает случайного копа с наименьшим рангом не меньшим указанного
        private Cop GetCopWithAtLeast(int rank) {

            // Берем копа с рангом, не меньшим указанного
            var matchedCop = cops.Find(cop => cop.rank >= rank);
            if (matchedCop == null) return null;

            // Находим всех с рангом таким же, как у найденного ранее
            var matchedCops = cops.FindAll(cop => cop.rank == matchedCop.rank);

            // Возвращаем случайного из них
            return matchedCops[rnd.Next(matchedCops.Count)];
        }

        // Дает копам разрешить дело
        // Повышает ранг и перераспределяет их, если они справились
        // Выводит сообщение о неудаче, если они не справились
        public void SolveCase() {

            // Берем случайного с низшем рангом, если участок не пуст
            Cop cop = GetCopWithAtLeast(0);
            if(cop != null) {

                // Даем ему или его боссам решить дело
                cop = cop.SolveCase();
                if (cop != null) {

                    // Если один из них справился, повышаем ему ранг...
                    if (cop.rank < 11) {
                        cop.rank++;
                    }

                    // считаем дело решенным...
                    solvedCases++;

                    // и перераспределяем всех
                    SortCops(cop);
                    Console.WriteLine("");
                    return;
                };
            };
            // Иначе считаем дело нерешенным
            unsolvedCases++;
            Console.WriteLine("Case remains unsolved...\n");
        }

        // Сортирует копов по рангу и перераспределяет боссов
        // Если передать повышенного копа, перераспределяться будут только его подчененные и он сам
        private void SortCops(Cop promoted = null) {
            cops.Sort((cop1, cop2) => {
                return cop1.rank > cop2.rank ? 1 : (cop1.rank < cop2.rank ? -1 : 0);
            });
            for (int i = 0; i < cops.Count; i++) {
                Cop cop = cops[i];
                if (promoted == null || cop == promoted || cop.boss == promoted) {
                    cop.boss = GetCopWithAtLeast(cop.rank + 1);
                }
                if (cop.boss == null) break;
            }
        }

        // Выводит отчет о всех копах и делах
        public void PrintReport() {
            Console.WriteLine("Police Report");
            Console.WriteLine("{0,-16} {1} {2} {3} {4}", "Officer", "Rank", "Solved", "Failed", "Boss");
            cops.ForEach(cop => {
                Console.WriteLine("{0,-16} {1,-4} {2,-6} {3,-6} {4}", cop.name, cop.rank, cop.solvedCases, cop.failedCases, cop.boss == null ? "N/A" : cop.boss.name);
            });
            Console.WriteLine("\nCases solved: {0}", solvedCases);
            Console.WriteLine("Cases unsolved: {0}\n", unsolvedCases);
        }
    }

    // Класс копа
    public class Cop {

        // Генератор случайных чисел для определения решаемости дел
        private Random rnd = new Random();

        // Ранг
        public int rank;

        // Кол-во решенных дел
        public int solvedCases { get; private set; } = 0;

        // Кол-во нерешенных дел
        public int failedCases { get; private set; } = 0;

        // Босс
        public Cop boss;

        // Имя
        public readonly string name;

        // Конструктор, устанавливающий имя и ранг
        public Cop(string name, int rank) {
            this.name = name;
            this.rank = rank;
        }

        // Дает копу и его боссам решить дело
        public Cop SolveCase() {

            // Бросок кубика на решение дела
            if(rnd.Next(18) <= rank) {
                // Дело решено, считаем его, сообщаем об этом и возвращаем себя
                solvedCases++;
                Console.WriteLine("{0,-14} ({1}): solved!", name, rank);
                return this;
            }
            else {
                // Дело не решено, считаем его, сообщаем об этом и даем решить его боссу или его боссу и т.д. пока босс есть
                failedCases++;
                Console.WriteLine("{0,-14} ({1}): failed", name, rank);
                return boss?.SolveCase();
            }
        }
    }
}
