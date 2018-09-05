#include <stdio.h>
#include <stdlib.h>
#include <math.h>

long Factorial(long a);
float PowWithNegative(float n, float p);
void SolveKindOfCubic(float a, float d);
long CountEvenColPositiveCells(long n, long m);

int main(int argc, char *argv[]) {

	long n1 = 5;
	long n2 = 6;
	int amount;
	long **m1, **m2;
	float a, d;
	long i;
	long c1, c2;

	if (argc > 2) {
		freopen(argv[2], "r", stdin);
	}
	else {
		freopen("input.txt", "r", stdin);
	}

	scanf("%d", &amount);
	printf("C program.\n%d matrices.\n", amount);

	while (amount--) {

		printf("T(5, 5)\n");
		c1 = CountEvenColPositiveCells(n1, n1);
		printf("D(6, 6)\n");
		c2 = CountEvenColPositiveCells(n2, n2);
		printf("\n");
		printf("a = %ld;\n", c1);
		printf("b = %ld;\n", c2);

		a = 3 * Factorial(c1 + c2);
		d = 2 * Factorial(c1);

		SolveKindOfCubic(a, d);
		printf("\n\n\n");
	}

	return 0;
}


long Factorial(long a) {

	long res;

	if (a <= 1) {
		return 1;
	}

	res = a;

	while (--a > 0) {
		res = res * a;
	}

	return res;
}

float PowWithNegative(float n, float p) {

	if (n < 0) {
		return pow(n * -1, p) * -1;
	}
	else {
		return pow(n, p);
	}
}

void SolveKindOfCubic(float a, float d) {
	float g, p, s, u, x1, x2r, x2i, hsqrt;

	printf("Solving cubic equiation for a = %.1f; b = 0.0; c = 0; d = %.1f;\n", a, d);

	g = d / a;

	hsqrt = sqrt(g * g / 4);

	p = 1.0 / 3.0;

	s = -(g / 2) + hsqrt;
	s = PowWithNegative(s, p);

	u = -(g / 2) - hsqrt;
	u = PowWithNegative(u, p);

	x1 = s + u;

	x2r = -1 * (s + u) / 2;
	x2i = ((s - u) / 2) * pow(3, 0.5);

	printf("x1 = %.8f;\nx2 = %.8f + i*%.8f;\nx3 = %.8f - i*%.8f;", x1, x2r, x2i, x2r, x2i);
}

long CountEvenColPositiveCells(long n, long m) {
	long i, k, count = 0, v = 0;

	for (k = 0; k < m; k++) {
		for (i = 0; i < n; i = i + 2) {
			scanf("%ld", &v);
			printf("%ld ", v);

			if (i + 1 < n) {
				scanf("%ld", &v);
				printf("%ld ", v);

				if (v > 0) {
					count++;
				}
			}
		}
		
		printf("\n");
	}

	return count;
}
