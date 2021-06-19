using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class Dialog : UILogicBase<Dialog>
{
    protected List<UIViewBase> answButtons = new List<UIViewBase>();
    protected Action<int> cb = null;
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

    void OnDisable()
    {
        // ResetAllAnsws();
    }
    
    public virtual void SetCB(Action<int> a)
    {
        cb = a;
    }

    public virtual void SetContent(string content)
    {
        uiRoot.i<TextMeshProUGUI>("Ex_弹窗内容").text = content;
    }

    public virtual void ResetAllAnsws()
    {
        foreach (var answ in answButtons) {
            answ.i<Button>("Ex_弹窗答案").onClick.RemoveAllListeners();
            ObjectPoolManager.I.RecycleGameObject(answ.gameObject);
        }
        answButtons.Clear();
        var answHost = uiRoot.i<RectTransform>("Ex_答案框");
        answHost.gameObject.SetActive(false);
    }

    public virtual void ShowAnsw(string content)
    {
        if (string.IsNullOrEmpty(content)) return;
        // 加载答案按钮
        var answ = ObjectPoolManager.Instance.GetGameObject<UIViewBase>(Constants.UIBasePath + Constants.UICardAnswer);
        var answHost = uiRoot.i<RectTransform>("Ex_答案框");
        answHost.gameObject.SetActive(true);
        var parent = uiRoot.i<RectTransform>("Ex_答案内容框");;
        answ.transform.SetParent(parent, false);
        // 设置文字
        var text = answ.i<TextMeshProUGUI>("Ex_Text");
        text.text = content;
        // 添加按钮回调
        var index = answButtons.Count;
        answ.i<Button>("Ex_弹窗答案").onClick.AddListener(() => {
            uiRoot.gameObject.SetActive(false);
            if (cb == null) return;
            cb(index);
        });
        // 添加到答案列表
        answButtons.Add(answ);
    }

    public virtual void SetColor(Color color)
    {
        var frame = uiRoot.GetComponent<Image>();
        frame.color = color;
    }
}
