/*
* Находит самый длинный простой цикл в полном направленном или ненаправленном графе методом полного перебора.
* Временная сложность - O((n+1)^n!)
*/

#include <iostream>
#include <vector>
#include <string>
#include <cmath>
#include <ctime>
#include <algorithm>

using namespace std;

enum class INPUT {
	CONSOLE, FILE, RANDOM
};

vector<string> INPUT_STRING = { "from console", "from input.txt", "random" };

const INPUT INPUT_FROM = INPUT::RANDOM; // Сбособ ввода ребер
const bool DIRECTED_GRAPH = false;	    // Ребра ориентированные?

const int MAX_RANDOM = 100;
const int MIN_RANDOM = 1;

int NUM_PERMUTATIONS = 0;
int NUM_ACCESSES = 0;

// Структура вершины
struct Node {
	int u;	// Номер вершины
	vector<int> edges; // Длины ребер
};

int n = 0;			// Кол-во вершин графа
vector<Node> graph;	// Граф
vector<int> path;	// Самый длинный цикл
int maxLength = 0;	// Длина самого длинного цикла

// Возвращает случайное число между мин и макс
int getRandomBetween(int min, int max) {
	int n = max - min + 1;
	int remainder = RAND_MAX % n;
	int x;
	do {
		x = rand();
	} while (x >= RAND_MAX - remainder);
	return min + x % n;
}

int main(){
	srand(int(time(0)));

	// Ввод из файла
	if (INPUT_FROM == INPUT::FILE) {
		freopen("input.txt", "r", stdin);
	}

	// Вводим кол-во вершин
	while (n <= 1) {
		cout << "Enter number of nodes in graph (n > 1): ";
		cin >> n;
	};
	if (INPUT_FROM == INPUT::FILE) {
		cout << n << endl;
	}
	cout << "Input is " << INPUT_STRING[static_cast<typename underlying_type<INPUT>::type>(INPUT_FROM)] << endl;

	// Выделяем достаточно место для вершин и ребер графа
	graph.resize(n);
	path.resize(n + 1);
	path[n] = path[0] = 1;
	for (int i = 0; i < n; i++) {
		graph[i].u = i;
		graph[i].edges.resize(n);
	}

	// Проходим по каждой вершине
	for (int u = 0; u < n; u++) {
		// Проходим по каждой вершине в ориентированном графе,
		// или по незаполненным вершинам в неориентированном
		// Наш граф полный по заданию
		for (int v = DIRECTED_GRAPH ? 0 : u + 1; v < n; v++) {
			
			if (u == v) continue; // Пропускаем петли
			int weight;

			// Вводим длины ребер
			if (INPUT_FROM == INPUT::RANDOM) {
				weight = getRandomBetween(MIN_RANDOM, MAX_RANDOM);
				cout << (u + 1) << " -> " << (v + 1) << ": " << weight << endl;
			}
			else {
				cout << (u + 1) << " -> " << (v + 1) << ": ";
				cin >> weight;
				if (INPUT_FROM == INPUT::FILE) {
					cout << weight << endl;
				}
			}

			// Записываем длины
			// Мы представляем неориентированный граф как ориентированный в котором
			// равны длины противоположных ребер
			if (DIRECTED_GRAPH) {
				graph[u].edges[v] = weight;
			}
			else {
				graph[u].edges[v] = graph[v].edges[u] = weight;
			}
		}
	}

	// Нахождение самого длинного простого цикла в полном графе это NP-полная задача
	// Перебираем все возможные циклы и находим самый длинный
	// Мы хотим прийти из стартовой (пусть будет первой) вершины в нее же,
	// поэтому ограничиваем перебор остальными вершинами
	do {
		NUM_PERMUTATIONS++;
		NUM_ACCESSES += 2;

		// Длина от стартовой вершины до следующей за ней (текущей)
		int v = graph[1].u;
		int length = graph[0].edges[v];
		
		// Длина от текущей до следующей
		for (int u = 1; u < n - 1; u++) {
			NUM_ACCESSES++;
			v = graph[u + 1].u;
			length += graph[u].edges[v];
		}

		// Длина от последней до стартовой
		v = 0;
		length += graph[n - 1].edges[v];

		// Нашли более длинный?
		if (length > maxLength) {
			maxLength = length;
			for (int i = 1; i < n; i++) {
				path[i] = graph[i].u + 1;
			}
		}
	} while (
		next_permutation(
			graph.begin() + 1, 
			graph.end(), 
			[](Node a, Node b) {
				return a.u < b.u;
			}
		)
	);

	// Выводим статистику
	cout << (DIRECTED_GRAPH ? "Directed" : "Undirected") << " graph with " << n << " nodes and " << (DIRECTED_GRAPH ? n * (n - 1) : n * (n - 1) / 2) << " edges" << endl;
	cout << "Required " << NUM_PERMUTATIONS << " permutations and ~" << NUM_ACCESSES << " accesses" << endl;

	// Выводим самый длинный цикл
	cout << "Longest simple cycle (" << maxLength << "): ";
	for (auto i = path.begin(); i != path.end(); ++i) {
		std::cout << *i << ' ';
	}
	cout << endl;

    return 0;
}

