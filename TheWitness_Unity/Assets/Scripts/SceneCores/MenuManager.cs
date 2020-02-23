using System.Collections;
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
    private MenuBuilder.MenuLinkedList menuMap;
    private Vector3 lastPos;
    private bool buttonPressed = false;
    delegate void MenuFunc();
    //????
    public static class DebugMessage
    {
        private static int seed = new int();
        private static string s;
        public static string path;
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
    }
    private static void ConvertPreferences()
    {
        Debug.Log("CONVERT");
    }
    public static void LoadPoleLevel()
    {
        Core.PolePreferences.mode = "normal";
        Core.PolePreferences.MyRandom.SetSeed();
        ConvertPreferences();
        SceneManager.LoadScene("PoleLevel");
    }
    public static void LoadTutorial()
    {
        Core.PolePreferences.info = "";
        SceneManager.LoadScene("Introduction");
    }
    public static void LoadRandomPoleLevel()
    {
        Core.PolePreferences.mode = "normal";
        Core.PolePreferences.MyRandom.SetSeed();
        Core.PolePreferences.height = 5 + Core.PolePreferences.MyRandom.GetRandom() % 5;
        Core.PolePreferences.width = 5 + Core.PolePreferences.MyRandom.GetRandom() % 5;
        Core.PolePreferences.complexity = Mathf.RoundToInt(Core.PolePreferences.width * Core.PolePreferences.height * (4 + Core.PolePreferences.MyRandom.GetRandom() % 4) / 10);
        //redo
        Core.PolePreferences.numOfCircles = Core.PolePreferences.width + Core.PolePreferences.MyRandom.GetRandom() % Core.PolePreferences.width;
        Core.PolePreferences.numOfPoints = Core.PolePreferences.width + Core.PolePreferences.MyRandom.GetRandom() % Core.PolePreferences.width;
        //
        Core.PolePreferences.numOfShapes = Mathf.RoundToInt(Core.PolePreferences.width * Core.PolePreferences.height * (1 + Core.PolePreferences.MyRandom.GetRandom() % 4) / 10);
        SceneManager.LoadScene("PoleLevel");
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
    public void CreateNewSlider(int size)
    {
        prevPaths.Add(activePath);
        activePath.GetComponent<ActivePath>().pointer.SetActive(false);
        prevPoles.Add(menuPole);
        menuPole = Instantiate(PolePF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        
        activePath = Instantiate(ActivePathPF, activePath.GetComponent<ActivePath>().currentFinish.transform.position + new Vector3(0f, 0f, 0.5f), transform.rotation);
        activePath.GetComponent<ActivePath>().Init(menuPole, menuPole.GetComponent<Pole>().starts, menuPole.GetComponent<Pole>().finishes);
        activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
    }


    public static void ParseLevels(string lvlSetName)
    {
        string[] lvls = (Resources.Load(lvlSetName) as TextAsset).text.Split('\n');

        Core.LevelList = new List<string>(lvls);
        if (Core.LevelList[Core.LevelList.Count - 1] == "")
            Core.LevelList.Remove(Core.LevelList[Core.LevelList.Count - 1]);

    }

    public void PressButton(int indexOfMenuItem)
    {
        buttonPressed = true;
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
                //activePath.GetComponent<ActivePath>().pointer.SetActive(true);
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

                //activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);


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
               //activePath.GetComponent<ActivePath>().pointer.SetActive(true);
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
                //activePath.GetComponent<ActivePath>().NewStart(menuPole.GetComponent<Pole>().starts[0]);
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(-2.5f, 0f, 0f);
            }
        }
        else Debug.Log("smth is wrong");
    }
    // Use this for initialization
    void Start () {
        // Menu structure Generation
        menuMap = MenuBuilder.BuildMenu();



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
        if (buttonPressed && Input.GetMouseButton(0) == false)
        {
            activePath.GetComponent<ActivePath>().pointer.SetActive(true);
        }
#else
        if (Input.touchCount > 0)
            {

                GetComponent<ParticleSystem>().Play();
                var touch = Input.GetTouch(0);
                lastPos = touch.position;
            }
        if (buttonPressed && Input.touchCount == 0)
        {
            activePath.GetComponent<ActivePath>().pointer.SetActive(true);
        }
#endif

        if (activePath.GetComponent<ActivePath>().isFinished && (Input.GetMouseButton(0) == false) && (Input.touchCount == 0))
        {

            if (menuMap.Go2(activePath.GetComponent<ActivePath>().finishes.IndexOf(activePath.GetComponent<ActivePath>().currentFinishOnPole)))
            {
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
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(-2.5f, 0f, 0f);
                activePath.GetComponent<ActivePath>().Update();
				
            }
        }
        else if (prevPaths.Count > 0 )
        {
            
            if (activePath.GetComponent<ActivePath>().pointer != null && activePath.GetComponent<ActivePath>().pointer.transform.position.x <= menuPole.GetComponent<Pole>().starts[0].transform.position.x && (Input.GetMouseButton(0) == false) && (Input.touchCount == 0))
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
       
                activePath.GetComponent<ActivePath>().pointer.transform.Translate(-2.5f, 0f, 0f);
                activePath.GetComponent<ActivePath>().Update();

            }
        }
        
    }
}
