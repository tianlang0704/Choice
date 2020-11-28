using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBButton : Button
{
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    override protected void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveAllListeners();
    }
}
