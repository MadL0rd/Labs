#include <iostream>
#include <map>
#include <string>
#include <fstream>

using std::string;
using std::cin;
using std::cout;
using std::endl;
using std::fstream;

// Максимальные размеры
typedef unsigned short int uint;
const uint MAX_HASH_VALUE = 1024;
const uint OVERFLOW_VALUE = USHRT_MAX;
const double MAX_FILL_DEFAULT = 0.95;
uint MAX_TABLE_SIZE;

// Статистика
int NUM_SUCCESS = 0;
int NUM_FAIL = 0;
int NUM_ATTEMPTS = 0;
int NUM_ATTEMPTS_SUCCESS = 0;
int NUM_ATTEMPTS_FAIL = 0;

// Элемент хеш-таблицы
class HashEntry {
private:
	string key;
	string value;
public:
	HashEntry(string key, string value): key(key), value(value) {}

	string getKey() {
		return key;
	}

	string getValue() {
		return value;
	}
};

// Указатель на сишную строку внутри c++ строки, в обычном виде и разбитой по 2 байта
union HashKey {
	const char *chars;
	const unsigned short *words;

	HashKey(const string& key) : chars(key.c_str()) {}
};

// Хеш-таблица
class HashMap {
private:
	std::map<uint, HashEntry> table;

	// Хеширует строку сложением двухбайтовых слов, при переполнении к сумме добавляется единица
	// Хеширует число умножением
	uint hash(HashKey key, size_t len) {
		uint i = 0;
		size_t halfLen = len / 2;

		// Прибавляем по два байта с учетом переполнения
		// Добавляем единицу при переполнении
		for (size_t j = 0; j < halfLen; ++j) {
			i = addWithOverflow(i, key.words[j], OVERFLOW_VALUE, 1);
		}

		// Если в строке нечетное кол-во символов, добавляем последний символ и пробел
		if (len % 2 != 0) {
			uint d = (uint(key.chars[len - 1]) << 8) + ' ';
			i += addWithOverflow(i, d, OVERFLOW_VALUE, 1);
		}
		return hashNumber(i);
	}

	// Хеширует число умножением
	// https://studopedia.ru/5_159617_metod-umnozheniya.html
	uint hashNumber(uint k){
		uint n = MAX_HASH_VALUE;
		double A = 0.618033; // Золотое сечение		
		uint h = n*fmod(double(k)*A, 1); // h(k)=floor[n*({k*A})]
		return h;
	}

	// Пытается найти значение по ключу начиная с указанного хеша
	std::pair<uint, string> getPair(uint i, const string& key) {
		uint d = 1;

		// Находим элемент по ключу-цифре
		while (table.find(i) != table.end()) {
			NUM_ATTEMPTS++;
			HashEntry entry = table.at(i);

			// Сравниваем ключ-строку
			if (key == entry.getKey()) {
				// Элемент найден
				return { i, entry.getValue() };
			}

			// Алгоритм квадратичных проб с учетом переполнения
			i = addWithOverflow(i, d, MAX_HASH_VALUE, 0);
			d = addWithOverflow(d, 2, OVERFLOW_VALUE, 0);

			cout << i << " -> ";
		}

		// Элемент не найден
		return { i, "" };
	}

	// Прибавляет i к d с переполнением в max, после чего прибавляет add
	uint addWithOverflow(uint i, uint d, uint max, uint add) {
		if (d >= max) {
			d = d % max;
		}
		if (max - i <= d) {
			return i - (max - d) + add;
		}
		return i + d;
	}

public:

	// Возвращает значение элемента хеш-таблицы по ключу или пустую строку
	string get(const string& key) {

		// Хешируем строку в цифру и хешируем цифру
		uint i = hash(key, key.size());

		cout << "Searching for " << key << " at " << i << " -> ";

		// Находим элемент по ключу
		auto pair = getPair(i, key);

		// Элемент не найден
		if (pair.second.empty()) {
			cout << "FAILED" << endl;
			return "";
		}

		cout << pair.first << endl;

		// Элемент найден, возвращаем значение
		return pair.second;
	}

	// Добавляет элемент в хеш-таблицу с указанным ключем и значением
	// Значение не может быть пустой строкой
	bool put(const string& key, const string& value) {
		if (value.empty() || table.size() >= MAX_TABLE_SIZE) {
			return false;
		}

		// Хешируем строку в цифру и хешируем цифру
		uint i = hash(key, key.size());

		cout << "Inserting " << key << " at " << i << " -> ";

		// Находим место для вставки элемента
		auto pair = getPair(i, key);

		// Место не найдено
		if (!pair.second.empty()) {
			cout << "FAILED" << endl;
			return false;
		}

		cout << pair.first << endl;

		// Место найдено, вставляем элемент
		table.insert({ pair.first, { key, value } });

		return true;
	}
};

int main(int arg, char *argv[]) {

	// Максимальная заполненность берется из параметров запуска
	const double MAX_FILL = !argv[1] ? MAX_FILL_DEFAULT : double(std::stoi(argv[1])) / 100;
	MAX_TABLE_SIZE = int(double(MAX_HASH_VALUE)*MAX_FILL) - 1;

	// Вывод в файл вместо консоли
	freopen("output.txt", "w", stdout);

	string filename = "input.txt";
	HashMap hashMap;
	string key;
	fstream info("info.txt", std::ios_base::app);

	// Вносим данные в таблицу
	fstream f1(filename);
	while (f1 >> key) {
		hashMap.put(key, key);
	}
	f1.close();

	cout << endl;

	// Ищем те же данные в таблице, считаем количество успехов и провалов
	fstream f2(filename);
	while (f2 >> key) {
		NUM_ATTEMPTS = 0;
		if (hashMap.get(key).empty()) {
			NUM_FAIL++;
			NUM_ATTEMPTS_FAIL += NUM_ATTEMPTS;
		}
		else {
			NUM_SUCCESS++;
			NUM_ATTEMPTS_SUCCESS += NUM_ATTEMPTS;
		}
	}
	f2.close();

	// Выводим статистику в отдельный файл
	info << 
		MAX_FILL << ' ' <<
		NUM_SUCCESS << ' ' << 
		NUM_FAIL << ' ' << 
		double(NUM_ATTEMPTS_SUCCESS) / NUM_SUCCESS << ' ' <<
		double(NUM_ATTEMPTS_FAIL) / NUM_FAIL << endl;

	return 0;
}