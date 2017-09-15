// ������������

#include <cmath>
#include <iostream>

using std::cin;
using std::cout;
using std::endl;


// ����������� ����� �������������� �������
class MathFunction {
public:
	virtual double solveFor(double x) = 0;
};


/* ���������� ����������� ������� */

// ����� ������� ��������
class Parabola : public MathFunction {
private:
	double p;
public:
	Parabola(double p);
	double solveFor(double x);
};

// ����� ������� ���������
class Hyperbola : public MathFunction {
private:
	double a, b;
public:
	Hyperbola(double a, double b);
	double solveFor(double x);
};

// ����� ������� �������
class Ellipse : public MathFunction {
private:
	double a, b;
public:
	Ellipse(double a, double b);
	double solveFor(double x);
};


/*
* �������� ����������� ������� 
* ������ ��� �������� ������� - https://www.desmos.com/calculator/4gsilibcxn
*/

// ������ �������� - y^2 = 2px
// ��� ����������� p > 0
Parabola::Parabola(double p) : p(p) {};
double Parabola::solveFor(double x) {
	return sqrt(2 * p * x);
}

// ������ ��������� - x^2/a^2 - y^2/b^2 = 1
Hyperbola::Hyperbola(double a, double b) : a(a), b(b) {};
double Hyperbola::solveFor(double x) {
	return sqrt(((x*x) / (a*a) - 1)*b*b);
}

// ������ ������� - x^2/a^2 + y^2/b^2 = 1
// ��� ����������� a > b
Ellipse::Ellipse(double a, double b) : a(a), b(b) {};
double Ellipse::solveFor(double x) {
	return sqrt((1 - (x*x) / (a*a))*b*b);
}


/* ����� ��� �������� ����������� ���������� ������� */
class Series {
public:
	double* data = nullptr;	// ������ �����������

	Series(MathFunction* func, int len);

	~Series();

	void store(double x);
	void purge();
	void log();

	bool isEmpty();
	bool isFull();

private:
	MathFunction* func;	// ����� �������
	int top = -1;		// ������ �������� ����������
	int len;			// ����. ���-�� �����������
};

// �����������, ��������� ��������� �� ������ ������ �� ������� �������������� �������
// � ������������ ���-�� ����������� y
Series::Series(MathFunction* func, int len) : func(func), len(len) {
	data = new double[len];
}

Series::~Series() {
	delete[] data;
}

// ��������� y = f(x) � ������
void Series::store(double x) {
	if (isFull()) {
		cout << "Full" << endl;
		return;
	}
	double y = func->solveFor(x);
	data[++top] = y;
	cout << y << endl;
}

// ������� ��� ����������� ����������
void Series::purge() {
	while (top--) {
		data[top] = NULL;
	}
}

// ������� ��� ����������
void Series::log() {
	if (isEmpty()) {
		cout << "Empty" << endl;
		return;
	}
	int i = 0;
	while (i <= top) {
		cout << data[i] << (i == top ? "\n" : ", ");
		++i;
	}
}

// ��� �����������
bool Series::isEmpty() {
	return top == -1;
}

// ������ ����������� ��������
bool Series::isFull() {
	return top == len - 1;
}


/* �������� ������ */
int main() {
	cout << "Ellipse:\n";
	Series e(&Ellipse(-3, 2.5), 3);
	e.store(2);
	e.store(10);
	e.store(-1);
	e.store(2);
	e.log();
	e.purge();
	e.log();

	cout << "Parabola:\n";
	Series p(&Parabola(1), 5);
	p.store(2);
	p.store(10);
	p.store(-1);
	p.store(2);
	p.log();

	cout << "Hyperbola:\n";
	Series h(&Hyperbola(2, 1), 2);
	h.store(2);
	h.store(10);
	h.store(-1);
	h.store(2);
	h.log();

    return 0;
}
