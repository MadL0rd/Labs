#pragma once
#include "Shape.h"

// Трапеция
class Trapeze : public Shape {
public:
	using Shape::Shape;
	Trapeze(const Point&, const std::initializer_list<double>&);
	double getArea() const;
	double getBoundingRadius() const;
	double getSize() const;
	void setSize(const std::initializer_list<double>&);
	std::string getInfo() const;
	std::string getType() const;
private:
	double height = 0;
	double topSide = 0;
	double bottomSide = 0;
};
