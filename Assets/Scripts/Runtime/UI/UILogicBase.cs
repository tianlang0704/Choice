using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogicBase<T> : SingletonBehaviour<T> where T:UILogicBase<T>
{
    public UIViewBase uiRoot = null;

    virtual protected void Awake()
    {
        if (uiRoot == null) {
            uiRoot = GetComponent<UIViewBase>();
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
