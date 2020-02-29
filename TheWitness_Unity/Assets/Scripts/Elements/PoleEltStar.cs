using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltStar : Elements
{
    private Mesh mesh;
    private static int NumOfPoint = 16;
    private Vector3[] vertices;

    private void Awake()
    {
    }
    public void Generate()
    {
        vertices = new Vector3[NumOfPoint + 1];
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        float r1 = 0.7f;
        float r2 = 0.5f;
        vertices[0] = new Vector3(0, 0);
        for (int i = 1; i < NumOfPoint; ++i)
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

        int[] triangles = new int[(NumOfPoint + 1) * 3];
        for (int i = 0; i < NumOfPoint; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        Debug.Log("all good");

        mesh.RecalculateNormals();
    }
}
