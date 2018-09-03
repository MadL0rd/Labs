using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator {
    class Program {

        static Random Rnd = new Random();

        static void Main(string[] args) {

            int amount = args.Length > 0 ? int.Parse(args[0]) : 1;
            string path = args.Length > 1 ? args[1] : "input.txt";
            int n1 = args.Length > 2 ? int.Parse(args[2]) : 5;
            int n2 = args.Length > 3 ? int.Parse(args[3]) : 6;
            

            string generated = "";
            for(int i = 0; i < amount; i++) {
                generated += GenerateMatrix(n1, n1);
                generated += GenerateMatrix(n2, n2);
                generated += "\n";
            }

            System.IO.File.WriteAllText(path, generated);
        }

        static string GenerateMatrix(int n, int m) {
            string res = "";
            for (int i = 0; i < n; i++) {
                for (int k = 0; k < m; k++) {
                    res += Rnd.Next(-100, 101) + " ";
                }
                res += "\n";
            }
            return res;
        }
    }
}
