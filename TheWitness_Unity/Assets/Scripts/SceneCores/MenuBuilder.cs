using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuBuilder
{
    public delegate void MenuFunc();
    public class MenuNode
    {
        public string MainName;
        public int size;
        public string[] Name;
        public MenuFunc[] Func;
        public MenuNode prev;
        public MenuNode[] next;
        public MenuNode(string[] name, MenuFunc[] Func, int size)
        {
            this.size = size;
            this.Name = name;
            this.Func = Func;
            this.next = new MenuNode[size];
        }

    }
    public class MenuLinkedList
    {
        public MenuNode pointer;
        public void Add(MenuNode next, int n)
        {
            if (pointer == null)
            {
                pointer = next;

                switch (MenuManager.MainSettings.language)
                {
                    case "RUS":
                        pointer.MainName = "Главное меню";
                        break;
                    default:
                        pointer.MainName = "Main Menu";
                        break;
                }
            }
            else
            {
                next.MainName = pointer.Name[n];
                pointer.next[n] = next;
                next.prev = pointer;
            }
        }
        public bool Go2(int n)
        {
            if (pointer.next.Length > n)
            {
                if (pointer.next[n] != null)
                {
                    pointer = pointer.next[n];
                    return true;
                }
                else if (pointer.Func[n] != null)
                {
                    pointer.Func[n]();
                    return false;
                }
                return false;
            }
            else
            {
                Debug.Log("you go in don't sel path line 45");
                return false;
            }
        }
        public void Back()
        {
            if (pointer.prev != null)
            {
                pointer = pointer.prev;
            }
            else
            {
                Debug.Log("you go in don't sel path line 56");
            }
        }
    }
    public static MenuLinkedList BuildMenu()
    {
        MenuLinkedList menuMap = new MenuLinkedList();
        MenuFunc[] funcList = new MenuFunc[5];
        funcList[0] = MenuManager.LoadRandomPoleLevel;
        funcList[2] = () => { PlayerPrefs.SetInt("IntroSkip", 0); MenuManager.LoadTutorial(); };
        funcList[4] = Application.Quit;
        string[] names;
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт", "Пользовательская", "Обучение", "Настройки", "Выход" };
                break;
            default:
                names = new string[] { "Start random", "Custom", "Tutorial", "Settings", "Exit" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 5), 0);

        funcList = new MenuFunc[3];
        funcList[2] = () => { PlayerPrefs.DeleteAll(); };
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Язык", "Скорость", "Сбросить прогресс" };
                break;
            default:
                names = new string[] { "Language", "Game speed", "Reset progress" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 3), 3);
        menuMap.Go2(3);
        funcList = new MenuFunc[2];
        funcList[0] = () =>
        {
            if (MenuManager.MainSettings.language != "ENG")
            {
                MenuManager.MainSettings.language = "ENG";
                SceneManager.LoadScene("MainMenu");
            }
        };
        funcList[1] = () =>
        {
            if (MenuManager.MainSettings.language != "RUS")
            {
                MenuManager.MainSettings.language = "RUS";
                SceneManager.LoadScene("MainMenu");
            }
        };
        names = new string[] { "English", "Russian" };
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Английский", "Русский" };
                break;
            default:
                names = new string[] { "English", "Russian" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 2), 0);
        names = new string[5] { "0.5x", "0.75x", "1x", "1.5x", "2x" };
        funcList = new MenuFunc[5];
        funcList[0] = () => MenuManager.MainSettings.speed = 0.5f;
        funcList[1] = () => MenuManager.MainSettings.speed = 0.75f;
        funcList[2] = () => MenuManager.MainSettings.speed = 1f;
        funcList[3] = () => MenuManager.MainSettings.speed = 1.5f;
        funcList[4] = () => MenuManager.MainSettings.speed = 2f;
        menuMap.Add(new MenuNode(names, funcList, 5), 1);
        menuMap.Back();
        funcList = new MenuFunc[5];
        funcList[0] = MenuManager.LoadPoleLevel;
        //funcList[4] = InputField;    
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт", "Размер", "Длина", "Элементы", "custompuzzle" };
                break;
            default:
                names = new string[] { "Start", "Size", "Complexity", "Elements", "custompuzzle" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 5), 1);
        funcList = new MenuFunc[4];
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Точки", "Круги", "Еще не готово", "Фигуры" };
                break;
            default:
                names = new string[] { "Points", "Circles", "Not done yet", "Shapes" };
                break;
        }
        menuMap.Go2(1);
        menuMap.Add(new MenuNode(names, funcList, 4), 3);
        funcList = new MenuFunc[5];
        funcList[0] = () => { Core.PolePreferences.height = 5; Core.PolePreferences.width = 5; };
        funcList[1] = () => { Core.PolePreferences.height = 6; Core.PolePreferences.width = 6; };
        funcList[2] = () => { Core.PolePreferences.height = 7; Core.PolePreferences.width = 7; };
        funcList[3] = () => { Core.PolePreferences.height = 8; Core.PolePreferences.width = 8; };
        funcList[4] = () => { Core.PolePreferences.height = 9; Core.PolePreferences.width = 9; };

        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Легко", "Средне", "Сложно", "Профессионал", "Невозможно" };
                break;
            default:
                names = new string[] { "Easy", "Medium", "Hard", "Pro", "Insane" };
                break;
        }

        menuMap.Add(new MenuNode(names, funcList, 5), 1);
        funcList = new MenuFunc[5];
        funcList[0] = () => MenuManager.MainSettings.complexity = 0.3f;
        funcList[1] = () => MenuManager.MainSettings.complexity = 0.4f;
        funcList[2] = () => MenuManager.MainSettings.complexity = 0.5f;
        funcList[3] = () => MenuManager.MainSettings.complexity = 0.6f;
        funcList[4] = () => MenuManager.MainSettings.complexity = 0.7f;
        switch (MenuManager.MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Легко", "Средне", "Сложно", "Профессионал", "Невозможно" };
                break;
            default:
                names = new string[] { "Easy", "Medium", "Hard", "Pro", "Insane" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 5), 2);
        menuMap.Go2(3);
        for (int i = 0; i < 4; i++)
        {
            funcList = new MenuFunc[5];
            switch (i)
            {
                case 0:
                    funcList = new MenuFunc[5];
                    funcList[0] = () => MenuManager.MainSettings.numOfPoints = 0;
                    funcList[1] = () => MenuManager.MainSettings.numOfPoints = 0.5f;
                    funcList[2] = () => MenuManager.MainSettings.numOfPoints = 1f;
                    funcList[3] = () => MenuManager.MainSettings.numOfPoints = 1.5f;
                    funcList[4] = () => MenuManager.MainSettings.numOfPoints = 2f;
                    break;
                case 1:
                    funcList = new MenuFunc[5];
                    funcList[0] = () => MenuManager.MainSettings.numOfCircles = 0;
                    funcList[1] = () => MenuManager.MainSettings.numOfCircles = 0.5f;
                    funcList[2] = () => MenuManager.MainSettings.numOfCircles = 1f;
                    funcList[3] = () => MenuManager.MainSettings.numOfCircles = 1.5f;
                    funcList[4] = () => MenuManager.MainSettings.numOfCircles = 2f;
                    break;
                case 2:
                    funcList = new MenuFunc[5];
                    funcList[0] = () => MenuManager.MainSettings.numOfStars = 0;
                    funcList[1] = () => MenuManager.MainSettings.numOfStars = 0.5f;
                    funcList[2] = () => MenuManager.MainSettings.numOfStars = 1f;
                    funcList[3] = () => MenuManager.MainSettings.numOfStars = 1.5f;
                    funcList[4] = () => MenuManager.MainSettings.numOfStars = 2f;
                    break;
                case 3:
                    funcList = new MenuFunc[5];
                    funcList[0] = () => MenuManager.MainSettings.numOfShapes = 0;
                    funcList[1] = () => MenuManager.MainSettings.numOfShapes = 0.2f;
                    funcList[2] = () => MenuManager.MainSettings.numOfShapes = 0.3f;
                    funcList[3] = () => MenuManager.MainSettings.numOfShapes = 0.4f;
                    funcList[4] = () => MenuManager.MainSettings.numOfShapes = 0.5f;
                    break;
            }
            switch (MenuManager.MainSettings.language)
            {
                case "RUS":
                    names = new string[] { "Нет", "Легко", "Средне", "Сложно", "Профессионал" };
                    break;
                default:
                    names = new string[] { "Off", "Easy", "Medium", "Hard", "Pro" };
                    break;
            }
            menuMap.Add(new MenuNode(names, funcList, 5), i);

            switch (MenuManager.MainSettings.language)
            {
                case "RUS":
                    menuMap.pointer.next[i].MainName += ": уровень";
                    break;
                default:
                    menuMap.pointer.next[i].MainName += " difficulty";
                    break;
            }
        }
        menuMap.Back();
        menuMap.Back();
        return menuMap;
    }
}
