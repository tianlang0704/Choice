using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{
    public GameObject UIRoot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToRoot(GameObject go)
    {
        if (UIRoot == null) return;
        go.transform.SetParent(UIRoot.transform, false);
    }
    public void AddToRoot(Component comp)
    {
        if (UIRoot == null) return;
        AddToRoot(comp.gameObject);
    }
}
