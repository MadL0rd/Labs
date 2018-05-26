using System;
using System.Collections.Generic;

namespace Lab1 {
    class Program {
        static void Main(string[] args) {

            // Создаем абонентов
            var caller1 = new Caller("6942069");
            var caller2 = new Caller("5553535");
            var caller3 = new Caller("1111111");

            // Звоним
            caller1.Call(caller2);
            caller1.Call(caller3);
            caller2.Call(caller1);
            caller2.Call("0");
        }
    }

    // Телефонный центр
    public sealed class CallCenter {

        // Доступный только изнутри единственный объекта этого класса
        // Будет инициализирован при первом упоминании класса во время выполнения
        private static readonly CallCenter instance = new CallCenter();

        // Делаем конструктор скрытым
        private CallCenter() { }

        // Через это поле мы будем обращаться к единственному объекту нашего класса
        public static CallCenter Instance {

            // По обращению возвращаем скрытый объект
            get {
                return instance;
            }
        }

        // Список телефонных номеров
        private List<string> numbers = new List<string> {
            "6942069",
            "1111111",
            "9876543"
        };

        // Производит попытку соединить двух абонентов
        // Пишет в консоль, если один из абонентов не существует или если соединение прошло успешно
        public string Call(string from, string to) {
            if (!numbers.Contains(from)) {
                return "Unidentified caller " + FormatNumber(from);
            }
            else if (!numbers.Contains(to)) {
                return "Unidentified callee " + FormatNumber(to);
            }
            else {
                return "Call successful!";
            }
        }

        // Возвращает отформатированную строку вида xx-xxx-xx, если строка совпадает по длине с телефонным номером
        public string FormatNumber(string number) {
            if (number.Length != 7) {
                return number;
            }
            else {
                return number.Substring(0, 2) + "-" + number.Substring(2, 3) + "-" + number.Substring(5, 2);
            }
        }
    }

    // Абонент
    public class Caller {

        // Телефонный номер
        public string number;

        // Конструктор с указанием номера
        public Caller(string number) {
            this.number = number;
        }

        // Звонит по указанному номеру
        public void Call(string number) {
            Console.WriteLine("Calling {0} from {1}", CallCenter.Instance.FormatNumber(number), CallCenter.Instance.FormatNumber(this.number));
            Console.WriteLine(CallCenter.Instance.Call(this.number, number));
        }

        // Звонит указанному абоненту
        public void Call(Caller caller) {
            Call(caller.number);
        }
    }
}