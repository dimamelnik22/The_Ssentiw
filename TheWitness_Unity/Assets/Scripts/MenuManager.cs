using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour {

    public GameObject PolePF;
    public GameObject ActivePathPF;
    public GameObject MenuItemPF;

    public GameObject activePath;
    public GameObject menuPole;
    public List<GameObject> prevPoles = new List<GameObject>();
    public List<GameObject> prevPaths = new List<GameObject>();

    private Vector3 lastPos;
    private bool resumed = true;
    public static class MainSettings
    {
        public static string language = "ENG";
        public static float speed = 1f;
    }
    private MenuLinkedList menuMap;

    delegate void MenuFunc();
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
        public void add(MenuNode next, int n)
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
        public bool go2(int n)
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
        public void back()
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

    private void LoadPoleLevel()
    {
        Debug.Log(Core.PolePreferences.poleSize + " " + Core.PolePreferences.complexity + " " + Core.PolePreferences.numOfPoints);
        SceneManager.LoadScene("PoleLevel");
    }

    private void LoadRandomPoleLevel()
    {
        Core.PolePreferences.MyRandom.seed = Core.PolePreferences.MyRandom.GetRandom();
        Core.PolePreferences.poleSize = 5 + Core.PolePreferences.MyRandom.GetRandom() % 3;
        Core.PolePreferences.numOfPoints = Core.PolePreferences.poleSize + Core.PolePreferences.MyRandom.GetRandom() % Core.PolePreferences.poleSize;
        //Debug.Log(Core.PolePreferences.MyRandom.seed + " " + Core.PolePreferences.poleSize + " " + Core.PolePreferences.numOfPoints);
        SceneManager.LoadScene("PoleLevel");
    }
    private void LoadLevelSelect()
    {
        
    }
    public void CreateNewPole(int size)
    {
        prevPaths.Add(activePath);
        activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = true;
        prevPoles.Add(menuPole);
        menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        menuPole.GetComponent<Pole>().InitMenuItem(size);
        activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().start, menuPole.GetComponent<Pole>().finishes);
    }
    public void CreateNewSlider(int size)
    {
        prevPaths.Add(activePath);
        activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = true;
        prevPoles.Add(menuPole);
        menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        menuPole.GetComponent<Pole>().InitMenuSlider(size);
        activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().start, menuPole.GetComponent<Pole>().finishes);
    }

    public void deb() { Debug.Log("debug"); }
    // Use this for initialization
    void Start () {

        // Menu structure Generation
        menuMap = new MenuLinkedList();

        MenuFunc[] funcList = new MenuFunc[5];
        funcList[0] = LoadRandomPoleLevel;
        funcList[2] = LoadLevelSelect;
        funcList[4] = Application.Quit;
        string[] names;
        switch(MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт", "Пользовательская", "Для отладки", "Настройки", "Выход" };
                break;
            default:
                names = new string[] { "Start random", "Custom", "Debug", "Settings", "Exit" };
                break;
        }
        menuMap.add(new MenuNode(names, funcList, 5), 0);

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
        menuMap.add(new MenuNode(names, funcList, 2), 3);
        menuMap.go2(3);
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
        menuMap.add(new MenuNode(names, funcList, 2),0);
        names = new string[5] { "0.5x", "0.75x", "1x", "1.5x", "2x" };
        funcList = new MenuFunc[5];
        funcList[0] = () => MenuManager.MainSettings.speed = 0.5f;
        funcList[1] = () => MenuManager.MainSettings.speed = 0.75f;
        funcList[2] = () => MenuManager.MainSettings.speed = 1f;
        funcList[3] = () => MenuManager.MainSettings.speed = 1.5f;
        funcList[4] = () => MenuManager.MainSettings.speed = 2f;
        menuMap.add(new MenuNode(names, funcList, 5), 1);
        menuMap.back();
        funcList = new MenuFunc[4];
        funcList[0] = LoadPoleLevel;
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Старт", "Размер", "Длина", "Элементы" };
                break;
            default:
                names = new string[] { "Start", "Size", "Complexity", "Elements" };
                break;
        }
        menuMap.add(new MenuNode(names, funcList, 4),1);
        funcList = new MenuFunc[4];
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Точки", "Круги", "Звезды", "Фигуры" };
                break;
            default:
                names = new string[] { "Points", "Circles", "Stars", "Shapes" };
                break;
        }
        menuMap.go2(1);
        menuMap.add(new MenuNode(names, funcList, 4), 3);
        funcList = new MenuFunc[5];
        funcList[0] = () => Core.PolePreferences.poleSize = 5;
        funcList[1] = () => Core.PolePreferences.poleSize = 6;
        funcList[2] = () => Core.PolePreferences.poleSize = 7;
        funcList[3] = () => Core.PolePreferences.poleSize = 8;
        funcList[4] = () => Core.PolePreferences.poleSize = 9;
        names = new string[5] { "Easy", "Medium", "Hard", "Pro", "Developer" };
        switch (MainSettings.language)
        {
            case "RUS":
                names = new string[] { "Чайник", "Легко", "Средне", "Сложно", "Разработчик" };
                break;
            default:
                names = new string[] { "Easy", "Medium", "Hard", "Pro", "Developer" };
                break;
        }

        menuMap.add(new MenuNode(names, funcList, 5), 1);
        funcList = new MenuFunc[5];
        funcList[0] = () => Core.PolePreferences.complexity = 0.3f;
        funcList[1] = () => Core.PolePreferences.complexity = 0.4f;
        funcList[2] = () => Core.PolePreferences.complexity = 0.5f;
        funcList[3] = () => Core.PolePreferences.complexity = 0.6f;
        funcList[4] = () => Core.PolePreferences.complexity = 0.7f;

        menuMap.add(new MenuNode(names, funcList, 5), 2);
        menuMap.go2(3);
        for (int i = 0; i < 4; i++)
        {
            funcList = new MenuFunc[5];
            switch (i)
            {
                case 0:
                    funcList = new MenuFunc[5];
                    funcList[0] = () => Core.PolePreferences.numOfPoints = 0;
                    funcList[1] = () => Core.PolePreferences.numOfPoints = Mathf.RoundToInt(Core.PolePreferences.poleSize*0.5f + Core.PolePreferences.MyRandom.GetRandom() %(Core.PolePreferences.poleSize-1));
                    funcList[2] = () => Core.PolePreferences.numOfPoints = Core.PolePreferences.poleSize + Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1);
                    funcList[3] = () => Core.PolePreferences.numOfPoints = Mathf.RoundToInt(Core.PolePreferences.poleSize * 1.5f+ Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1));
                    funcList[4] = () => Core.PolePreferences.numOfPoints = Mathf.RoundToInt(Core.PolePreferences.poleSize * 2f+ Core.PolePreferences.MyRandom.GetRandom() % (Core.PolePreferences.poleSize - 1));
                    break;
                case 1:
                    for (int j = 0; j < 5; j++)
                    {
                        funcList[j] = () => Core.PolePreferences.numOfCircles = j + Core.PolePreferences.MyRandom.GetRandom() % j;
                    }
                    break;
                case 2:
                    for (int j = 0; j < 5; j++)
                    {
                        funcList[j] = () => Core.PolePreferences.numOfStars = 2*j;
                    }
                    break;
                case 3:
                    for (int j = 0; j < 5; j++)
                    {
                        funcList[j] = () => Core.PolePreferences.numOfShapes = j;
                    }
                    break;
            }
            switch (MainSettings.language)
            {
                case "RUS":
                    names = new string[] { "Чайник", "Легко", "Средне", "Сложно", "Разработчик" };
                    break;
                default:
                    names = new string[] { "Easy", "Medium", "Hard", "Pro", "Developer" };
                    break;
            }
            menuMap.add(new MenuNode(names, funcList, 5), i);

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
        menuMap.back();
        menuMap.back();
        names = new string[] { "1", "2" };
        funcList = new MenuFunc[2];
        menuMap.add(new MenuNode(names, funcList, 2),2);


        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        Core.PolePreferences.isFrozen = false;
        lastPos = Input.mousePosition;
        menuPole = Instantiate(PolePF, transform);
        menuPole.GetComponent<Pole>().InitMenuItem(menuMap.pointer.size);
        foreach(GameObject finish in menuPole.GetComponent<Pole>().finishes)
        {
            int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
            Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
            
        }
        Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
        activePath = Instantiate(ActivePathPF, transform);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().start, menuPole.GetComponent<Pole>().finishes);
    }
	
	// Update is called once per frame
	void Update () {

        



        Vector3 dist = Input.mousePosition - lastPos;
        lastPos = Input.mousePosition;
        if (activePath.GetComponent<ActivePath>().isFinished && resumed)
        {

            if (menuMap.go2(activePath.GetComponent<ActivePath>().finishes.IndexOf(activePath.GetComponent<ActivePath>().currentFinishOnPole)))
            {

                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                CreateNewPole(menuMap.pointer.size);
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
            
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
                activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = false;
                menuMap.back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
                resumed = false;
            }
        }
        else if (prevPaths.Count > 0 && resumed)
        {
            
            if (activePath.GetComponent<ActivePath>().pointer != null && activePath.GetComponent<ActivePath>().pointer.transform.position.x <= menuPole.GetComponent<Pole>().start.transform.position.x && dist.x < 0)
            {
                Destroy(menuPole);
                Destroy(activePath);
                
                activePath = prevPaths[prevPaths.Count - 1];
                
                menuPole = prevPoles[prevPoles.Count - 1];
                prevPaths.RemoveAt(prevPaths.Count - 1);
                prevPoles.RemoveAt(prevPoles.Count - 1);
                activePath.GetComponent<ActivePath>().pointer.GetComponent<follow>().notActive = false;
                menuMap.back();
                foreach (GameObject finish in GameObject.FindGameObjectsWithTag("MenuItem"))
                {
                    Destroy(finish);
                }
                foreach (GameObject finish in menuPole.GetComponent<Pole>().finishes)
                {
                    int index = menuPole.GetComponent<Pole>().finishes.IndexOf(finish);
                    Instantiate(MenuItemPF, finish.transform.position + new Vector3(10f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
        
                resumed = false;
            }
        }
        if (!activePath.GetComponent<ActivePath>().isFinished) resumed = true;
    }
}
