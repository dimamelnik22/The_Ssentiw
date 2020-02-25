using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    private int xSize = 10;
    float roundness;
    private int ySize = 10;
    void Start()
    {
        Generate();
    }
    public void Generate()
    {
        roundness = 3;
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        Color normal = GetComponent<Renderer>().material.color;

        //Color[] colors = new Color[(xSize + 1)];

        vertices = new Vector3[(xSize + ySize+2) * 2];
        normals = new Vector3[vertices.Length];
        int[] triangles = new int[(xSize + ySize+2) * 6];
        vertices[0] = new Vector3(0, 0);

        {
            int i = 1;
            int y = 0;
            int x = 0;
            for (; x <= xSize; x++, ++i)
            {
                SetVertex(i, (float)x, (float)y);

            }
            for (++y; y <= ySize; y++, ++i)
            {
                SetVertex(i, (float)x, (float)y);
            }
            for (--x; x >= 0; x--, ++i)
            {
                SetVertex(i, (float)x, (float)y);

            }
            for (--y; y >= 0; y--, ++i)
            {
                SetVertex(i, (float)x, (float)y);
            }
        }
        for (int i = 0; i < (xSize + ySize+2) * 2-1; ++i)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i;
        }
        triangles[3 * ((xSize + ySize+2) * 2 - 1)] = 0;
        triangles[3 * ((xSize + ySize+2) * 2 - 1) + 1] = 1;
        triangles[3 * ((xSize + ySize+2) * 2 - 1) + 2] = (xSize + ySize + 2) * 2 -1;
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;


        mesh.RecalculateNormals();
    }
    private void SetVertex(int i, float x, float y)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y);

        if (x < roundness)
        {
            inner.x = roundness;
        }
        else if (x > xSize - roundness)
        {
            inner.x = xSize - roundness;
        }
        if (y < roundness)
        {
            inner.y = roundness;
        }
        else if (y > ySize - roundness)
        {
            inner.y = ySize - roundness;
        }
        normals[i] = (vertices[i] - inner).normalized;
        Vector3 delta = new Vector3(-50/9, -50/9);
        vertices[i] = inner + normals[i] * roundness + delta;
    }
}
