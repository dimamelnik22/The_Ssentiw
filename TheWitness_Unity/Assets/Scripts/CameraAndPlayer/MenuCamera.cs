using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour {
    public GameObject Menu;

	void Update () {
        Vector3 pos = Menu.GetComponent<MenuManager>().menuPole.transform.position + new Vector3(10f,-10f,-35f);
        if (Mathf.Abs(Vector3.Distance(transform.position, pos)) > 1f)
            transform.Translate((pos - transform.position).normalized * 0.45f);
    }
}
