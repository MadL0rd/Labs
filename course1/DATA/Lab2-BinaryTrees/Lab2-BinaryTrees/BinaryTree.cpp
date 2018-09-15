#include "BinaryTree.h"
#include <fstream>
#include <sstream>
#include <vector>

using std::cin;
using std::cout;
using std::endl;
using std::ifstream;
using std::string;
using std::getline;
using std::stoi;
using std::vector;

// Создание пустого дерева
BinaryTree::BinaryTree() {}

// Создание дерева из контента файла
BinaryTree::BinaryTree(const string& fileName) {
	addFromFile(fileName);
}

// Деструктор дерева
BinaryTree::~BinaryTree() {
	if (root != nullptr) {
		deleteTree(root);
	}
}

// Создает новый корень дерева с указанным значением, предварительно удаляя предыдыщие
void BinaryTree::addAsRoot(int value) {
	if (root != nullptr) {
		deleteTree(root);
	}
	root = new Node{ value };
}

// Добавляет новую ветку дерева с указанным значением к указанной векте
Node* BinaryTree::addTo(Node* node, BRANCH branch, int value) {
	if (branch == BRANCH::NEXT) {
		return nullptr;
	}
	Node* newNode = new Node{ value };
	if (branch == BRANCH::LEFT) {
		node->left = newNode;
	}
	else if (branch == BRANCH::RIGHT) {
		node->right = newNode;
	}
	return newNode;
}

// Добавляет новую ветку к указанному ноду и удаляет emptyNode из веток нода,
// если из line удалось получить значение, иначе добавляет emptyNode, как новую ветку
Node* BinaryTree::addTo(Node* node, BRANCH branch, const string& line, Node* emptyNode) {
	try {
		int n = stoi(line);
		removeNodeShallow(node, emptyNode);
		return addTo(node, branch, n);
	}
	catch (...) {
		if (branch == BRANCH::LEFT) {
			node->left = emptyNode;
		}
		else if (branch == BRANCH::RIGHT) {
			node->right = emptyNode;
		}
		return nullptr;
	}
}

/*
* Построчно добавляет ветки из файла.
* Значения веток - положительные или отрицательные целые числа.
* Каждая новая строка должна иметь правильное кол-во отступов, соответствующих глубине ветки.
* Первая строка - корень дерева.
* Отступ берется из второй строки.
* Добавляется сначала левая, затем правая ветки.
* Чтобы пропустить левую ветку, нужно ввести строку с отступами, но без значения.
* Правую ветку и пустые ветки можно не писать.
* Метод выводит сообщения в консоль, если формат файла невалиден.
* Примеры валидных контентов файлов:
10
25
100
15
2
88
99
1000
500
*
1
~-4
~~4
~5
~~6
~~~
~~~100
~~7
~~~8
~~~9
~~~~10
~~~~11
*/
void BinaryTree::addFromFile(const string& fileName) {

	// Открываем файл
	ifstream file(fileName);

	if (!file) {
		cout << "Coudn't open file " << fileName << endl;
		return;
	}

	string error = "Error parsing file " + fileName + ": ";

	// Считываем и добавляем корень дерева
	int n;
	string line;
	getline(file, line);
	try {
		n = stoi(line);
	}
	catch (...) {
		cout << error << "No root provided" << endl;
		return;
	}
	addAsRoot(n);

	Node* curNode = root;		// Текущий узел дерева
	Node* emptyNode = new Node;	// Пустой узел, для запоминания веток, в которые было добавлено пустое значение
	int curLevel = 1;			// Текущая глубина (высота, отступ) дерева
	string indentator;			// Строка отступа

	for (int i = 1; getline(file, line); i++) {

		// Находим строку отступа
		if (i == 1) {
			indentator = extractIndenter(line);

			if (indentator.length() == 0) {
				cout << error << "No indentator found on line " << i + 1 << endl;
				deleteTree(root);
				break;
			}
		}

		// Пустая ветка
		BRANCH branch = getEmptyBranch(curNode);

		// Текущий уровень отступа в файле
		int indentLevel = countSubstring(line, indentator);

		// Вводим ветку на текущем уровне
		if (indentLevel == curLevel) {

			// Лишняя ветка
			if (branch == BRANCH::NEXT) {
				cout << error << "Extra branch on line " << i + 1 << endl;
				removeNodeShallow(curNode, emptyNode);
				deleteTree(root);
				break;
			}

			// Вытаскиваем значение из строки и если значение удалось вытащить,
			// добавляем новый узел к дереву и увеличиваем уровень отступа,
			// иначе добавляем ссылку на пустой узел (чтобы запомнить, что сюда не нужно добавлять)
			Node* newNode = addTo(curNode, branch, line.substr(indentator.length()*indentLevel), emptyNode);
			if (newNode != nullptr) {
				curLevel++;
				curNode = newNode;
			}
		}
		// Переходим на нижний уровень
		else if (indentLevel < curLevel) {
			// Сдвигаемся вниз по уровням, удаляя все пустые узлы,
			// пока не дойдем до корня или не найдем ветку, в которую нужно добавить новый узел
			do {
				removeNodeShallow(curNode, emptyNode);
				curLevel--;
				curNode = getBranchWith(curNode);
				if (curNode == nullptr) {
					break;
				}
				branch = getEmptyBranch(curNode);
			} while (curLevel != indentLevel || branch == BRANCH::NEXT);

			// Дошли до корня и у него уже есть две ветки
			if (curNode == nullptr) {
				cout << error << "No parent node on line " << i + 1 << endl;
				deleteTree(root);
				break;
			}

			// Вытаскиваем значение из строки и если значение удалось вытащить,
			// добавляем новый узел к дереву и увеличиваем уровень отступа,
			// иначе добавляем ссылку на пустой узел (чтобы запомнить, что сюда не нужно добавлять)
			Node* newNode = addTo(curNode, branch, line.substr(indentator.length()*indentLevel), emptyNode);
			if (newNode != nullptr) {
				curLevel++;
				curNode = newNode;
			}
		}
		// Слишком много отступов в файле
		else {
			cout << error << "Too much indentation on line " << i + 1 << endl;
			removeNodeShallow(curNode, emptyNode);
			deleteTree(root);
			break;
		}
	}

	// Удаляем ссылки на пустые узлы из последнего узла
	removeNodeShallow(curNode, emptyNode);

	delete emptyNode;
}

// Возвращает левую или правую ветку, если одна из них пуста или BRANCH::NEXT, если обе заполнены 
BRANCH BinaryTree::getEmptyBranch(Node* node) {
	return node->left == nullptr ? BRANCH::LEFT : (node->right == nullptr ? BRANCH::RIGHT : BRANCH::NEXT);
}

// Возвращает родительскую ветку
Node* BinaryTree::getBranchWith(Node* node) {
	return getBranchWith(node, root);
}

// Рекурсивно находит и возвращает ветку, которая является родетелем указанной
Node* BinaryTree::getBranchWith(Node* node, Node* curNode) {
	if (curNode->left == node || curNode->right == node) {
		return curNode;
	}
	Node* parentNode;
	if (curNode->left != nullptr) {
		parentNode = getBranchWith(node, curNode->left);
		if (parentNode != nullptr) {
			return parentNode;
		}
	}
	if (curNode->right != nullptr) {
		parentNode = getBranchWith(node, curNode->right);
		if (parentNode != nullptr) {
			return parentNode;
		}
	}
	return nullptr;
}

// Возвращает все символы строки до первой цифры или символа `-`
string BinaryTree::extractIndenter(const string& line) {
	string indentator = "";
	for (size_t i = 0; i < line.length(); i++) {
		if (isdigit(line[i]) || line[i] == '-') {
			break;
		}
		indentator += line[i];
	}
	return indentator;
}

// Считает кол-во вхождений второй строки в первую
int BinaryTree::countSubstring(const std::string& str, const std::string& sub) {
	if (sub.length() == 0) {
		return 0;
	}
	int count = 0;
	for (
		size_t offset = str.find(sub);
		offset != std::string::npos;
		offset = str.find(sub, offset + sub.length())
		) {
		++count;
	}
	return count;
}

// Удаляет нод, если он является веткой указанного нода
void BinaryTree::removeNodeShallow(Node* node, Node* nodeToRemove) {
	if (node == nullptr) {
		return;
	}
	if (node->left == nodeToRemove) {
		node->left = nullptr;
	}
	if (node->right == nodeToRemove) {
		node->right = nullptr;
	}
};


// Рекурсивно удаляет все векти дерева
void BinaryTree::deleteTree(Node*& node) {
	if (node->left != nullptr) {
		deleteTree(node->left);
	}
	if (node->right != nullptr) {
		deleteTree(node->right);
	}
	delete node;
	node = nullptr;
}

// Выводит отображение дерева в консоль с `  ` в качестве отступов
void BinaryTree::print() {
	print("  ");
}

// Выводит отображение дерева в консоль с указанной строкой в качестве отступов
void BinaryTree::print(const string& indentator) {
	int level = 0;
	if (root == nullptr) {
		cout << "Tree is empty" << endl;
		return;
	}
	print(root, level, indentator);
}

// Рекурсивно выводит значения веток в консоль с указанным отступом
void BinaryTree::print(Node* node, int level, const string& indentator) {
	printIndentedValue(node->value, level, indentator);
	level++;
	if (node->left) {
		print(node->left, level, indentator);
	}
	else {
		printIndentedValue('x', level, indentator);
	}
	if (node->right) {
		print(node->right, level, indentator);
	}
	else {
		printIndentedValue('x', level, indentator);
	}
}

// Выводит отступ указанное кол-во раз и печатает символ
void BinaryTree::printIndentedValue(char value, int level, const string& indentator) {
	while (level--) {
		cout << indentator;
	}
	cout << value << endl;
}

// Выводит отступ указанное кол-во раз и печатает целое число
void BinaryTree::printIndentedValue(int value, int level, const string& indentator) {
	while (level--) {
		cout << indentator;
	}
	cout << value << endl;
}

// Возвращает отсортировано ли дерево (рекурсивный метод)
bool BinaryTree::isSorted() {
	return isSorted(root, nullptr, nullptr);
}

// Рекурсивно проверяет, отсортирован ли узел и все его ветки
bool BinaryTree::isSorted(Node* node, Node* left, Node* right) {
	if (node == nullptr) {
		return true;
	}
	if (left != nullptr && node->value < left->value) {
		return false;
	}
	if (right != nullptr && node->value > right->value) {
		return false;
	}
	return (
		isSorted(node->left, left, node) &&
		isSorted(node->right, node, right)
	);
}

// Возвращает отсортировано ли дерево (нерекурсивный метод)
bool BinaryTree::isSortedLinear() {
	if (root == nullptr) {
		return true;
	}
	vector<Node*> stackMax;	// Стек частично пройденных веток\веток с максимальными значениями
	vector<Node*> stackMin;	// Стек пар веток: ветка с минимальным значением и правой подветкой этой ветки
	Node* cur = root;		// Текущая ветка (узел)
	Node* curMin = nullptr;	// Ветка с локальным минимальным значением
	bool goLeft = true;		// Нужно ли идти налево (не была ли пройдена левая ветка)

	// Проходим дерево и проверяем отсортированность каждой ветки начиная с корня
	while(true){
		// Идем по левой ветке, если она есть и по ней еще не проходили
		if (cur->left != nullptr && goLeft) {

			// Запоминаем ветку с локальным минимальным значением и ее правую ветку
			if (curMin != nullptr) {
				stackMin.push_back(curMin);
				curMin = nullptr;
			}

			// Сверям значение с родительской веткой и текущим минимумом
			if (cur->left->value > cur->value || stackMin.size() != 0 && cur->left->value < stackMin.back()->value) {
				return false;
			}
			
			// Запоминаем текущую ветку как неполностью пройденную и как локальное максимальное значение
			stackMax.push_back(cur);

			// Двигаемся влево
			cur = cur->left;
		}
		// Идем по правой ветке, если она есть
		else if (cur->right != nullptr) {

			// Сверям значение с родительской веткой и текущим максимумом
			if (cur->right->value < cur->value || stackMax.size() != 0 && cur->right->value > stackMax.back()->value) {
				return false;
			}

			// Запоминаем текущую ветку, как локальный минимум
			curMin = cur;

			// Двигаемся вправо
			cur = cur->right;

			// После движения вправо мы хотим двигаться влево
			goLeft = true;
		}
		// Если ни одной ветки нет, идем вверх к последней ветке с непройденной правой подветкой
		else{
			// Мы прошли все ветки
			if (stackMax.size() == 0) {
				break;
			}

			// Мы хотим идти по правой ветке
			goLeft = false;

			// Идем назад
			cur = stackMax.back();
			stackMax.pop_back();

			// Если текущая ветка является правой веткой локального минимума,
			// забываем этот локальный минимум
			if (stackMin.size() != 0 && stackMin.back()->right == cur) {
				stackMin.pop_back();
			}
		}
	}
	return true;
}
