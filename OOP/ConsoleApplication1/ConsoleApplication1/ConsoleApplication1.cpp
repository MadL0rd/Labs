// Классы

#include <iostream>
#include <string>

using std::cin;
using std::cout;
using std::endl;
using std::string;

// Шаблон стека из массива
template <class T>
class Stack {

private:
	T* stack = nullptr;	// Массив стека
	int top = -1;		// Индекс верхушки стека
	int len;			// Макс. длина стека

public:
	// Инициализация с указанием длины и созданием массива
	Stack(int len) : len(len) {
		if (len < 1) {
			len = 1;
		}
		stack = new T[len];
	}

	// Уничтожение с удалением массива
	~Stack() {
		delete[] stack;
		stack = nullptr;
	}

	// Добавляет элемент наверх стека, если он не полон,
	// возвращает успешность добавления
	bool push(T el) {
		if (isFull()) {
			return false;
		}
		stack[++top] = el;
		return true;
	}

	// Удаляет и возвращает элемент сверху стека, если он не пуст
	T pop() {
		if (isEmpty()) {
			return NULL;
		}
		T el = stack[top--];
		return el;
	}

	// Пуст ли стек
	bool isEmpty() {
		return top == -1;
	}

	// Полон ли стек
	bool isFull() {
		return top == len - 1;
	}

};

// "Интерфейс" для демонстрации работы со стеком
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

