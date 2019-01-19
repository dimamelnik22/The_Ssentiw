#include <iostream>
#include "Interface.h"
using namespace std;

int main()
{
	int size = 5;
	while (true)
	{
		system("cls");
		Pole *myPole = new Pole(size);
		myPole->setStart(0, 0);
		myPole->setFinish(size - 1, size - 1);
		myPole->createSolution();
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				cout << "0 ";
				if (j < size - 1)
					if (myPole->poleDots[i][j]->right != nullptr)
						cout << "0 0 0 ";
					else cout << "      ";
			}
			cout << endl;
			if (i < size - 1)
				for (int k = 0; k < 3; k++)
				{
					for (int j = 0; j < size; j++)
					{
						if (myPole->poleDots[i][j]->down != nullptr)
							cout << "0       ";
						else cout << "        ";
					}
					cout << endl;
				}
		}

		system("pause");
	}
	return 0;
}