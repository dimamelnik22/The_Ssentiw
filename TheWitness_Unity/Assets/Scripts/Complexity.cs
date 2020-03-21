using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Complexity : MonoBehaviour
{
    public enum Difficult
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }
    [Header("Coefficient")]
    [Range(5, 9)]
    public int height;
    [Range(5, 9)]
    public int width;
    public int numOfPoints = 7;
    public int quantityColor = 3;
    public int numOfClrRing = 5;
    public int numOfStars = 5;
    public int numOfShapes = 20;
    public int complexity;// some coefficiant
    public Difficult difficult;
    
    
    public static int seed = 4323;
    [HideInInspector]
    public System.Random random = new System.Random();
    
    public Difficult GetDifficult()
    {
        if(difficult != null) return difficult;
        return Difficult.Easy;//rework
    }
    public void SetSeed(int s = 0)
    {
        seed = s;
        random = s == 0 ? new System.Random() : new System.Random(seed);
    }
    public void GenerateCoefficient(Difficult dif)
    {
        if(dif == Difficult.Easy)
        {

        }
    }
    void Start()
    {
        if(DataHolder.isSet)
        { 
            height = DataHolder.height;
            width = DataHolder.width;
            seed = DataHolder.seed;
            numOfPoints = DataHolder.numOfPoints;
            quantityColor = DataHolder.quantityColor;
            numOfClrRing = DataHolder.numOfClrRing;
            numOfStars = DataHolder.numOfStars;
            numOfShapes = DataHolder.numOfShapes;
            difficult = DataHolder.difficult;
        }
    }
    void OnDestroy()
    {
        DataHolder.isSet = true;
        DataHolder.height = height;
        DataHolder.width = width;
        DataHolder.numOfPoints = numOfPoints;
        DataHolder.quantityColor = quantityColor;
        DataHolder.numOfClrRing = numOfClrRing;
        DataHolder.numOfStars = numOfStars;
        DataHolder.numOfShapes = numOfShapes;

        DataHolder.seed = seed;
        DataHolder.difficult = difficult;
}
}
