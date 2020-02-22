using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltClrRing : Elements {
    private Mesh mesh;
    private static int NumOfPoint = 32;
    private Vector3[] vertices;

    private void Awake()
    {
        Generate();
    }
    private void Generate()
    {
        vertices = new Vector3[NumOfPoint + 1];
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        
        float r1 = 0.5f;
        float r2 = 0.35f;
        for (int i = 0; i < NumOfPoint ; ++i)
        {
            float dx;
            float dy;
            float angle = 2.0f * 3.1415926f * (float)i / (float)NumOfPoint;
            if (i % 2 == 0)
            {
                dx = r1 * Mathf.Cos(angle);
                dy = r1 * Mathf.Sin(angle);
            }
            else
            {
                dx = r2 * Mathf.Cos(angle);
                dy = r2 * Mathf.Sin(angle);
            }
            vertices[i] = new Vector3(dx, dy);
        }
        //for (int i = 0, y = 0; y <= ySize; y++)
        //{
        //    for (int x = 0; x <= xSize; x++, i++)
        //    {
        //        vertices[i] = new Vector3(x, y);
        //    }
        //}
        mesh.vertices = vertices;
        int[] triangles = new int[(NumOfPoint + 1) * 3];
        for (int i = 0; i<NumOfPoint-2; i+=2)
        {
            triangles[3 * i+1] = triangles[3 * (i + 1) + 1] = i + 1;
            triangles[3 * i  ] = i;
            triangles[3 * i + 2] = triangles[3 * (i + 1)] = i + 2;
            triangles[3 * (i+1)+2] = i + 3;
        }
        triangles[3 * (NumOfPoint - 1)] = triangles[3 * (NumOfPoint - 2) + 2] = 0;
        triangles[3 * (NumOfPoint - 1) + 1] = triangles[3 * (NumOfPoint - 2) + 1] = NumOfPoint - 1;
        triangles[3 * (NumOfPoint - 1) + 2] = 1;
        triangles[3 * (NumOfPoint - 2)] = NumOfPoint - 2;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
