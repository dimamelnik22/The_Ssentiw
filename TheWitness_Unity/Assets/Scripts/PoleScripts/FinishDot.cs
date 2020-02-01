using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDot : MonoBehaviour
{
    [Header("Location")]
    public GameObject dot;

    [Header("Prefabs")]
    public GameObject FinishPF;

    private GameObject finish;

    public void Create()
    {
        if (finish == null)
            finish = Instantiate(FinishPF, transform);
    }
}
