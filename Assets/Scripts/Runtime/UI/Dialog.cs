using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class Dialog : UILogicBase<Dialog>
{
    Action<bool> cb = null;
    // Start is called before the first frame update
    void Start()
    {
        UnityAction unityOkAction = new UnityAction(()=>{
            uiRoot.gameObject.SetActive(false);
            if (cb == null) return;
            cb(true);
        });
        UnityAction unityCancelAction = new UnityAction(()=>{
            uiRoot.gameObject.SetActive(false);
            if (cb == null) return;
            cb(false);
        });
        var yesBtn = uiRoot.i<Button>("Ex_YES");
        var noBtn = uiRoot.i<Button>("Ex_No");
        yesBtn.onClick.AddListener(unityOkAction);
        noBtn.onClick.AddListener(unityCancelAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetCB(Action<bool> a)
    {
        cb = a;
    }

    public void SetContent(string content)
    {
        uiRoot.i<TextMeshProUGUI>("Ex_弹窗内容").text = content;
    }
}
