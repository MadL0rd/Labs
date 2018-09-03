#include <cmath>
#include "Shapes.h"

// Конструктор указания позиции
Shape::Shape(const Point& position) : position(position) {}

// Возвращает позицию фигуры
Point Shape::getPosition() const {
	return position;
}

// Устанавливает позицию фигуры
void Shape::setPosition(const Point& position) {
	this->position = position;
}

// Статические методы Shape

// Сравнивает две фигуры, возвращает разницу между площадями
double Shape::compare(Shape* a, Shape* b) {
	return a->getArea() - b->getArea();
}

// Возвращает пересекаются ли окружности, включающие в себя фигуры
bool Shape::intersects(Shape* a, Shape* b) {
	const Point ap = a->position;
	const Point bp = b->position;
	double ar = a->getBoundingRadius();
	double br = b->getBoundingRadius();
	double d = sqrt(pow(bp.x - ap.x, 2) + pow(bp.y - ap.y, 2));

	// Проверяем не находится ли одна из окружностей внутри другой
	if (ar - d - br > EPSILON || br - d - ar > EPSILON) {
		return false;
	}

	double dif = d - (ar + br);
	return dif < EPSILON;
}

// Для двух окружностей, включающих в себя фигуры,
// возвращает находится ли вторая окружность внутри первой
bool Shape::contains(Shape* a, Shape* b) {
	const Point ap = a->position;
	const Point bp = b->position;
	double ar = a->getBoundingRadius();
	double br = b->getBoundingRadius();
	double d = sqrt(pow(bp.x - ap.x, 2) + pow(bp.y - ap.y, 2));
	return ar - d - br > EPSILON;
}


// Фабрика фигур
// Принимает обязательный тип и опциональные позицию и размер фигуры 
Shape* Shape::create(
	Shape::Type type,
	Point position = { 0, 0 },
	const std::initializer_list<double>& sizes = {}
) {
	switch (type) {
	case Shape::Type::Square:
		return new Square(position, sizes);
	case Shape::Type::Hexagon:
		return new Hexagon(position, sizes);
	case Shape::Type::Trapeze:
		return new Trapeze(position, sizes);
	default:
		return nullptr;
	}
}
