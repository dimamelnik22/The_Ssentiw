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
                pointer.MainName = "Main Menu";
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
        funcList[0] = LoadPoleLevel;
        funcList[2] = LoadLevelSelect;
        funcList[4] = Application.Quit;
        string[] names = {"Start", "Custom", "Debug", "Settings", "Exit" };
        menuMap.add(new MenuNode(names, funcList, 5), 0);
        funcList = new MenuFunc[4];
        funcList[0] = LoadPoleLevel;
        names = new string[]{ "Start", "Size","Complexity","Elements"};
        menuMap.add(new MenuNode(names, funcList, 4),1);
        funcList = new MenuFunc[4];
        names = new string[] { "Points", "Circles", "Stars", "Shapes" };
        menuMap.go2(1);
        menuMap.add(new MenuNode(names, funcList, 4), 3);
        funcList = new MenuFunc[5];
        names = new string[5] { "Easy", "Medium", "Hard", "Pro", "Developer" };
        menuMap.add(new MenuNode(names, funcList, 5), 1);
        menuMap.add(new MenuNode(names, funcList, 5), 2);
        menuMap.go2(3);
        for (int i = 0; i < 4; i++)
        {
            funcList = new MenuFunc[5];
            names = new string[5] { "Easy","Medium","Hard","Pro","Developer" };
            menuMap.add(new MenuNode(names, funcList, 5), i);
            menuMap.pointer.next[i].MainName += " difficulty";
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
            Instantiate(MenuItemPF, finish.transform.position + new Vector3(7f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);
            
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
                    Instantiate(MenuItemPF, finish.transform.position + new Vector3(7f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
            
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
                    Instantiate(MenuItemPF, finish.transform.position + new Vector3(7f, 0f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.Name[index]);

                }
                Instantiate(MenuItemPF, menuPole.GetComponent<Pole>().start.transform.position + new Vector3(10f, 5f, 0f), MenuItemPF.transform.rotation).GetComponent<MenuItem>().SetName(menuMap.pointer.MainName);
        
                resumed = false;
            }
        }
        if (!activePath.GetComponent<ActivePath>().isFinished) resumed = true;
    }
}
