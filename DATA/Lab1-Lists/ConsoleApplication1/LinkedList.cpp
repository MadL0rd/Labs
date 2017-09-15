// ����������� ����������� ������ � ������������� ������ ����������
// �������������� ��������:
//	a) ��������� (�� �������) ��� �������� �� ������� ���� � ��������� ������;
//	�) �������� ����� ���� ����� ���� � �������� ��������� ������.

#include <iostream>
#include <string>

using std::cin;
using std::cout;
using std::endl;
using std::string;

// ������ ��������� �������� ������
template <class T>
struct Link {
	
	// ����������� ��� ������������� � �������
	Link(const T &data) : data(data) {};

	// ����������� ��� ������������� ��� ������ (��� head)
	Link() {};
	
	T data;
	Link* next;
};

// ������ ������ ������
template <class T>
class List {

private:
	Link<T>* head = new Link<T>();

public:
	// ������������� ������
	List() {
		head->next = head;
	}

	// ������� ��� �������� ������
	~List() {
		
		Link<T>* link = head->next;
		Link<T>* next;
		while (link != head) {
			next = link->next;
			delete link;
			link = next;
		}
		delete head;
	}

	// ��������� ������� � ������ ������
	void pushLink(Link<T> *link) {
		link->next = head->next;
		head->next = link;
	}

	// ������� ������� ������ � ������� � ��������� ��� � ������ ������
	void push(const T &data) {
		Link<T>* link = new Link<T>(data);
		pushLink(link);
	}

	// ��������� ������� ������ � ����� ������
	void shiftLink(Link<T> *link) {
		Link<T>* oldLink = head->next;
		while (oldLink->next != head) {
			oldLink = oldLink->next;
		}
		oldLink->next = link;
		link->next = head;
	}

	// ������� ������� ������ � ������� � ��������� ��� � ����� ������
	void shift(const T &data) {
		Link<T>* link = new Link<T>(data);
		shiftLink(link);
	}

	// ������� ������ ������� ������ � ���������� ��� ������
	T pop() {
		if(isEmpty()){
			return NULL;
		}
		T data = head->next->data;
		head->next = head->next->next;
		return data;

	}

	// ������� ��������� ������� ������ � ���������� ��� ������
	T unshift() {
		if (isEmpty()) {
			return NULL;
		}
		Link<T>* link = head;
		while (link->next->next != head) {
			link = link->next;
		}
		T data = link->next->data;
		link->next = head;
		return data;

	}

	// ��������������� ������ � ������ ��� ������ � �������
	string stringify() {
		Link<T>* link = head->next;
		string str = "";
		while (link != head) {
			str.append(link->data);
			if (link->next != head) {
				str.append(" -> ");
			}
			link = link->next;
		}
		return str;
	}

	// ������� � ��������� ������� � ������ ����� �������� � ����������� �������
	void insertAfterData(const T &lookupData, const T &data) {
		if (isEmpty()) {
			return;
		}
		Link<T>* link = head;
		while (link->next != head) {
			link = link->next;
			if (link->data == lookupData) {
				Link<T>* newLink = new Link<T>(data);
				newLink->next = link->next;
				link->next = newLink;
				return;				
			}
		}
	}

	// ��������� ��� �������� �������� ������ � ������ ������
	// list - ������ ������
	// reversed - �������� ����� ��������� � �������� �������
	void moveOdd(List<T> &list, bool reversed = false) {
		bool isOdd = false;
		Link<T>* link = head;
		Link<T>* next;
		while (link->next != head) {
			isOdd = !isOdd;
			if (isOdd) {
				next = link->next->next;
				link->next->next = nullptr;
				if (reversed) {
					list.pushLink(link->next);
				}
				else {
					list.shiftLink(link->next);
				}
				link->next = next;
			}
			else {
				link = link->next;
			}
		}
	}

	// ���� �� ������
	bool isEmpty() {
		return head->next == head;
	}

};

int main()
{
	List<string> list;
	List<string> list2;

	string data;		// ������ ��� push, shift � insertafter
	string lookupData;	// ������ ��� insertafter
	string info = 
R"(Enter one of the following:
    push <string>                  - add to head
    pop                            - remove from head
    shift <string>                 - add to tail
    unshift                        - remove from tail
    insertafter <string> <string>  - insert second string after first one
    moveodd                        - move odd elements to another list
    print                          - print all elements to console
    help                           - this information
    quit, q                        - quit program
)";
	cout << info;

	// "���������" ��� ������������ ������ �� �������
	while (true) {
		cout << "list ";
		string com;
		cin >> com;

		if (com == "push") {			
			cin >> data;
			list.push(data);
			continue;
		}

		if (com == "pop") {
			if (list.isEmpty()) {
				cout << "List is empty\n";
				continue;
			}
			cout << list.pop() << endl;
			continue;
		}

		if (com == "shift") {
			cin >> data;
			list.shift(data);
			continue;
		}

		if (com == "unshift") {
			if (list.isEmpty()) {
				cout << "List is empty\n";
				continue;
			}
			cout << list.unshift() << endl;
			continue;
		}

		if (com == "insertafter") {
			cin >> lookupData >> data ;
			list.insertAfterData(lookupData, data);
			continue;
		}

		if (com == "moveodd") {
			cout << "list1: " << list.stringify() << endl;
			cout << "list2: " << list2.stringify() << endl;
			cout << "Moving odd links from list1 to list2\n";
			list.moveOdd(list2);
			cout << "list1: " << list.stringify() << endl;
			cout << "list2: " << list2.stringify() << endl;
			continue;
		}

		if (com == "print") {
			cout << list.stringify() << endl;
			continue;
		}

		if (com == "quit" || com == "q") {
			break;
		}

		cout << info;
	}
	return 0;
}

