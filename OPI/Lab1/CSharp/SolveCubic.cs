using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp {

    // http://www.1728.org/cubic2.htm
    class CubicSolver {
        static string[] Solve(double a, double b, double c, double d) {

            double pow = 1.0 / 3.0;

            string x1 = "", x2 = "", x3 = "";

            // f
            double f = ((3 * c) / a) - (((b * b) / (a * a))) / 3;

            // g
            double g = (2 * ((b * b * b) / (a * a * a)) - (9 * b * c / (a * a)) + ((27 * (d / a)))) / 27;

            // h
            double h = ((g * g) / 4) + ((f * f * f) / 27);

            if (h > 0) {
                double m = -(g / 2) + (Math.Sqrt(h));
                m = Program.PowWithNegative(m, pow);

                double n = -(g / 2) - (Math.Sqrt(h));
                n = Program.PowWithNegative(n, pow);

                x1 = "" + ((m + n) - (b / (3 * a)));
                // ((S+U) - (b/(3*a)))
                x2 = (-1 * (m + n) / 2 - (b / (3 * a)) + " + i* " + ((m - n) / 2) * Math.Pow(3, .5));
                // -(S + U)/2 - (b/3a) + i*(S-U)*(3)^.5
                x3 = (-1 * (m + n) / 2 - (b / (3 * a)) + " - i* " + ((m - n) / 2) * Math.Pow(3, .5));
            }
            else if (h == 0 && f == 0 && g == 0) {
                double x = 0;

                if (d >= 0) {
                    x = Math.Pow(d / a, pow);
                    x = x * -1;
                }
                else {
                    d = d * -1;
                    x = Math.Pow(d / a, pow);
                }

                x1 = "" + x;
                x2 = "" + x;
                x3 = "" + x;
            }
            else {
                // -(S + U)/2  - (b/3a) - i*(S-U)*(3)^.5
                double r = Math.Sqrt((g * g / 4) - h);
                r = Program.PowWithNegative(r, pow);

                double theta = Math.Acos((-g / (2 * r)));

                double xx1 = 2 * (r * Math.Cos(theta / 3)) - (b / (3 * a));
                double x2a = r * -1;
                double x2b = Math.Cos(theta / 3);
                double x2c = Math.Sqrt(3) * (Math.Sin(theta / 3));
                double x2d = (b / 3 * a) * -1;

                double xx2 = (x2a * (x2b + x2c)) - (b / (3 * a));
                double xx3 = (x2a * (x2b - x2c)) - (b / (3 * a));

                x1 = "" + Math.Round(xx1 * 1E+14) / 1E+14;
                x2 = "" + Math.Round(xx2 * 1E+14) / 1E+14;
                x3 = "" + Math.Round(xx3 * 1E+14) / 1E+14;
            }

            return new[] { x1, x2, x3 };
        }
    }
}
