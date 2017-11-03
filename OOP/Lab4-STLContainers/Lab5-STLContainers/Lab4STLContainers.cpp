#include <iostream>
#include <map>
#include <vector>
#include <string>
#include <fstream>
#include <algorithm>
#include <iomanip>

using namespace std;

struct CompCenter {
	int year, lab;
	string cpu, screen, ram, diskSpace;
};

typedef multimap<int, CompCenter> DBMapCont;
typedef vector<CompCenter> DBVectorCont;

// Абстрактный класс базы данных
class DBBase {

protected:
	ifstream openFile(string filename) {
		ifstream file(filename);
		if (!file) {
			throw runtime_error("File couldn't be opened");
		}
		return file;
	}

	CompCenter readEntry(ifstream& file) {
		CompCenter cc;

		file >> cc.lab;

		// Проверяем конец файла
		if (file.eof()) {
			cc.year = 0;
			return cc;
		}

		file >> cc.cpu;
		file >> cc.ram;
		file >> cc.diskSpace;
		file >> cc.screen;
		file >> cc.year;

		// Пропускаем конец строки
		file.get();

		return cc;
	}

	void printEntry(CompCenter cc) {
		cout << 
			setw(4) << cc.lab << ' ' << 
			setw(5) << cc.cpu << ' ' << 
			setw(6) << cc.ram << ' ' << 
			setw(10) << cc.diskSpace << ' ' << 
			setw(11) << cc.screen << ' ' <<
			setw(9) << cc.year << endl;
	}

public:
	virtual void read(string filename) = 0;
	virtual void printBefore(int year) = 0;

	void printHeader() {
		cout << "LabN   CPU    RAM Disk space Screen type Year made" << endl;
	}

};

// База данных в виде мапы
class DBMap : public DBBase {
private:
	DBMapCont db;

public:
	void read(string filename) {
		ifstream file = openFile(filename);
		while (!file.eof()) {
			CompCenter cc = readEntry(file);
			if (cc.year > 0) {
				db.insert({ cc.year, cc });
			}
		}
		file.close();
	}

	void printBefore(int year) {

		DBMapCont::iterator it, lower_bound, upper_bound;
		lower_bound = db.lower_bound(0);
		upper_bound = db.upper_bound(year - 1);
		int i = 0;

		for (it = lower_bound; it != upper_bound; ++it) {
			printEntry((*it).second);
			i++;
		}
		if (i == 0) {
			cout << "No entries found" << endl;
		}
	}
};

// База данных в виде вектора
class DBVector : public DBBase {
private:
	DBVectorCont db;

	bool cmp(const CompCenter& a, const CompCenter& b) {
		return a.year > b.year;
	}

public:
	void read(string filename) {
		ifstream file = openFile(filename);
		while (!file.eof()) {
			CompCenter cc = readEntry(file);
			if (cc.year > 0) {
				db.push_back(cc);
			}
		}
		sort(db.begin(), db.end(), [](const CompCenter& a, const CompCenter& b) {
			return a.year < b.year;
		});
		file.close();
	}
	void printBefore(int year) {
		size_t i = 0;
		for (; i < db.size(); i++) {
			const CompCenter& cc = db[i];
			if (cc.year >= year) {
				break;
			}
			printEntry(cc);
		}
		if (i == 0) {
			cout << "No entries found" << endl;
		}
	}
};

// Считывает год из консоли и возвращает его
int readYear() {
	string yearString;
	cout << "Enter year: ";
	cin >> yearString;
	cout << endl;
	try {
		return stoi(yearString);
	}
	catch (...) {
		return 0;
	}
}

int main() {

	DBMap dbMap;
	DBVector dbVector;
	
	string filename = "input.txt";

	// Считываем информацию из файла в базы данных
	try {
		dbMap.read(filename);
		dbVector.read(filename);
	}
	catch (runtime_error e) {
		cout << e.what() << endl;
	}

	// Выводим нужные элементы
	int year = readYear();
	while (year > 0) {
		cout << "Map:" << endl;
		dbMap.printHeader();
		dbMap.printBefore(year);
		cout << endl << "Vector:" << endl;
		dbVector.printHeader();
		dbVector.printBefore(year);
		cout << endl;
		year = readYear();
	}

	return 0;
}

