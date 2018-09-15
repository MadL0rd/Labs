#include "BinaryTree.h"

int main() {
	BinaryTree bt("Tree.txt");
	bt.print();
	std::cout << (bt.isSortedLinear() ? "Tree is sorted" : "Tree isn't sorted") << " (linear)" << std::endl;
	std::cout << (bt.isSorted() ? "Tree is sorted" : "Tree isn't sorted") << " (recursive)" << std::endl;
	return 0;
}
