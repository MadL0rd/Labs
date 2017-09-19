#pragma once
#include <iostream>

// Ветка дерева
struct Node {
	int value;
	Node* left = nullptr;
	Node* right = nullptr;
};

// Возможные ветки дерева
// NEXT значит, что обе заняты
enum class BRANCH {
	LEFT, RIGHT, NEXT
};

// Рекурсивное дерево
class BinaryTree {
public:
	Node* root = nullptr;
	BinaryTree();
	BinaryTree(const std::string&);
	~BinaryTree();

	void print();
	void print(const std::string&);

	bool isSorted();
	bool isSortedLinear();

private:
	Node* addTo(Node*, BRANCH, int);
	Node* addTo(Node*, BRANCH, const std::string&, Node*);
	void addAsRoot(int);
	void addFromFile(const std::string&);

	void removeNodeShallow(Node*, Node*);

	void deleteTree(Node*&);

	Node* getBranchWith(Node*);
	Node* getBranchWith(Node*, Node*);

	std::string extractIndenter(const std::string&);
	int countSubstring(const std::string&, const std::string&);
	BRANCH getEmptyBranch(Node*);

	void print(Node*, int, const std::string&);
	void printIndentedValue(char, int, const std::string&);
	void printIndentedValue(int, int, const std::string&);

	bool isSorted(Node*);
};
