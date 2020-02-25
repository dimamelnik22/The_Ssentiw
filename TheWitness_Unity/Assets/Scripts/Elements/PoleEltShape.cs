﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltShape : Elements
{
    [Header("Prefabs")]
    public GameObject BlockPF;

    public bool rot = false;

    [HideInInspector]
    public List<List<bool>> boolList = new List<List<bool>>();
    
    private readonly List<GameObject> blocks = new List<GameObject>();

    public void OnDestroy()
    {
        foreach (var block in blocks)
            Destroy(block);
    }

    public int GetSize()
    {
        int size = 0;
        foreach (List<bool> line in boolList)
            foreach (bool bit in line)
                if (bit)
                    size++;
        return size;
    }
    public void Create()
    {
        for (int i = 0; i < boolList.Count; i++)
        {
            for (int j = 0; j < boolList[0].Count; j++)
            {
                if (boolList[i][j])
                {
                    blocks.Add(Instantiate(BlockPF, transform.position + new Vector3(j, -i, 0), BlockPF.transform.rotation));
                    blocks[blocks.Count - 1].transform.parent = this.transform;
                }
            }
        }
        c = new Material(blocks[0].GetComponent<Renderer>().material);
        int size = Mathf.Max(boolList.Count, boolList[0].Count);
        float scale = 1f;
        if (size >= 4)
        {
            scale = 3.7f / size;
        }
        else if (size <= 3)
        {
            scale = 1f + 0.5f / size;
        }
        if (rot) scale *= 0.85f;
        transform.localScale = new Vector3(scale,scale,1f);
        foreach (var block in blocks)
        {
            block.transform.Translate(new Vector3(-boolList[0].Count + 1, boolList.Count - 1, 0f) / 2 * scale);
        }
        //transform.Translate(new Vector3(-0.3f, 0f, 0f) / 2 * scale);
        if (rot) transform.Rotate(new Vector3(0f, 0f, 20f));
    }

    public new void ShowUnsolvedColor()
    {
        colorlerping = true;
        StartCoroutine(Do());
    }
    public new void ShowNormalizedColor()
    {
        colorlerping = false;
        foreach (GameObject block in blocks)
            block.GetComponent<Renderer>().material.color = c.color;
    }
    // Start is called before the first frame update
    public override IEnumerator Do()
    {
        bool tored = true;
        while (colorlerping)
        {
            if (countdown > 0f)
            {
                foreach (GameObject block in blocks)
                    if (tored)
                        block.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, c.color, 2 * countdown);
                    else
                        block.GetComponent<Renderer>().material.color = Color.Lerp(c.color, Color.red, 2 * countdown);
            }
            else
            {
                countdown = 0.5f;
                tored = !tored;
            }
            countdown -= Time.deltaTime;
            yield return null;
        }
    }
}
