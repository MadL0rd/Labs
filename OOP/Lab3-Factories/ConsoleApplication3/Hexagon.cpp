#include <string>
#include <cmath>
#include <sstream>
#include <vector>
#include "Hexagon.h"

using std::string;
using std::ostringstream;
using std::initializer_list;

// Конструктор с указанием позиции и стороны шестиугольника
Hexagon::Hexagon(const Point& position, const initializer_list<double>& sizes) : Shape(position) {
	setSize(sizes);
}

double Hexagon::getArea() const {
	return (3 * sqrt(3) * sideWidth * sideWidth) / 2;
}

double Hexagon::getSize() const {
	return sideWidth;
}

double Hexagon::getBoundingRadius() const {
	return sideWidth;
}

// Устанавливает размеры шестиугольника
// sizes[0] - длина стороны шестиугольника
void Hexagon::setSize(const initializer_list<double>& sizes) {
	if (sizes.size() == 1) {
		sideWidth = sizes.begin()[0];
	}
	if (sideWidth < 0) {
		sideWidth = 0;
	}
}

string Hexagon::getInfo() const {
	ostringstream info;
	info << getType() <<
		" with side width " << getSize() <<
		", position {x: " << getPosition().x << ", y:" << getPosition().y <<
		"} and area " << getArea();
	return info.str();
}

string Hexagon::getType() const {
	return "Hexagon";
}
