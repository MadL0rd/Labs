#pragma once
#include "Shape.h"

// Квадрат
class Square : public Shape {
public:
	using Shape::Shape;
	Square(const Point&, const std::initializer_list<double>&);
	double getArea() const;
	double getBoundingRadius() const;
	double getSize() const;
	void setSize(const std::initializer_list<double>&);
	std::string getInfo() const;
	std::string getType() const;
private:
	double sideWidth = 0;
};
