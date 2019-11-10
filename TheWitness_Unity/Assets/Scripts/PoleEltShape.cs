using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltShape : Elements
{
    public GameObject BlockPF;
    public List<List<bool>> boolList = new List<List<bool>>();
    public int size = 0;
    public List<GameObject> blocks = new List<GameObject>();



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
                    size++;
                }
            }
        }
        transform.Translate(new Vector3(-boolList[0].Count + 1, boolList.Count - 1, 0f) / 2);
    }

    public void ShowUnsolvedColor()
    {
        colorlerping = true;
        StartCoroutine(Do());
    }
    public void ShowNormalizedColor()
    {
        colorlerping = false;
		tored = true;
        foreach (GameObject block in blocks)
            block.GetComponent<Renderer>().material.color = c;
    }
    // Start is called before the first frame update
    void Start()
    {
        c = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override IEnumerator Do()
    {
        bool tored = true;
        while (colorlerping)
        {
            if (countdown > 0f)
            {
                foreach (GameObject block in blocks)
                    if (tored)
                        block.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, c, 2 * countdown);
                    else
                        block.GetComponent<Renderer>().material.color = Color.Lerp(c, Color.red, 2 * countdown);
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
