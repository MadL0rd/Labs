using System;
using System.Collections.Generic;

namespace Lab1 {
    class Program {
        static void Main(string[] args) {

            // ������� ���������
            var caller1 = new Caller("6942069");
            var caller2 = new Caller("5553535");
            var caller3 = new Caller("1111111");

            // ������
            caller1.Call(caller2);
            caller1.Call(caller3);
            caller2.Call(caller1);
            caller2.Call("0");
        }
    }

    // ���������� �����
    public sealed class CallCenter {

        // ��������� ������ ������� ������������ ������� ����� ������
        // ����� ��������������� ��� ������ ���������� ������ �� ����� ����������
        private static readonly CallCenter instance = new CallCenter();

        // ������ ����������� �������
        private CallCenter() { }

        // ����� ��� ���� �� ����� ���������� � ������������� ������� ������ ������
        public static CallCenter Instance {

            // �� ��������� ���������� ������� ������
            get {
                return instance;
            }
        }

        // ������ ���������� �������
        private List<string> numbers = new List<string> {
            "6942069",
            "1111111",
            "9876543"
        };

        // ���������� ������� ��������� ���� ���������
        // ����� � �������, ���� ���� �� ��������� �� ���������� ��� ���� ���������� ������ �������
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

        // ���������� ����������������� ������ ���� xx-xxx-xx, ���� ������ ��������� �� ����� � ���������� �������
        public string FormatNumber(string number) {
            if (number.Length != 7) {
                return number;
            }
            else {
                return number.Substring(0, 2) + "-" + number.Substring(2, 3) + "-" + number.Substring(5, 2);
            }
        }
    }

    // �������
    public class Caller {

        // ���������� �����
        public string number;

        // ����������� � ��������� ������
        public Caller(string number) {
            this.number = number;
        }

        // ������ �� ���������� ������
        public void Call(string number) {
            Console.WriteLine("Calling {0} from {1}", CallCenter.Instance.FormatNumber(number), CallCenter.Instance.FormatNumber(this.number));
            Console.WriteLine(CallCenter.Instance.Call(this.number, number));
        }

        // ������ ���������� ��������
        public void Call(Caller caller) {
            Call(caller.number);
        }
    }
}