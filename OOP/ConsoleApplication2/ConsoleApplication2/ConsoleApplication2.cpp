// Наследование

#include <cmath>
#include <iostream>

using std::cin;
using std::cout;
using std::endl;


// Абстрактный класс математических функций
class MathFunction {
public:
	virtual double solveFor(double x) = 0;
};


/* Объявление наследующих классов */

// Класс функции параболы
class Parabola : public MathFunction {
private:
	double p;
public:
	Parabola(double p);
	double solveFor(double x);
};

// Класс функции гиберболы
class Hyperbola : public MathFunction {
private:
	double a, b;
public:
	Hyperbola(double a, double b);
	double solveFor(double x);
};

// Класс функции эллипса
class Ellipse : public MathFunction {
private:
	double a, b;
public:
	Ellipse(double a, double b);
	double solveFor(double x);
};


/*
* Описание наследующих классов 
* График для проверки функций - https://www.desmos.com/calculator/4gsilibcxn
*/

// Методы параболы - y^2 = 2px
// Без ограничения p > 0
Parabola::Parabola(double p) : p(p) {};
double Parabola::solveFor(double x) {
	return sqrt(2 * p * x);
}

// Методы гиперболы - x^2/a^2 - y^2/b^2 = 1
Hyperbola::Hyperbola(double a, double b) : a(a), b(b) {};
double Hyperbola::solveFor(double x) {
	return sqrt(((x*x) / (a*a) - 1)*b*b);
}

// Методы эллипса - x^2/a^2 + y^2/b^2 = 1
// Без ограничения a > b
Ellipse::Ellipse(double a, double b) : a(a), b(b) {};
double Ellipse::solveFor(double x) {
	return sqrt((1 - (x*x) / (a*a))*b*b);
}


/* Класс для хранения результатов выполнения функций */
class Series {
public:
	double* data = nullptr;	// Массив результатов

	Series(MathFunction* func, int len);

	~Series();

	void store(double x);
	void purge();
	void log();

	bool isEmpty();
	bool isFull();

private:
	MathFunction* func;	// Класс функции
	int top = -1;		// Индекс верхнего результата
	int len;			// Макс. кол-во результатов
};

// Конструктор, принимает указатель на объект одного из классов математических функций
// и максимальное кол-во сохраняемых y
Series::Series(MathFunction* func, int len) : func(func), len(len) {
	data = new double[len];
}

Series::~Series() {
	delete[] data;
}

// Добавляет y = f(x) в массив
void Series::store(double x) {
	if (isFull()) {
		cout << "Full" << endl;
		return;
	}
	double y = func->solveFor(x);
	data[++top] = y;
	cout << y << endl;
}

// Убирает все сохраненные результаты
void Series::purge() {
	while (top--) {
		data[top] = NULL;
	}
}

// Выводит все результаты
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

// Нет результатов
bool Series::isEmpty() {
	return top == -1;
}

// Массив результатов заполнен
bool Series::isFull() {
	return top == len - 1;
}


/* Тестовые вызовы */
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
