#include <string>
#include <cmath>
#include <sstream>
#include <vector>
#include "Trapeze.h"

using std::cout;
using std::endl;
using std::string;
using std::ostringstream;
using std::initializer_list;

// Конструктор с указанием позиции и размеров трапеции
Trapeze::Trapeze(const Point& position, const initializer_list<double>& sizes) : Shape(position) {
	setSize(sizes);
}

double Trapeze::getArea() const {
	cout << "Not implemented" << endl;
	return NULL;
}
double Trapeze::getBoundingRadius() const {
	cout << "Not implemented" << endl;
	return NULL;
}

// Устанавливает размеры трапеции
// sizes[0] - высота
// sizes[1] - длина верхней стороны
// sizes[2] - длина нижней стороны
void Trapeze::setSize(const initializer_list<double>& sizes) {
	if (sizes.size() == 3) {
		height = sizes.begin()[0];
		topSide = sizes.begin()[1];
		bottomSide = sizes.begin()[2];
	}
	if (height < 0) {
		height = 0;
	}
	if (topSide < 0) {
		topSide = 0;
	}
	if (bottomSide < 0) {
		bottomSide = 0;
	}
}

double Trapeze::getSize() const {
	cout << "Not implemented" << endl;
	return NULL;
}

string Trapeze::getInfo() const {
	return "Not implemented";
}

string Trapeze::getType() const {
	return "Trapeze";
}