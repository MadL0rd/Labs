#pragma once

#include <iostream> 

#define EPSILON 0.00000001

// Точка/позиция
struct Point {
	double x, y;
};

/*
* Абстрактный класс фигуры и держатель фабричного метода
* + методов для работы с парами фигур
*/
class Shape {
public:
	Shape(const Point&);

	enum class Type;
	static Shape* create(Shape::Type, Point, const std::initializer_list<double>&);
	static double compare(Shape*, Shape*);
	static bool intersects(Shape*, Shape*);
	static bool contains(Shape*, Shape*);

	// Возвращает площадь фигуры
	virtual double getArea() const = 0;

	// Возвращает радиус наименьшей окружности, включающей в себя фигуру
	virtual double getBoundingRadius() const = 0;

	// Возвращает размер фигуры
	virtual double getSize() const = 0;

	// Устанавливает размер фигуры
	// Каждая фигура может принимать различное кол-во параметров размера
	virtual void setSize(const std::initializer_list<double>&) = 0;

	// Возвращает информацию о фигуре
	virtual std::string getInfo() const = 0;

	// Вовзращает тип фигуры (только для вывода в консоль)
	virtual std::string getType() const = 0;

	Point getPosition() const;
	void setPosition(const Point& pos);

protected:
	Point position;

};

// Типы фигур
enum class Shape::Type {
	Square = 0,
	Hexagon = 1,
	Trapeze = 2
};
