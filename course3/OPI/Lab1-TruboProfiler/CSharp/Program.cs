using System;
using System.IO;

namespace CSharp {
    class Program {

        static long Factorial(long a) {
            if (a <= 1)
                return 1;

            long res = a;

            while (--a > 0) {
                res = res * a;
            }

            return res;
        }

        //  Negative roots lel
        public static double PowWithNegative(double n, double pow) {

            if (n < 0) {
                return Math.Pow(n * -1, pow) * -1;
            }
            else {
                return Math.Pow(n, pow);
            }
        }

        // http://www.1728.org/cubic2.htm
        // b = 0, c = 0
        static string[] SolveKindOfCubic(double a, double d) {

            string x1 = "", x2 = "", x3 = "";

            double g = d / a;

            double hsqrt = Math.Sqrt(g * g / 4);

            double pow = 1.0 / 3.0;

            double s = -(g / 2) + hsqrt;
            s = PowWithNegative(s, pow);

            double u = -(g / 2) - hsqrt;
            u = PowWithNegative(u, pow);

            x1 = "" + (s + u);
            // ((S+U) - (b/(3*a)))
            x2 = (-1 * (s + u) / 2 + " + i*" + ((s - u) / 2) * Math.Pow(3, 0.5));
            // -(S + U)/2 - (b/3a) + i*(S-U)*(3)^.5
            x3 = (-1 * (s + u) / 2 + " - i*" + ((s - u) / 2) * Math.Pow(3, 0.5));

            return new[] { x1, x2, x3 };
        }

        static long CountEvenColPositiveCells(long[][] matrix) {
            long count = 0;

            for(long n = 1; n < matrix.Length; n = n + 2) {
                for(long m = 0; m < matrix[n].Length; m++) {
                    if(matrix[n][m] > 0) {
                        count++;
                    }
                }
            }

            return count;
        }

        static long[][] ReadMatrix(StreamReader input, long n, long m) {
            long[][] res = new long[n][];
            for (long i = 0; i < n; i++) {
                res[i] = new long[m];
                string[] str = input.ReadLine().Split(' ');
                for (long k = 0; k < m; k++) {
                    res[i][k] = long.Parse(str[k]);
                    Console.Write(res[i][k] + " ");
                }
                Console.WriteLine();
            }
            return res;
        }

        static void Main(string[] args) {

            string path = args.Length > 0 ? args[0] : "input.txt";
            StreamReader input = new StreamReader(path);

            long n1 = args.Length > 1 ? long.Parse(args[1]) : 5;
            long n2 = args.Length > 2 ? long.Parse(args[2]) : 6;

            long amount = long.Parse(input.ReadLine());

            while (!input.EndOfStream) {

                Console.WriteLine("T(5, 5)");
                long[][] m1 = ReadMatrix(input, n1, n1);
                Console.WriteLine("D(6, 6)");
                long[][] m2 = ReadMatrix(input, n2, n2);
                Console.WriteLine();

                long c1, c2;
                c1 = CountEvenColPositiveCells(m1);
                c2 = CountEvenColPositiveCells(m2);
                Console.WriteLine("a = " + c1);
                Console.WriteLine("b = " + c2);

                double a = 3 * Factorial(c1 + c2);
                double d = 2 * Factorial(c1);

                Console.WriteLine("Solving cubic equiation for a = {0}; b = {1}; c = {2}; d = {3};", a, 0, 0, d);
                string[] x = SolveKindOfCubic(a, d);
                Console.WriteLine("x1 = " + x[0] + ";");
                Console.WriteLine("x2 = " + x[1] + ";");
                Console.WriteLine("x3 = " + x[2] + ";");
                Console.Write("\n\n");

                input.ReadLine();
            }

        }
    }
}
