// Фабрики
#include <sstream>
#include "Shapes.h"

using std::cin;
using std::cout;
using std::endl;
using std::string;

// Выводит информацию о двух фигурах
void printInfo(Shape* s, Shape* h) {
	string difString;
	double dif = Shape::compare(s, h);
	if (dif > EPSILON) {
		difString = " is bigger than ";
	}
	else if (dif < -EPSILON) {
		difString = " is smaller than ";
	}
	else {
		difString = " is equal size to ";
	}
	string stype = s->getType();
	string htype = h->getType();
	cout <<
		s->getInfo() << endl <<
		h->getInfo() << endl <<
		std::boolalpha <<
		stype << difString << htype << endl <<
		stype << " intersects " << htype << ": " << Shape::intersects(s, h) << endl <<
		stype << " contains " << htype << ": " << Shape::contains(s, h) << endl <<
		stype << " contains " << htype << ": " << Shape::contains(h, s) << endl << endl;
}

int main() {
	Shape* s = Shape::create(Shape::Type::Square, { 1, 1 }, { 5 });
	Shape* h = Shape::create(Shape::Type::Hexagon, { -2, 3 }, { 2 });
	printInfo(s, h);

	s->setSize({ sqrt(h->getArea()) });
	h->setPosition({ 10, 10 });
	printInfo(s, h);

	h->setSize({ 100 });
	s->setPosition({ 20, 10 });
	printInfo(s, h);

	s->setSize({ 1 });
	h->setPosition({ 100, 100 });
	printInfo(s, h);

	delete s;
	delete h;

	return 0;
}
