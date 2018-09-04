#include <stdio.h>
#include <stdlib.h>
#include <math.h>

long Factorial(long a);
float PowWithNegative(float n, float p);
void SolveKindOfCubic(float a, float d);
long CountEvenColPositiveCells(long** matrix, long n, long m);
long** ReadMatrix(long n, long m);

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

	while (amount--) {

		printf("T(5, 5)\n");
		m1 = ReadMatrix(n1, n1);
		printf("D(6, 6)\n");
		m2 = ReadMatrix(n2, n2);
		printf("\n");

		c1 = CountEvenColPositiveCells(m1, n1, n1);
		c2 = CountEvenColPositiveCells(m2, n2, n2);
		printf("a = %ld\n", c1);
		printf("b = %ld\n", c2);

		a = 3 * Factorial(c1 + c2);
		d = 2 * Factorial(c1);

		SolveKindOfCubic(a, d);

		for (i = 0; i < n1; i++) {
			free(m1[i]);
		}
		for (i = 0; i < n2; i++) {
			free(m2[i]);
		}
		free(m1);
		free(m2);
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

	printf("Solving cubic equiation for a = %f; b = 0; c = 0; d = %f;\n", a, d);

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

	printf("x1 = %f;\nx2 = %f + i*%f;\nx3 = %f - i*%f", x1, x2r, x2i, x2r, x2i);
}

long CountEvenColPositiveCells(long **matrix, long n, long m) {
	long i, k, count;

	count = 0;

	for (i = 1; i < n; i = i + 2) {
		for (k = 0; k < m; k++) {
			if (matrix[i][k] > 0) {
				count++;
			}
		}
	}

	return count;
}

long** ReadMatrix(long n, long m) {
	long i, k;
	long **res = (long**)malloc(sizeof(long*)*n);
	for (i = 0; i < n; i++) {
		res[i] = (long*)malloc(sizeof(long)*m);
		for (k = 0; k < m; k++) {
			scanf("%ld", &res[i][k]);
			printf("%ld ", res[i][k]);
		}
		printf("\n");
	}
	return res;
}

