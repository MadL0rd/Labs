// ������

#include <iostream>
#include <string>

using std::cin;
using std::cout;
using std::endl;
using std::string;

// ������ ����� �� �������
template <class T>
class Stack {

private:
	T* stack = nullptr;	// ������ �����
	int top = -1;		// ������ �������� �����
	int len;			// ����. ����� �����

public:
	// ������������� � ��������� ����� � ��������� �������
	Stack(int len) : len(len) {
		if (len < 1) {
			len = 1;
		}
		stack = new T[len];
	}

	// ����������� � ��������� �������
	~Stack() {
		delete[] stack;
		stack = nullptr;
	}

	// ��������� ������� ������ �����, ���� �� �� �����,
	// ���������� ���������� ����������
	bool push(T el) {
		if (isFull()) {
			return false;
		}
		stack[++top] = el;
		return true;
	}

	// ������� � ���������� ������� ������ �����, ���� �� �� ����
	T pop() {
		if (isEmpty()) {
			return NULL;
		}
		T el = stack[top--];
		return el;
	}

	// ���� �� ����
	bool isEmpty() {
		return top == -1;
	}

	// ����� �� ����
	bool isFull() {
		return top == len - 1;
	}

};

// "���������" ��� ������������ ������ �� ������
int main() {
	int len = 0;
	while (len <= 0) {
		cout << "Stack length: ";
		cin >> len;
		cout << endl;
	}

	Stack <string> stack(len);

	string info = "Enter push <string>, pop or exit.\n";
	cout << info;

	while (true) {
		string com;
		cin >> com;

		if (com == "push") {
			string el;
			cin >> el;

			if (stack.isFull()) {
				cout << "Stack is full\n";
				continue;
			}

			stack.push(el);
			continue;
		}

		if (com == "pop") {

			if (stack.isEmpty()) {
				cout << "Stack is empty\n";
				continue;
			}

			cout << stack.pop() << endl;
			continue;
		}

		if (com == "exit") {
			break;
		}

		cout << info;
	}
    return 0;
}

