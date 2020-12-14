using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class Dialog : UILogicBase<Dialog>
{
    List<Button> answButtons = new List<Button>();
    Action<int> cb = null;
    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
        answButtons.Add(uiRoot.i<Button>("Ex_A1"));
        answButtons.Add(uiRoot.i<Button>("Ex_A2"));
        answButtons.Add(uiRoot.i<Button>("Ex_A3"));
        answButtons.Add(uiRoot.i<Button>("Ex_A4"));
        for (int i = 0; i < answButtons.Count; i++)
        {
            var button = answButtons[i];
            var index = i; //i不能被闭包捕获
            button.onClick.AddListener(() => {OnCallback(index);});
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCallback(int ansNum)
    {
        uiRoot.gameObject.SetActive(false);
        if (cb == null) return;
        cb(ansNum);
    }
    
    public void SetCB(Action<int> a)
    {
        cb = a;
    }

    public void SetContent(string content)
    {
        uiRoot.i<TextMeshProUGUI>("Ex_弹窗内容").text = content;
    }

    public void ResetAllAnsws()
    {
        foreach (var button in answButtons)
        {
            // button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    public void ShowAnsw(int i, string content)
    {
        if (i >= answButtons.Count || string.IsNullOrEmpty(content)) return;
        var button = answButtons[i];
        button.gameObject.SetActive(true);
        if (i >= answButtons.Count) return;
        var text = button.i<TextMeshProUGUI>("Ex_Text");
        if (text == null) return;
        text.text = content;
    }
}
