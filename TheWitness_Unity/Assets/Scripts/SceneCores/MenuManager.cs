﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject PolePF;
    public GameObject ActivePathPF;
    public GameObject MenuItemPF;
    public GameObject MenuInputFeildPF;

    [HideInInspector]
    public GameObject activePath;
    [HideInInspector]
    public GameObject menuPole;

    private readonly List<GameObject> prevPoles = new List<GameObject>();
    private readonly List<GameObject> prevPaths = new List<GameObject>();
    private MenuLinkedList menuMap;
    private Vector3 lastPos;
    
    delegate void MenuFunc();
    //????
    public static class DebugMessage
    {
        private static int seed = new int();
        private static string s;
        private static string path;
        public static void Push2Buffer()
        {
            GUIUtility.systemCopyBuffer =":"+ seed + "\n" + s + ":" + path;
        }
        public static void SavePath(string p)
        {
            path = p;
        }
        public static void SaveSeed(int a)
        {
            seed = a;
        }
        public static void Log(string output)
        {
            //Debug.Log(output);
            s += output+"\n";
        }
        public static void Clear()
        {
            path = "";
            s = "";    
            seed = new int();
        }
    }
    //????
    public static class MainSettings
    {
        public static string language = "ENG";
        public static float speed = 1f;
        public static float complexity = 0.5f;
        public static float numOfPoints = 1f;
        public static float numOfCircles = 1f;
        public static float numOfStars = 1f;
        public static float numOfShapes = 0.3f;
        public static List<string> levels = new List<string>();
    }

    class MenuNode
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
    class MenuLinkedList
    {
        public MenuNode pointer;
        public void Add(MenuNode next, int n)
        {
            if (pointer == null)
            {
                pointer = next;
                
                switch (MainSettings.language)
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
    private static void Log()
    {
        DebugMessage.SaveSeed(Core.PolePreferences.MyRandom.seed);
        DebugMessage.Log(":" + Core.PolePreferences.numOfCircles);
        DebugMessage.Log(":" + Core.PolePreferences.numOfPoints);
        DebugMessage.Log(":" + Core.PolePreferences.numOfShapes);
        DebugMessage.Log(":" + Core.PolePreferences.numOfStars);
        DebugMessage.Log(":" + Core.PolePreferences.poleSize);
    }
    private void DebugSetting(int seed, int numOfCircles, int numOfPoints, int numOfShapes, int numOfStars, int poleSize)
    {
        Core.PolePreferences.MyRandom.seed = seed;
        Core.PolePreferences.numOfCircles = numOfCircles;
        Core.PolePreferences.numOfPoints = numOfPoints;
        Core.PolePreferences.numOfShapes = numOfShapes;
        Core.PolePreferences.numOfStars = numOfStars;
        Core.PolePreferences.poleSize = poleSize;
    }
    private void ConvertPreferences()
    {
        int p = Core.PolePreferences.poleSize * Core.PolePreferences.poleSize;
        Core.PolePreferences.complexity = Mathf.RoundToInt(p * MainSettings.complexity);
        Core.PolePreferences.numOfPoints = Mathf.RoundToInt(Core.PolePreferences.poleSize * MainSettings.numOfPoints + Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1) * MainSettings.numOfPoints);
        Core.PolePreferences.numOfCircles = Mathf.RoundToInt(Core.PolePreferences.poleSize * MainSettings.numOfCircles + Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1) * MainSettings.numOfCircles);
        Core.PolePreferences.numOfStars = Mathf.RoundToInt(Core.PolePreferences.poleSize * MainSettings.numOfStars + Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1) * MainSettings.numOfStars);
        Core.PolePreferences.numOfShapes = Mathf.RoundToInt(p * MainSettings.numOfShapes);
    }
    private void LoadPoleLevel()
    {
        Core.PolePreferences.mode = "normal";
        //Debug.Log(Core.PolePreferences.poleSize + " " + Core.PolePreferences.complexity + " " + Core.PolePreferences.numOfPoints);
        Core.PolePreferences.MyRandom.SetSeed();
        Log();
        ConvertPreferences();
        SceneManager.LoadScene("PoleLevel");
    }
    private void LoadLevelWithString()
    {
        Core.PolePreferences.mode = "info";
        Core.PolePreferences.info = MainSettings.levels[0];
        Debug.Log(Core.PolePreferences.info);
        Debug.Log(MainSettings.levels[0]);
        SceneManager.LoadScene("PoleLevel");
    }
    private void LoadRandomPoleLevel()
    {
        Core.PolePreferences.mode = "normal";
        //Core.PolePreferences.MyRandom.seed = Core.PolePreferences.MyRandom.GetRandom();
        //Core.PolePreferences.MyRandom.SetSeed(Core.PolePreferences.MyRandom.GetRandom());
        Core.PolePreferences.MyRandom.SetSeed();
        Core.PolePreferences.poleSize = 5 + Core.PolePreferences.MyRandom.GetRandom() % 5;
        //Core.PolePreferences.poleSize = 8;
        Core.PolePreferences.complexity = Mathf.RoundToInt(Core.PolePreferences.poleSize * Core.PolePreferences.poleSize * (4 + Core.PolePreferences.MyRandom.GetRandom() % 4) / 10);
        Core.PolePreferences.numOfCircles = Core.PolePreferences.poleSize + Core.PolePreferences.MyRandom.GetRandom() % Core.PolePreferences.poleSize;
        Core.PolePreferences.numOfPoints = Core.PolePreferences.poleSize + Core.PolePreferences.MyRandom.GetRandom() % Core.PolePreferences.poleSize;
        Log();
        Core.PolePreferences.numOfShapes = Mathf.RoundToInt(Core.PolePreferences.poleSize * Core.PolePreferences.poleSize * (1 + Core.PolePreferences.MyRandom.GetRandom() % 4) / 10);
        //Core.PolePreferences.numOfShapes = 50;
        //Debug.Log(Core.PolePreferences.MyRandom.seed + " " + Core.PolePreferences.poleSize + " " + Core.PolePreferences.numOfPoints);
        SceneManager.LoadScene("PoleLevel");
    }
    public void InputField()
    {
        //Instantiate(MenuInputFeildPF, new Vector3(Screen.width/2, Screen.height/2, 0f), MenuItemPF.transform.rotation);
        //Instantiate(MenuItemPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), MenuItemPF.transform.rotation);

    }

    public void CreateNewPole(int size)
    {
        prevPaths.Add(activePath);
        activePath.GetComponent<ActivePath>().pointer.SetActive(false);
        prevPoles.Add(menuPole);
        menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        menuPole.GetComponent<Pole>().InitMenuItem(size);
        activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().starts, menuPole.GetComponent<Pole>().finishes);
        activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
    }
    //public void CreateNewSlider(int size)
    //{
    //    prevPaths.Add(activePath);
    //    activePath.GetComponent<ActivePath>().pointer.SetActive(false);
    //    prevPoles.Add(menuPole);
    //    menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
    //    menuPole.GetComponent<Pole>().InitMenuSlider(size);
    //    activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
    //    activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().starts, menuPole.GetComponent<Pole>().finishes);
    //    activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
    //}


    private void ParseLevels()
    {
        MainSettings.levels = new List<string>();
        string rawlvls = (Resources.Load("levels") as TextAsset).text;
        string lvl = "";
        for (int i = 0; i < rawlvls.Length; i++)
        {
            if (rawlvls[i].ToString() == "L" && i > 0)
            {
                MainSettings.levels.Add(string.Copy(lvl));
                lvl = "";
            }
            else if (i > 0)
            {
                lvl += rawlvls[i];
            }
        }
        MainSettings.levels.Add(string.Copy(lvl));

    }

    public void PressButton(int indexOfMenuItem)
    {
        if (indexOfMenuItem == -1)
        {
            if (prevPaths.Count > 0)
            {

                Destroy(menuPole);
                Destroy(activePath);

                activePath = prevPaths[prevPaths.Count - 1];

                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.SetActive(true);
                menuMap.Back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
                    item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
                    item.GetComponentInChildren<MenuButton>().index = index;
                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);

                activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);


            }
        }
        else if (indexOfMenuItem < menuPole.GetComponent<Pole>().finishes.Count)
        {
            if (menuMap.Go2(indexOfMenuItem))
            {
                menuMap.Back();
                activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
                activePath.GetComponent<ActivePath>().SystemStep(menuPole.GetComponent<Pole>().poleDots[0][1]);
                
                for (int i = 0; i < indexOfMenuItem; ++i)
                {
                    Debug.Log(indexOfMenuItem);
                    activePath.GetComponent<ActivePath>().SystemStep(menuPole.GetComponent<Pole>().poleDots[i + 1][1]);
                }
                activePath.GetComponent<ActivePath>().SystemStep(menuPole.GetComponent<Pole>().poleDots[indexOfMenuItem][2]);
                
            }
            else if (prevPaths.Count > 0)
            {
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                Destroy(menuPole);
                Destroy(activePath);

                activePath = prevPaths[prevPaths.Count - 1];

                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.SetActive(true);
                menuMap.Back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
                    item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
                    item.GetComponentInChildren<MenuButton>().index = index;
                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
                activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
            }
        }
        else Debug.Log("smth is wrong");
    }
    // Use this for initialization
    void Start () {
        ParseLevels();
        // Menu structure Generation
        menuMap = new MenuLinkedList();

        MenuFunc[] funcList = new MenuFunc[5];
        funcList[0] = LoadRandomPoleLevel;
        funcList[2] = () => LoadLevelWithString();
        funcList[4] = Application.Quit;
        string[] names;
        switch(MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт", "Пользовательская", "Примеры", "Настройки", "Выход" };
                break;
            default:
                names = new string[] { "Start random", "Custom", "Examples", "Settings", "Exit" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 5), 0);

        funcList = new MenuFunc[2];
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Язык" , "Скорость" };
                break;
            default:
                names = new string[] { "Language", "Game speed"  };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 2), 3);
        menuMap.Go2(3);
        funcList = new MenuFunc[2];
        funcList[0] = () => 
        {
            if (MenuManager.MainSettings.language !="ENG")
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
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Английский", "Русский" };
                break;
            default:
                names = new string[] { "English", "Russian" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 2),0);
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
        funcList[0] = LoadPoleLevel;
        //funcList[4] = InputField;    
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт" , "Размер", "Длина", "Элементы", "custompuzzle" };
                break;
            default:
                names = new string[] { "Start","Size", "Complexity", "Elements", "custompuzzle" };
                break;
        }
        menuMap.Add(new MenuNode(names, funcList, 5),1);
        funcList = new MenuFunc[4];
        switch (MainSettings.language)
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
        funcList[0] = () => Core.PolePreferences.poleSize = 5;
        funcList[1] = () => Core.PolePreferences.poleSize = 6;
        funcList[2] = () => Core.PolePreferences.poleSize = 7;
        funcList[3] = () => Core.PolePreferences.poleSize = 8;
        funcList[4] = () => Core.PolePreferences.poleSize = 9;
        
        switch (MainSettings.language)
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
        switch (MainSettings.language)
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
            switch (MainSettings.language)
            {
                case "RUS":
                    names = new string[] { "Нет", "Легко", "Средне", "Сложно", "Профессионал" };
                    break;
                default:
                    names = new string[] { "Off", "Easy", "Medium", "Hard", "Pro" };
                    break;
            }
            menuMap.Add(new MenuNode(names, funcList, 5), i);

            switch (MainSettings.language)
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
        //names = new string[] { "1", "2" };
        //funcList = new MenuFunc[2];
        //funcList[0] = () => LoadLevelWithString();
        //menuMap.add(new MenuNode(names, funcList, 2),2);


        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
#if UNITY_EDITOR
        lastPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)
            {

                GetComponent<ParticleSystem>().Play();
                var touch = Input.GetTouch(0);
                lastPos = touch.position;
            }
#endif
        menuPole = Instantiate(PolePF, transform);
        menuPole.GetComponent<Pole>().InitMenuItem(menuMap.pointer.size);
        foreach(GameObject finish in menuPole.GetComponent<Pole>().finishes)
        {
            int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
            var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
            item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
            item.GetComponentInChildren<MenuButton>().index = index;
        }
        Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);

        activePath = Instantiate(ActivePathPF, transform);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().starts, menuPole.GetComponent<Pole>().finishes);
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 dist = Input.mousePosition - lastPos;
#if UNITY_EDITOR
        lastPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)
            {

                GetComponent<ParticleSystem>().Play();
                var touch = Input.GetTouch(0);
                lastPos = touch.position;
            }
#endif

        if (activePath.GetComponent<ActivePath>().isFinished && (Input.GetMouseButton(0) == false) && (Input.touchCount == 0))
        {

            if (menuMap.Go2(activePath.GetComponent<ActivePath>().finishes.IndexOf(activePath.GetComponent<ActivePath>().currentFinishOnPole)))
            {
                Debug.Log("qwjdoiwasd");

                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                CreateNewPole(menuMap.pointer.size);
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
                    item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
                    item.GetComponentInChildren<MenuButton>().index = index;

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(0.75f, 0f, 0f);
                activePath.GetComponent<ActivePath>().Update();
            }
            else  if (prevPaths.Count>0)
            {
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                Destroy(menuPole);
                Destroy(activePath);

                activePath = prevPaths[prevPaths.Count - 1];

                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.SetActive(true);
                menuMap.Back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
                    item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
                    item.GetComponentInChildren<MenuButton>().index = index;
                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(-4f, 0f, 0f);
                activePath.GetComponent<ActivePath>().Update();
            }
        }
        else if (prevPaths.Count > 0 )
        {
            
            if (activePath.GetComponent<ActivePath>().pointer != null && activePath.GetComponent<ActivePath>().pointer.transform.position.x <= menuPole.GetComponent<Pole>().starts[0].transform.position.x && dist.x < 0 && (Input.GetMouseButton(0) == false) && (Input.touchCount == 0))
            {
                Debug.Log(222);
                Destroy(menuPole);
                Destroy(activePath);
                
                activePath = prevPaths[prevPaths.Count - 1];
                
                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.SetActive(true);
                menuMap.Back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    var item = Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation);
                    item.GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
                    item.GetComponentInChildren<MenuButton>().index = index;
                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().starts[0].transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
       
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(-4f, 0f, 0f);
                activePath.GetComponent<ActivePath>().Update();

            }
        }
        
    }
}
