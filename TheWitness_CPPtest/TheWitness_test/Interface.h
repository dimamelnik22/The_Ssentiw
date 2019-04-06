#pragma once
#include <ctime>
#include <iostream>
using namespace std;
class Pole;
class PoleDot;
class PoleLine;
class PoleSquere;

class PoleDot
{
private:
	
public:
	PoleLine* up;
	PoleLine* down;
	PoleLine* left;
	PoleLine* right;
	int position_x;
	int position_y;
	PoleDot(int x, int y)
	{
		position_x = x;
		position_y = y;
		up = nullptr;
		down = nullptr;
		right = nullptr;
		left = nullptr;
	}
	void addLine(PoleLine *newLine, PoleDot *anotherDot)
	{
		if (position_x < anotherDot->position_x /*&& position_y == anotherDot->position_y*/)
		{
			right = newLine;
		}
		else if (position_x > anotherDot->position_x /*&& position_y == anotherDot->position_y*/)
		{
			left = newLine;
		}
		else if (position_y < anotherDot->position_y /*&& position_x == anotherDot->position_x*/)
		{
			down = newLine;
		}
		else if (position_y > anotherDot->position_y /*&& position_x == anotherDot->position_x*/)
		{
			up = newLine;
		}
	}
	bool allowedToUp()
	{
		return (up != nullptr);
	}
	bool allowedToDown()
	{
		return (down != nullptr);
	}
	bool allowedToLeft()
	{
		return (left != nullptr);
	}
	bool allowedToRight()
	{
		return (right != nullptr);
	}
};

class PathDotStack
{
private:
	struct dotNode
	{
		PoleDot *dot = nullptr;
		dotNode *next = nullptr;
	};
	int size;
	dotNode *head;
public:
	void addDot(PoleDot* newDot)
	{
		dotNode *curNode = new dotNode;
		curNode->dot = newDot;
		if (head == nullptr)
		{
			size = 1;
			head = curNode;
		}
		else
		{
			size++;
			curNode->next = head;
			head = curNode;
		}
	}
	int PathLength()
	{
		return size;
	}
	PoleDot* getDot()
	{
		dotNode* tmp = head;
		head = head->next;
		size--;
		return tmp->dot;
	}
	bool isEmpty()
	{
		return head == nullptr;
	}
};

class PoleLine
{
private:
	PoleDot * first;
	PoleDot *second;
public:
	PoleLine(PoleDot *firstDot, PoleDot *secondDot)
	{
		first = firstDot;
		second = secondDot;
	}
};

class PoleSquere
{
private:

public:

};

class Pole
{
private:
	
	PoleDot *start;
	PoleDot *finish;
	int poleSize;
	PathDotStack *dotData;
	bool findPath(PoleDot* begin, PoleDot* end, int** ways)
	{
		if (begin == end)
		{
			if (dotData->PathLength() < poleSize * 2 + poleSize / 2) return false;
			return true;
		}
		
		bool tries[4] = { 1,1,1,1 };
		dotData->addDot(begin);
		bool triesLeft = true;
		/*for (int i = 0; i < poleSize; i++)
		{
			for (int j = 0; j < poleSize; j++)
			{
				cout << ways[i][j];
			}
			cout << endl;
		}
		cout << endl;*/
		while (triesLeft)
		{
			int k = rand() % 4;
			switch (k)
			{
			case 0:
				if (begin->position_y > 0 && ways[begin->position_y - 1][begin->position_x] == 0 && tries[0])
				{
					ways[begin->position_y - 1][begin->position_x] = 1;
					if (findPath(poleDots[begin->position_y - 1][begin->position_x], end, ways)) return true;
					else
					{
						ways[begin->position_y - 1][begin->position_x] = 0;
						tries[0] = 0;
					}
				}
				else tries[0] = 0;
				break;
			case 1:
				if (begin->position_x < poleSize - 1 && ways[begin->position_y][begin->position_x + 1] == 0 && tries[1])
				{
					ways[begin->position_y][begin->position_x + 1] = 1;
					if (findPath(poleDots[begin->position_y][begin->position_x + 1], end, ways)) return true;
					else
					{
						ways[begin->position_y][begin->position_x + 1] = 0;
						tries[1] = 0;
					}
				}
				else tries[1] = 0;
				break;
			case 2:
				if (begin->position_y < poleSize - 1 && ways[begin->position_y + 1][begin->position_x] == 0 && tries[2])
				{
					ways[begin->position_y + 1][begin->position_x] = 1;
					if (findPath(poleDots[begin->position_y + 1][begin->position_x], end, ways)) return true;
					else
					{
						ways[begin->position_y + 1][begin->position_x] = 0;
						tries[2] = 0;
					}
				}
				else tries[2] = 0;
				break;
			case 3:
				if (begin->position_x > 0 && ways[begin->position_y][begin->position_x - 1] == 0 && tries[3])
				{
					ways[begin->position_y][begin->position_x - 1] = 1;
					if (findPath(poleDots[begin->position_y][begin->position_x - 1], end, ways)) return true;
					else
					{
						ways[begin->position_y][begin->position_x - 1] = 0;
						tries[3] = 0;
					}
				}
				else tries[3] = 0;
				break;
			}
			if (!tries[0] && !tries[1] && !tries[2] && !tries[3])
				{
					triesLeft = false;
				}
				
		}
		ways[begin->position_y][begin->position_x] = 0;
		dotData->getDot();
		return false;
	}
public:
	PoleDot ***poleDots;
	Pole(int size)
	{
		poleSize = size;
		poleDots = new PoleDot**[size];
		for (int y = 0; y < size; y++)
		{
			poleDots[y] = new PoleDot*[size];
			for (int x = 0; x < size; x++)
			{
				poleDots[y][x] = new PoleDot(x, y);
			}
		}
	}
	void setStart(int x, int y)
	{
		start = poleDots[y][x];
	}
	void setFinish(int x, int y)
	{
		finish = poleDots[y][x];
	}
	void createSolution()
	{
		int **ways = new int*[poleSize];
		for (int i = 0; i < poleSize; i++)
		{
			ways[i] = new int[poleSize];
			for (int j = 0; j < poleSize; j++)
			{
				ways[i][j] = 0;
			}
		}
		ways[start->position_y][start->position_x] = 1;
		dotData = new PathDotStack();
		dotData->addDot(start);
		srand(time(nullptr));
		bool isFound = findPath(start, finish, ways);
		if (isFound)
		{
			PoleDot *prevDot;
			PoleDot *curDot = finish;
			while (!dotData->isEmpty())
			{
				prevDot = dotData->getDot();
				PoleLine *newLine = new PoleLine(curDot, prevDot);
				prevDot->addLine(newLine, curDot);
				curDot->addLine(newLine, prevDot);
				curDot = prevDot;
			}
		}
	}
	void clearPole()
	{
		
	}
};





