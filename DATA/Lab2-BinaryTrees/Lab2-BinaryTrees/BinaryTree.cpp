#include "BinaryTree.h"
#include <fstream>
#include <sstream>

using std::cin;
using std::cout;
using std::endl;
using std::ifstream;
using std::string;
using std::getline;
using std::stoi;

// �������� ������� ������
BinaryTree::BinaryTree() {}

// �������� ������ �� �������� �����
BinaryTree::BinaryTree(const string& fileName) {
	addFromFile(fileName);
}

// ���������� ������
BinaryTree::~BinaryTree() {
	deleteTree(root);
}

// ���������� ������� ��� ����� ������
void BinaryTree::deleteTree(Node* node) {
	if (node->left != nullptr) {
		deleteTree(node->left);
	}
	if (node->right != nullptr) {
		deleteTree(node->right);
	}
	delete node;
}

// ������� ����� ������ ������ � ��������� ���������, �������������� ������ ����������
void BinaryTree::addAsRoot(int value) {
	if (root != nullptr) {
		deleteTree(root);
	}
	root = new Node{ value };
}

// ��������� ����� ����� ������ � ��������� ��������� � ��������� �����
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

// ��������� ����� ����� � ��������� �����, ���� �� line ������� �������� ��������,
// ����� ��������� emptyNode, ��� ����� �����
Node* BinaryTree::addTo(Node* node, BRANCH branch, const string& line, Node* emptyNode) {
	try {
		int n = stoi(line);
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
* ��������� ��������� ����� �� �����.
* �������� ����� - ������������� ��� ������������� ����� �����.
* ������ ����� ������ ������ ����� ���������� ���-�� ��������, ��������������� ������� �����.
* ������ ������ - ������ ������.
* ������ ������� �� ������ ������.
* ����������� ������� �����, ����� ������ �����.
* ����� ���������� ����� �����, ����� ������ ������ � ���������, �� ��� ��������.
* ������ ����� � ������ ����� ����� �� ������.
* ����� ������� ��������� � �������, ���� ������ ����� ���������.
* ������� �������� ��������� ������:
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

	// ��������� ����
	ifstream file(fileName);

	if (!file) {
		cout << "Coudn't open file" << endl;
		return;
	}

	// ��������� � ��������� ������ ������
	int n;
	string line;
	getline(file, line);
	try {
		n = stoi(line);
	}
	catch (...) {
		cout << "No root provided" << endl;
		return;
	}
	addAsRoot(n);

	Node* curNode = root;		// ������� ���� ������
	Node* emptyNode = new Node;	// ������ ����, ��� ����������� �����, � ������� ���� ��������� ������ ��������
	int curLevel = 1;			// ������� ������� (������, ������) ������
	string indentator;			// ������ �������

	for (int i = 1; getline(file, line); i++) {

		// ������� ������ �������
		if (i == 1) {
			indentator = extractIndenter(line);

			if (indentator.length() == 0) {
				cout << "No indentator found" << endl;
				break;
			}
		}

		// ������ �����
		BRANCH branch = getEmptyBranch(curNode);

		// ������� ������� ������� � �����
		int indentLevel = countSubstring(line, indentator);

		// ������ ����� �� ������� ������
		if (indentLevel == curLevel) {

			if (branch == BRANCH::NEXT) {
				cout << "Extra branch" << endl;
				break;
			}

			// ����������� �������� �� ������ � ���� �������� ������� ��������,
			// ��������� ����� ���� � ������ � ����������� ������� �������,
			// ����� ��������� ������ �� ������ ���� (����� ���������, ��� ���� �� ����� ���������)
			Node* newNode = addTo(curNode, branch, line.substr(indentator.length()*indentLevel), emptyNode);
			if (newNode != nullptr) {
				curLevel++;
				curNode = newNode;
			}
		}
		// ��������� �� ������ �������
		else if (indentLevel < curLevel) {
			// ���������� ���� �� �������, ������ ��� ������ ����,
			// ���� �� ������ �� ����� ��� �� ������ �����, � ������� ����� �������� ����� ����
			do {
				if (curNode->left == emptyNode) {
					curNode->left = nullptr;
				}
				if (curNode->right == emptyNode) {
					curNode->right = nullptr;
				}
				curLevel--;
				curNode = getParentNode(curNode);
				if (curNode == nullptr) {
					break;
				}
				branch = getEmptyBranch(curNode);
			} while (curLevel != indentLevel || branch == BRANCH::NEXT);

			// ����� �� �����
			if (curNode == nullptr) {
				cout << "No parent node" << endl;
				break;
			}

			// ����������� �������� �� ������ � ���� �������� ������� ��������,
			// ��������� ����� ���� � ������ � ����������� ������� �������,
			// ����� ��������� ������ �� ������ ���� (����� ���������, ��� ���� �� ����� ���������)
			Node* newNode = addTo(curNode, branch, line.substr(indentator.length()*indentLevel), emptyNode);
			if (newNode != nullptr) {
				curLevel++;
				curNode = newNode;
			}
		}
		// ������� ����� �������� � �����
		else {
			cout << "Too much indentation" << endl;
			break;
		}
	}

	// ������� ������ �� ������ ���� �� ���������� ����
	if (curNode != nullptr) {
		if (curNode->left == emptyNode) {
			curNode->left = nullptr;
		}
		if (curNode->right == emptyNode) {
			curNode->right = nullptr;
		}
	}

	delete emptyNode;
}

// ���������� ����� ��� ������ �����, ���� ���� �� ��� ����� ��� BRANCH::NEXT, ���� ��� ��������� 
BRANCH BinaryTree::getEmptyBranch(Node* node) {
	return node->left == nullptr ? BRANCH::LEFT : (node->right == nullptr ? BRANCH::RIGHT : BRANCH::NEXT);
}

// ���������� ������������ �����
Node* BinaryTree::getParentNode(Node* node) {
	return getBranchWith(node, root);
}

// ���������� ������� � ���������� �����, ������� �������� ��������� ���������
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

// ���������� ��� ������� ������ �� ������ ����� ��� ������� `-`
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

// ������� ���-�� ��������� ������ ������ � ������
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

// ������� ����������� ������ � ������� � `  ` � �������� ��������
void BinaryTree::print() {
	print("  ");
}

// ������� ����������� ������ � ������� � ��������� ������� � �������� ��������
void BinaryTree::print(const string& indentator) {
	int level = 0;
	if (root == nullptr) {
		cout << "Tree is empty" << endl;
	}
	print(root, level, indentator);
}

// ���������� ������� �������� ����� � ������� � ��������� ��������
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

// ������� ������ ��������� ���-�� ��� � �������� ������
void BinaryTree::printIndentedValue(char value, int level, const string& indentator) {
	while (level--) {
		cout << indentator;
	}
	cout << value << endl;
}

// ������� ������ ��������� ���-�� ��� � �������� ����� �����
void BinaryTree::printIndentedValue(int value, int level, const string& indentator) {
	while (level--) {
		cout << indentator;
	}
	cout << value << endl;
}

// ���������� ������������� �� ������
bool BinaryTree::isSorted() {
	if (root == nullptr) {
		return true;
	}
	return isSorted(root);
}

// ���������� ���������, ������������ �� ���� � ��� ��� �����
bool BinaryTree::isSorted(Node* node) {
	return (!node->left || node->value >= node->left->value && isSorted(node->left)) && (!node->right || node->value <= node->right->value && isSorted(node->right));
}
