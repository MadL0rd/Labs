#pragma once

#include <iostream> 

#define EPSILON 0.00000001

// �����/�������
struct Point {
	double x, y;
};

/*
* ����������� ����� ������ � ��������� ���������� ������
* + ������� ��� ������ � ������ �����
*/
class Shape {
public:
	Shape(const Point&);

	enum class Type;
	static Shape* create(Shape::Type, Point, const std::initializer_list<double>&);
	static double compare(Shape*, Shape*);
	static bool intersects(Shape*, Shape*);
	static bool contains(Shape*, Shape*);

	// ���������� ������� ������
	virtual double getArea() const = 0;

	// ���������� ������ ���������� ����������, ���������� � ���� ������
	virtual double getBoundingRadius() const = 0;

	// ���������� ������ ������
	virtual double getSize() const = 0;

	// ������������� ������ ������
	// ������ ������ ����� ��������� ��������� ���-�� ���������� �������
	virtual void setSize(const std::initializer_list<double>&) = 0;

	// ���������� ���������� � ������
	virtual std::string getInfo() const = 0;

	// ���������� ��� ������ (������ ��� ������ � �������)
	virtual std::string getType() const = 0;

	Point getPosition() const;
	void setPosition(const Point& pos);

protected:
	Point position;

};

// ���� �����
enum class Shape::Type {
	Square = 0,
	Hexagon = 1,
	Trapeze = 2
};
