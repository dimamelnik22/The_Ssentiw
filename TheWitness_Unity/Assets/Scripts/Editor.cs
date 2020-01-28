using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Editor : MonoBehaviour
{
    public GameObject PolePrefab;
    public GameObject PointPrefab;

    public GameObject PathDotPrefab;
    public GameObject PathVerticalLinePrefab;
    public GameObject PathHorizontalLinePrefab;
    public GameObject PathStartPrefab;
    public GameObject PathFinishPrefab;

    public Material PlayerWrongPathMaterial;
    public Material PlayerGoodPathMaterial;
    public Material EltPointMaterial;
    public Material EltWrongPointMaterial;

    private static Vector3 pathstepz = new Vector3(0f, 0f, -0.5f);

    private static Vector3 stepx = new Vector3(5f, 0f, 0f);
    private static Vector3 stepy = new Vector3(0f, -5f, 0f);
    //private List<GameObject> finishes = new List<GameObject>();
    public bool pathIsShown = false;
    public bool playerIsActive = false;
    public GameObject myPole;

    public int currentSize = 5;
    public InputField heightInput;
    public InputField widthInput;

    public void ButtonSavePazzl()
    {
        foreach (PoleEltPoint point in myPole.GetComponent<Pole>().eltsManager.points)
        {

            Debug.Log(point.x);
            Debug.Log(point.y);
            Debug.Log(point.down);
            Debug.Log(point.right);
        }
        /*foreach (GameObject sq in GameObject.FindGameObjectsWithTag("PoleSquere"))
        {
            var now = sq.GetComponent<PoleSquare>();
            string cod = "";
            if (now.hasElem)
            {
                byte[] bytes;
                //bytes = BitConverter.GetBytes(n.x);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.y);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.c.r);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.c.g);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.c.b);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.Type);
                //cod += BitConverter.ToString(bytes);
                //bytes = BitConverter.GetBytes(n.rotate);
                //cod += BitConverter.ToString(bytes);
                Debug.Log(cod);
                //if(now.element.) ;//!!! сделать все элементы наследованными от род класса

            }
        }*/
    }

    public void ButtonMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MenuManager.DebugMessage.clear();
    }

    public void ButtonTrySolve()
    {
        
    }

    public void ButtonResize()
    {
        Destroy(myPole);
        myPole = Instantiate(PolePrefab);
        currentSize++;
        myPole.GetComponent<Pole>().Init(System.Int32.Parse(heightInput.text), System.Int32.Parse(widthInput.text));
        myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        myPole.GetComponent<Pole>().StartScaling(myPole.GetComponent<Pole>().poleDots[0][0]);
    }

    public void CreateStartIn()
    {

    }

    public void ShowEditButtons()
    {
        foreach(var dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            dot.GetComponent<PoleDot>().ShowEditButton();
        }
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            line.GetComponent<PoleLine>().ShowEditButton();
        }
        foreach (var squere in GameObject.FindGameObjectsWithTag("PoleSquere"))
        {
            squere.GetComponent<PoleSquare>().ShowEditButton();
        }
    }

    public void HideEditButtons()
    {
        foreach (var dot in GameObject.FindGameObjectsWithTag("PoleDot"))
        {
            dot.GetComponent<PoleDot>().HideEditButton();
        }
        foreach (var line in GameObject.FindGameObjectsWithTag("PoleLine"))
        {
            line.GetComponent<PoleLine>().HideEditButton();
        }
        foreach (var squere in GameObject.FindGameObjectsWithTag("PoleSquere"))
        {
            squere.GetComponent<PoleSquare>().HideEditButton();
        }
    }
    void Start()
    {

        myPole = Instantiate(PolePrefab);
        myPole.GetComponent<Pole>().Init(5,5);
        myPole.GetComponent<Pole>().poleDots[0][0].GetComponent<PoleDot>().CreateDot();
        myPole.GetComponent<Pole>().StartScaling(myPole.GetComponent<Pole>().poleDots[0][0]);
        //myPole.GetComponent<Pole>().AddStart(x, y);
        //myPole.GetComponent<Pole>().AddFinish(x, y);
        //foreach (GameObject start in myPole.GetComponent<Pole>().starts)
        //    myPole.GetComponent<Pole>().StartScaling(start);
        //myPole.GetComponent<Pole>().GenerateShapes(Core.PolePreferences.numOfShapes);
        ////myPole.GetComponent<Pole>().GenerateShapes(100);
        //myPole.GetComponent<Pole>().SetClrRing(myPole.GetComponent<Pole>().quantityColor, myPole.GetComponent<Pole>().quantityRing);
        //myPole.GetComponent<Pole>().GeneratePoints(PolePreferences.numOfPoints);



        ////rework
        //for (int i = 0; i < myPole.GetComponent<Pole>().GetSize(); i++)
        //{
        //    for (int j = 0; j < myPole.GetComponent<Pole>().GetSize(); j++)
        //    {
        //        if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().hasPoint)
        //        {
        //            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point = Instantiate(PointPrefab, transform.position + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
        //            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<PoleEltPoint>().SetDot(myPole.GetComponent<Pole>().poleDots[i][j]);
        //            myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point);

        //            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.c = new Color(45 / 255, 104 / 255, 1);
        //            myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
        //        }
        //        if (j < myPole.GetComponent<Pole>().GetSize() - 1)
        //        {
        //            if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right != null)
        //            {
        //                if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().hasPoint)
        //                {
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepx * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right);
        //                    myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point);
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().right.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);
        //                }
        //            }
        //        }
        //        if (i < myPole.GetComponent<Pole>().GetSize() - 1)
        //        {
        //            if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down != null)
        //            {
        //                if (myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().hasPoint)
        //                {
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point = Instantiate(PointPrefab, transform.position + stepy * 0.5f + stepx * j + stepy * i + pathstepz, PointPrefab.transform.rotation).GetComponent<Elements>();
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<PoleEltPoint>().SetLine(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down);
        //                    myPole.GetComponent<Pole>().eltsManager.points.Add(myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point);
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.c = new Color(45 / 255, 104 / 255, 1);
        //                    myPole.GetComponent<Pole>().poleDots[i][j].GetComponent<PoleDot>().down.GetComponent<PoleLine>().point.GetComponent<Renderer>().material.color = new Color(45 / 255, 104 / 255, 1);

        //                }
        //            }
        //        }
        //    }
        //}

    }

    void Update()
    {

    }

}
