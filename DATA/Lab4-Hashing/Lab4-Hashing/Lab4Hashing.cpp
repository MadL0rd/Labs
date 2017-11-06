#include <iostream>
#include <map>
#include <string>
#include <fstream>

using std::string;
using std::cin;
using std::cout;
using std::endl;
using std::fstream;

// ������������ �������
typedef unsigned short int uint;
const uint MAX_HASH_VALUE = 1024;
const uint OVERFLOW_VALUE = USHRT_MAX;
const double MAX_FILL_DEFAULT = 0.95;
uint MAX_TABLE_SIZE;

// ����������
int NUM_SUCCESS = 0;
int NUM_FAIL = 0;
int NUM_ATTEMPTS = 0;
int NUM_ATTEMPTS_SUCCESS = 0;
int NUM_ATTEMPTS_FAIL = 0;

// ������� ���-�������
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

// ��������� �� ������ ������ ������ c++ ������, � ������� ���� � �������� �� 2 �����
union HashKey {
	const char *chars;
	const unsigned short *words;

	HashKey(const string& key) : chars(key.c_str()) {}
};

// ���-�������
class HashMap {
private:
	std::map<uint, HashEntry> table;

	// �������� ������ ��������� ������������ ����, ��� ������������ � ����� ����������� �������
	// �������� ����� ����������
	uint hash(HashKey key, size_t len) {
		uint i = 0;
		size_t halfLen = len / 2;

		// ���������� �� ��� ����� � ������ ������������
		// ��������� ������� ��� ������������
		for (size_t j = 0; j < halfLen; ++j) {
			i = addWithOverflow(i, key.words[j], OVERFLOW_VALUE, 1);
		}

		// ���� � ������ �������� ���-�� ��������, ��������� ��������� ������ � ������
		if (len % 2 != 0) {
			uint d = (uint(key.chars[len - 1]) << 8) + ' ';
			i += addWithOverflow(i, d, OVERFLOW_VALUE, 1);
		}
		return hashNumber(i);
	}

	// �������� ����� ����������
	// https://studopedia.ru/5_159617_metod-umnozheniya.html
	uint hashNumber(uint k){
		uint n = MAX_HASH_VALUE;
		double A = 0.618033; // ������� �������		
		uint h = n*fmod(double(k)*A, 1); // h(k)=floor[n*({k*A})]
		return h;
	}

	// �������� ����� �������� �� ����� ������� � ���������� ����
	std::pair<uint, string> getPair(uint i, const string& key) {
		uint d = 1;

		// ������� ������� �� �����-�����
		while (table.find(i) != table.end()) {
			NUM_ATTEMPTS++;
			HashEntry entry = table.at(i);

			// ���������� ����-������
			if (key == entry.getKey()) {
				// ������� ������
				return { i, entry.getValue() };
			}

			// �������� ������������ ���� � ������ ������������
			i = addWithOverflow(i, d, MAX_HASH_VALUE, 0);
			d = addWithOverflow(d, 2, OVERFLOW_VALUE, 0);

			cout << i << " -> ";
		}

		// ������� �� ������
		return { i, "" };
	}

	// ���������� i � d � ������������� � max, ����� ���� ���������� add
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

	// ���������� �������� �������� ���-������� �� ����� ��� ������ ������
	string get(const string& key) {

		// �������� ������ � ����� � �������� �����
		uint i = hash(key, key.size());

		cout << "Searching for " << key << " at " << i << " -> ";

		// ������� ������� �� �����
		auto pair = getPair(i, key);

		// ������� �� ������
		if (pair.second.empty()) {
			cout << "FAILED" << endl;
			return "";
		}

		cout << pair.first << endl;

		// ������� ������, ���������� ��������
		return pair.second;
	}

	// ��������� ������� � ���-������� � ��������� ������ � ���������
	// �������� �� ����� ���� ������ �������
	bool put(const string& key, const string& value) {
		if (value.empty() || table.size() >= MAX_TABLE_SIZE) {
			return false;
		}

		// �������� ������ � ����� � �������� �����
		uint i = hash(key, key.size());

		cout << "Inserting " << key << " at " << i << " -> ";

		// ������� ����� ��� ������� ��������
		auto pair = getPair(i, key);

		// ����� �� �������
		if (!pair.second.empty()) {
			cout << "FAILED" << endl;
			return false;
		}

		cout << pair.first << endl;

		// ����� �������, ��������� �������
		table.insert({ pair.first, { key, value } });

		return true;
	}
};

int main(int arg, char *argv[]) {

	// ������������ ������������� ������� �� ���������� �������
	const double MAX_FILL = !argv[1] ? MAX_FILL_DEFAULT : double(std::stoi(argv[1])) / 100;
	MAX_TABLE_SIZE = int(double(MAX_HASH_VALUE)*MAX_FILL) - 1;

	// ����� � ���� ������ �������
	freopen("output.txt", "w", stdout);

	string filename = "input.txt";
	HashMap hashMap;
	string key;
	fstream info("info.txt", std::ios_base::app);

	// ������ ������ � �������
	fstream f1(filename);
	while (f1 >> key) {
		hashMap.put(key, key);
	}
	f1.close();

	cout << endl;

	// ���� �� �� ������ � �������, ������� ���������� ������� � ��������
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

	// ������� ���������� � ��������� ����
	info << 
		MAX_FILL << ' ' <<
		NUM_SUCCESS << ' ' << 
		NUM_FAIL << ' ' << 
		double(NUM_ATTEMPTS_SUCCESS) / NUM_SUCCESS << ' ' <<
		double(NUM_ATTEMPTS_FAIL) / NUM_FAIL << endl;

	return 0;
}