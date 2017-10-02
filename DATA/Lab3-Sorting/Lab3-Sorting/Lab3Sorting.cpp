// Lab3 - Алгоритмы сортировки

#include <iostream>
#include <vector>
#include <chrono>

using std::cin;
using std::cout;
using std::endl;
using std::vector;
using std::pair;
using std::make_pair;
using std::swap;

using std::chrono::high_resolution_clock;
using std::chrono::duration_cast;
using std::chrono::milliseconds;

// Статистика сортировки - <кол-во сравнений, кол-во перестановок>
typedef pair<int, int> Stats;

// Делает указанную часть массива пирамидальной с максимальным элементом по индексу root
Stats heapify(vector<int>& arr, int root, int n) {

	int numSwaps = 0, numComp = 0;

	int newRoot = root;	   // Новый корень - наибольший элемент
	int l = 2 * root + 1;  // Левая ветка
	int r = 2 * root + 2;  // Правая ветка

	// Левая ветка больше корня
	if (l < n && arr[l] > arr[newRoot]) {
		newRoot = l;
	}
	++numComp;

	// Правая ветка больше корня\левой ветки
	if (r < n && arr[r] > arr[newRoot]) {
		newRoot = r;
	}
	++numComp;

	// Если корень изменился
	if (newRoot != root) {

		// Заменяем новый корень со старым
		swap(arr[root], arr[newRoot]);
		++numSwaps;

		// Идем дальше по дереву
		Stats stats = heapify(arr, newRoot, n);
		numComp += stats.first;
		numSwaps += stats.second;
	}
	// Иначе сортировка закончена
	return make_pair(numComp, numSwaps);
}

// Сортирует массив по алгоритму Heap Sort
Stats heapsort(vector<int>& arr) {

	int numSwaps = 0, numComp = 0;

	// Делаем массив пирамидальным c наибольшим элементом в корне (индекс 0)
	for (int i = (arr.size() - 1) / 2 - 1; i >= 0; i--) {
		Stats stats = heapify(arr, i, arr.size() - 1);
		numComp += stats.first;
		numSwaps += stats.second;
	}

	for (int i = arr.size() - 1; i >= 0; i--) {

		// Ставим корень (наибольший элемент) в начало отсортированной части массива
		swap(arr[0], arr[i]);
		++numSwaps;

		// Восстанавливаем пирамидальность оставшегося массива
		Stats stats = heapify(arr, 0, i);
		numComp += stats.first;
		numSwaps += stats.second;
	}

	return make_pair(numComp, numSwaps);
}

// Сортирует массив по алгоритму Selection Sort
Stats selectionSort(vector<int>& arr) {
	int numSwaps = 0, numComp = 0;

	// Ну я думаю не надо объяснять как это работает
	for (int i = 0; i < arr.size() - 1; i++) {
		int minVal = arr[i];
		int minIdx = i;
		for (int j = i + 1; j < arr.size(); j++) {
			if (minVal > arr[j]) {
				minVal = arr[j];
				minIdx = j;
			}
			numComp++;
		}
		swap(arr[i], arr[minIdx]);
		numSwaps++;
	}

	return make_pair(numComp, numSwaps);
}

// Выводит массив в stdout
void printArray(vector<int>& arr) {
	for (int i = 0; i < arr.size(); i++) {
		cout << arr[i] << ' ';
	}
}

// Функция для демонстрации сортировки
int main() {
	freopen("input.txt", "r", stdin);
	vector<int> arr1;
	vector<int> arr2;
	int m;
	while (cin >> m) {
		arr1.push_back(m);
		arr2.push_back(m);
	}

	auto t1 = high_resolution_clock::now();
	Stats stats = heapsort(arr1);
	auto t2 = high_resolution_clock::now();

	freopen("output1.txt", "w", stdout);
	cout << 
		"Heap sort" << endl <<
		"Time elapsed: " << duration_cast<milliseconds>(t2 - t1).count() << endl <<
		"Comparations: " << stats.first << endl <<
		"Swaps: " << stats.second << endl;
	printArray(arr1);

	t1 = high_resolution_clock::now();
	stats = selectionSort(arr2);
	t2 = high_resolution_clock::now();

	freopen("output2.txt", "w", stdout);
	cout <<
		"Selection sort" << endl <<
		"Time elapsed: " << duration_cast<milliseconds>(t2 - t1).count() << endl <<
		"Comparations: " << stats.first << endl <<
		"Swaps: " << stats.second << endl;
	printArray(arr2);
}
