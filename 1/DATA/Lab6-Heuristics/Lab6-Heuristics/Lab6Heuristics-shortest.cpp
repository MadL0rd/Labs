/*
* Находит самый длинный простой цикл в полном направленном или ненаправленном графе методом полного перебора.
*/

#include <iostream>
#include <vector>
#include <string>
#include <cmath>
#include <ctime>
#include <algorithm>
#include <chrono>

using namespace std;

using chrono::high_resolution_clock;
using chrono::duration_cast;
using chrono::milliseconds;

enum class INPUT {
	CONSOLE, FILE, RANDOM
};

vector<string> INPUT_STRING = { "from console", "from input.txt", "random" };

const INPUT INPUT_FROM = INPUT::RANDOM;			// Сбособ ввода ребер
const INPUT TEST_INPUT_FROM = INPUT::CONSOLE;	// Способ ввода настроек тестов
const INPUT OUTPUT_TO = INPUT::CONSOLE;			// Способ вывода
const bool DIRECTED_GRAPH = false;				// Ребра ориентированные?

const int MAX_RANDOM = 100;
const int MIN_RANDOM = 1;

struct Edge {
	int v;
	int weight;
};

// Структура вершины
struct Node {
	int u;	// Номер вершины
	vector<Edge> edges; // Длины ребер
};

// Информация о цикле
struct Cycle {
	int length;
	int numAccesses;
	int numPermutations;
	vector<int> path;
};

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

// Возвращает один из длинных циклов
Cycle estimateLongestCycle(vector<Node> graph, int n) {
	int NUM_PERMUTATIONS = 0;
	int NUM_ACCESSES = 0;

	int maxLength = 0;
	vector<int> path;

	vector<int> backWeights; // Длины ребер, идущих в вершину начала цикла

	int maxWeight = INT_MIN;
	int from;

	// Начинаем с самого длинного ребра
	for (int u = 0; u < n; u++) {
		for (int v = 0; v < graph[u].edges.size(); v++) {
			if (u == v) continue;

			NUM_ACCESSES++;
			int curWeight = graph[u].edges[v].weight;
			if (maxWeight < curWeight) {
				from = u;
				maxWeight = curWeight;
			}
		}
	}

	// Добавляем первую вершину в путь
	path.push_back(from);
	// Запоминаем длины ребер, идущих в первую вершину
	for (int u = 0; u < n; u++) {
		backWeights.push_back(graph[u].edges[from].weight);
	}

	// Находим цикл, удаляя используемые вершины из графа
	while (graph.size() > 1) {

		int maxWeight = INT_MIN;
		int to;

		// Находим самое длинное ребро из текущей вершины
		for (int i = 0; i < graph[from].edges.size(); i++) {
			if (from == i) continue;

			NUM_ACCESSES++;
			int curWeight = graph[from].edges[i].weight;
			if (maxWeight < curWeight) {
				to = graph[from].edges[i].v;
				maxWeight = curWeight;
			}
		}

		// Идем по нему
		maxLength += maxWeight;
		path.push_back(to);

		NUM_ACCESSES++;

		// Удаляем вершину из которой мы идем и все ведущие в нее ребра
		graph.erase(graph.begin() + from);

		for (int i = 0; i < graph.size(); i++) {
			NUM_ACCESSES++;
			graph[i].edges.erase(graph[i].edges.begin() + from);
		}		

		// Находим индекс вершины, в которую мы пришли в измененном графе
		for (int i = 0; i < graph.size(); i++) {
			NUM_ACCESSES++;
			if (graph[i].u == to) {
				from = i;
				break;
			}
		}		
	}

	NUM_ACCESSES++;
	
	// Завершаем цикл
	maxLength += backWeights[graph[0].u];
	path.push_back(path[0]);
	
	return { maxLength, NUM_ACCESSES, NUM_PERMUTATIONS, path };
}

// Возвращает самый длинный цикл
Cycle getLongestCycle(vector<Node> graph, int n) {
	int NUM_PERMUTATIONS = 0;
	int NUM_ACCESSES = 0;

	int maxLength = 0;
	vector<int> path;
	do {
		NUM_PERMUTATIONS++;
		NUM_ACCESSES += 2;

		// Длина от стартовой вершины до следующей за ней (текущей)
		int u = 0;
		int v = graph[1].u;
		int length = graph[u].edges[v].weight;

		// Длина от текущей до следующей
		for (u = 1; u < n - 1; u++) {
			NUM_ACCESSES++;
			v = graph[u + 1].u;
			length += graph[u].edges[v].weight;
		}

		// Длина от последней до стартовой
		u = n - 1;
		v = 0;
		length += graph[u].edges[v].weight;

		// Нашли более длинный?
		if (length > maxLength) {
			maxLength = length;
			path.resize(n + 1);
			path[n] = path[0] = 1;
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
	return { maxLength, NUM_ACCESSES, NUM_PERMUTATIONS, path };
}

Cycle getShortestCycle(vector<Node> graph, int n) {
	int NUM_PERMUTATIONS = 0;
	int NUM_ACCESSES = 0;

	int maxLength = 0;
	vector<int> path;
	do {
		NUM_PERMUTATIONS++;
		NUM_ACCESSES += 2;

		// Длина от стартовой вершины до следующей за ней (текущей)
		int u = 0;
		int v = graph[1].u;
		int length = graph[u].edges[v].weight;

		// Длина от текущей до следующей
		for (u = 1; u < n - 1; u++) {
			NUM_ACCESSES++;
			v = graph[u + 1].u;
			length += graph[u].edges[v].weight;
		}

		// Длина от последней до стартовой
		u = n - 1;
		v = 0;
		length += graph[u].edges[v].weight;

		// Нашли более длинный?
		if (length < maxLength) {
			maxLength = length;
			path.resize(n + 1);
			path[n] = path[0] = 1;
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
	return { maxLength, NUM_ACCESSES, NUM_PERMUTATIONS, path };
}

// Выводит статистику цикла
void outputResults(int n, Cycle cycle, long long int duration) {
	// Выводим статистику
	cout << (DIRECTED_GRAPH ? "Directed" : "Undirected") << " graph with " << n << " nodes and " << (DIRECTED_GRAPH ? n * (n - 1) : n * (n - 1) / 2) << " edges" << endl;
	cout << "Required " << cycle.numPermutations << " permutations, ~" << cycle.numAccesses << " accesses and " << duration << "ms" << endl;

	// Выводим самый длинный цикл
	cout << "Longest simple cycle (" << cycle.length << "): ";
	for (auto i = cycle.path.begin(); i != cycle.path.end(); ++i) {
		std::cout << (*i) + 1 << ' ';
	}
	cout << endl;
}

vector<double> results;

// Вводит граф и находит длинный цикл в нем разными способами, после чего выводит статистику и сравнения
bool runTest(int n, bool enableOutput, bool enableComparison) {
	vector<Node> graph;	// Граф
	vector<int> path;	// Самый длинный цикл
	int maxLength = 0;	// Длина самого длинного цикла

						// Выделяем достаточно место для вершин и ребер графа
	graph.resize(n);
	for (int i = 0; i < n; i++) {
		graph[i].edges.resize(n);
		graph[i].u = i;
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
				if (enableOutput) {
					//cout << (u + 1) << " -> " << (v + 1) << ": " << weight << endl;
				}
			}
			else {
				if (enableOutput) {
					//cout << (u + 1) << " -> " << (v + 1) << ": ";
				}
				cin >> weight;
				if (enableOutput && INPUT_FROM == INPUT::FILE) {
					cout << weight << endl;
				}
			}

			// Записываем длины
			// Мы представляем неориентированный граф как ориентированный в котором
			// равны длины противоположных ребер
			if (DIRECTED_GRAPH) {
				graph[u].edges[v] = { v, weight };
			}
			else {
				graph[u].edges[v] = { v, weight };
				graph[v].edges[u] = { u, weight };
			}
		}
	}

	// Один из длинных циклов
	auto estimatedTime1 = high_resolution_clock::now();
	auto estimatedCycle = estimateLongestCycle(graph, n);
	auto estimatedTime2 = high_resolution_clock::now();

	bool matched = false;

	// Копируем, чтобы можно было использовать auto (лень одолела)
	auto exactTime1 = estimatedTime1;
	auto exactTime2 = estimatedTime1;
	auto exactCycle = estimatedCycle;
	if (enableComparison) {

		// Самый длинный цикл
		exactTime1 = high_resolution_clock::now();
		exactCycle = getLongestCycle(graph, n);
		exactTime2 = high_resolution_clock::now();

		auto shortestCycle = getShortestCycle(graph, n);

		// Такого не должно быть
		if (estimatedCycle.length > exactCycle.length) {
			throw(logic_error("Estimated was better than exact"));
		}

		double dif = exactCycle.length - shortestCycle.length;
		double estimated = estimatedCycle.length - shortestCycle.length;

		double percentage = estimated / dif;

		results.push_back(percentage);

		// Примерный совпал с самым длинным?
		matched = (estimatedCycle.length == exactCycle.length);

		cout << "(" << exactCycle.length << " <-> " << estimatedCycle.length << ")" << endl;
	}

	// Выводим результаты
	if (enableOutput) {
		if (enableComparison) {
			cout << (matched ? "Matched" : "Worse") << endl << endl;

			cout << "Exact" << endl;
			outputResults(n, exactCycle, duration_cast<milliseconds>(exactTime2 - exactTime1).count());
			cout << "Estimated" << endl;
		}
		outputResults(n, estimatedCycle, duration_cast<milliseconds>(estimatedTime2 - estimatedTime1).count());
	}

	return matched;
}

// Сравнение полного перебора с жадным алгоритмом
void runCompareTest(int m, int n) {
	int results = 0;
	for (int i = 0; i < m; i++) {
		cout << "Test " << i + 1 << ' ';
		results += runTest(n, false, true);
	}

	cout << "Estimated matched exact in " << (double(results) / double(m) * 100) << "% of the tests (" << results << "/" << m << ")" << endl;
}

// Тестирование жадного алгоритма
void runSpeedTest(int n) {
	runTest(n, true, false);
}

int main() {
	srand(int(time(0)));
	
	if (OUTPUT_TO == INPUT::FILE) {
		freopen("output.txt", "w", stdout);
	}

	if (INPUT_FROM == INPUT::FILE || TEST_INPUT_FROM == INPUT::FILE) {
		freopen("input.txt", "r", stdin);
	}

	cout << "Input is " << INPUT_STRING[static_cast<typename underlying_type<INPUT>::type>(INPUT_FROM)] << endl;
	cout << "Test input is " << INPUT_STRING[static_cast<typename underlying_type<INPUT>::type>(TEST_INPUT_FROM)] << endl << endl;

	// Сравнение полного перебора с жадным алгоритмом
	cout << "Comparing estimated vs exact results" << endl;
	int m = 0, n = 0;
	while (true) {
		cout << "Enter number of tests (m > 0): ";
		cin >> m;
		if (INPUT_FROM == INPUT::FILE || TEST_INPUT_FROM == INPUT::FILE) {
			cout << m << endl;
		}
		cout << "Enter number of nodes in graph (n > 2): ";
		cin >> n;
		if (INPUT_FROM == INPUT::FILE || TEST_INPUT_FROM == INPUT::FILE) {
			cout << n << endl;
		}
		if (m < 1 || n < 3) break;

		runCompareTest(m, n);

		double total = 0;
		for (int i = 0; i < results.size(); i++) {
			total += results[i];
		}
		total /= results.size();
		cout << endl << endl << total << endl << endl;

		results.clear();
	}

	// Тестирование жадного алгоритма
	cout << "Speed test" << endl;
	n = 0;
	while (true) {
		cout << "Enter number of nodes in graph (n > 2): ";
		cin >> n;
		if (INPUT_FROM == INPUT::FILE || TEST_INPUT_FROM == INPUT::FILE) {
			cout << n << endl;
		}
		if (n < 3) break;

		runSpeedTest(n);
	}
	return 0;
}

