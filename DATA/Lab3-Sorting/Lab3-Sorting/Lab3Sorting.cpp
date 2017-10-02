// Lab3 - ��������� ����������

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

// ���������� ���������� - <���-�� ���������, ���-�� ������������>
typedef pair<int, int> Stats;

// ������ ��������� ����� ������� ������������� � ������������ ��������� �� ������� root
Stats heapify(vector<int>& arr, int root, int n) {

	int numSwaps = 0, numComp = 0;

	int newRoot = root;	   // ����� ������ - ���������� �������
	int l = 2 * root + 1;  // ����� �����
	int r = 2 * root + 2;  // ������ �����

	// ����� ����� ������ �����
	if (l < n && arr[l] > arr[newRoot]) {
		newRoot = l;
	}
	++numComp;

	// ������ ����� ������ �����\����� �����
	if (r < n && arr[r] > arr[newRoot]) {
		newRoot = r;
	}
	++numComp;

	// ���� ������ ���������
	if (newRoot != root) {

		// �������� ����� ������ �� ������
		swap(arr[root], arr[newRoot]);
		++numSwaps;

		// ���� ������ �� ������
		Stats stats = heapify(arr, newRoot, n);
		numComp += stats.first;
		numSwaps += stats.second;
	}
	// ����� ���������� ���������
	return make_pair(numComp, numSwaps);
}

// ��������� ������ �� ��������� Heap Sort
Stats heapsort(vector<int>& arr) {

	int numSwaps = 0, numComp = 0;

	// ������ ������ ������������� c ���������� ��������� � ����� (������ 0)
	for (int i = (arr.size() - 1) / 2 - 1; i >= 0; i--) {
		Stats stats = heapify(arr, i, arr.size() - 1);
		numComp += stats.first;
		numSwaps += stats.second;
	}

	for (int i = arr.size() - 1; i >= 0; i--) {

		// ������ ������ (���������� �������) � ������ ��������������� ����� �������
		swap(arr[0], arr[i]);
		++numSwaps;

		// ��������������� ��������������� ����������� �������
		Stats stats = heapify(arr, 0, i);
		numComp += stats.first;
		numSwaps += stats.second;
	}

	return make_pair(numComp, numSwaps);
}

// ��������� ������ �� ��������� Selection Sort
Stats selectionSort(vector<int>& arr) {
	int numSwaps = 0, numComp = 0;

	// �� � ����� �� ���� ��������� ��� ��� ��������
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

// ������� ������ � stdout
void printArray(vector<int>& arr) {
	for (int i = 0; i < arr.size(); i++) {
		cout << arr[i] << ' ';
	}
}

// ������� ��� ������������ ����������
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
