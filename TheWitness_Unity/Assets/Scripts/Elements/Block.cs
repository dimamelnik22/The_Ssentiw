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

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        normals = new Vector3[vertices.Length];
        int[] triangles = new int[xSize * ySize * 6];


        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                SetVertex(i, (float)x, (float)y);
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }


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
