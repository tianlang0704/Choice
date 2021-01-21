using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class ItemButton : UILogicBase<ItemButton>
{
    Action cb = null;

    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        uiRoot.i<Button>("Ex_道具").onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        uiRoot.i<Button>("Ex_道具").onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        if (cb == null) return;
        cb();
    }

    public void SetCB(Action a)
    {
        cb = a;
    }
    
    public void SetText(string t)
    {
        uiRoot.i<TextMeshProUGUI>("Ex_文字").text = t;
    }
    
    public void SetColor(Color c)
    {
        uiRoot.i<Image>("Ex_道具").color = c;
    }

    public void SetTextColor(Color c)
    {
        uiRoot.i<TextMeshProUGUI>("Ex_文字").color = c;
    }
}
