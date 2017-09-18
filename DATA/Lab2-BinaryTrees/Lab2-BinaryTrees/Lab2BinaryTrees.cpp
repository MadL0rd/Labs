#include "BinaryTree.h"

int main() {
	BinaryTree bt("Tree.txt");
	bt.print();
	std::cout << (bt.isSorted() ? "Tree is sorted" : "Tree isn't sorted") << std::endl;
    return 0;
}
