#include <string>
#include <cmath>
#include <sstream>
#include <vector>
#include "Square.h"
#include "Shapes.h"

using std::string;
using std::ostringstream;
using std::initializer_list;

// Конструктор с указанием позиции и стороны квадрата
Square::Square(const Point& position, const initializer_list<double>& sizes) : Shape(position) {
	setSize(sizes);
}

double Square::getArea() const {
	return sideWidth*sideWidth;
}

double Square::getSize() const {
	return sideWidth;
}

double Square::getBoundingRadius() const {
	return sideWidth*sqrt(2) / 2;
}

// Устанавливает размеры квадрата
// sizes[0] - длина стороны квадрата
void Square::setSize(const initializer_list<double>& sizes) {
	if (sizes.size() == 1) {
		sideWidth = sizes.begin()[0];
	}
	if (sideWidth < 0) {
		sideWidth = 0;
	}
}

string Square::getInfo() const {
	ostringstream info;
	info << getType() <<
		" with side width " << getSize() <<
		", position {x: " << getPosition().x << ", y:" << getPosition().y <<
		"} and area " << getArea();
	return info.str();
}

string Square::getType() const {
	return "Square";
}
