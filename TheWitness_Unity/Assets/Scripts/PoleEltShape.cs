using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleEltShape : MonoBehaviour
{
    public GameObject BlockPF;
    public List<List<bool>> boolList = new List<List<bool>>();
    public int size = 0;





    public void Create()
    {
        for (int i = 0; i < boolList.Count; i++)
        {
            for (int j = 0; j < boolList[0].Count; j++)
            {
                if (boolList[i][j])
                {
                    Instantiate(BlockPF, transform.position + new Vector3(j, -i, 0), BlockPF.transform.rotation).transform.parent = this.transform;
                    size++;
                }
            }
        }
        transform.Translate(new Vector3(-boolList[0].Count + 1, boolList.Count - 1, 0f) / 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
