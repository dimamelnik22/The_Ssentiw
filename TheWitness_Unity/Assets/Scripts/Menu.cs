using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    delegate void MenuFunc();
    class MenuNode
    {
        public string MainName;
        public string[] Name;
        public MenuFunc[] Func;
        public MenuNode prev;
        public MenuNode[] next;
        MenuNode(string MainName,string[] Name, MenuFunc[] Func, MenuNode prev, MenuNode[] next)
        {
            this.MainName = MainName;
            this.Name = Name;
            this.Func = Func;
            this.next = next;
            this.prev = prev;
        }
        MenuNode(string[] Name, MenuFunc[] Func, MenuNode prev, MenuNode[] next)
        {
            this.Name = Name;
            this.Func = Func;
            this.next = next;
            this.prev = prev;
        }
    }
    class MenuLinkedList
    {
        MenuNode node;
        public void add(MenuNode next, int n)
        {
            if(node == null)
            {
                node = next;
            }
            else
            {
                next.MainName = node.Name[n];
                node.next[n] = next;
                next.prev = node;
            }
        }
        public void go2(int n)  
        {
            if(node.next.Length > n)
            {
                if (node.next[n] != null)
                { 
                    node = node.next[n];
                }
                else if(node.Func[n] != null)
                {
                    node.Func[n]();
                }
            }
            else
            {
                Debug.Log("you go in don't sel path line 45");
            }
        }
        public void back()
        {
            if (node.prev != null)
            {
                node = node.prev;
            }
            else
            {
                Debug.Log("you go in don't sel path line 56");
            }
        }
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
