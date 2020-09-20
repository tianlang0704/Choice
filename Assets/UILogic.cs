using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class UILogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.i("Ex_弹窗").SetActive(false);
        this.i<Button>("Ex_导游").onClick.AddListener(()=>{
            ShowDialog("这是点了导游啊? 安逸哦!");
        });
        this.i<Button>("Ex_小僧").onClick.AddListener(()=>{
            ShowDialog("这是一个小僧呢!");
        });
    }

    void ShowDialog(string content, Action okAction = null, Action cancelAction = null) {
        UnityAction unityOkAction = new UnityAction(()=>{
            this.i("Ex_弹窗").SetActive(false);
            if (okAction == null) return;
            okAction();
        });
        UnityAction unityCancelAction = new UnityAction(()=>{
            this.i("Ex_弹窗").SetActive(false);
            if (cancelAction == null) return;
            cancelAction();
        });
        this.i("Ex_弹窗").SetActive(true);
        this.i("Ex_弹窗").i<TextMeshProUGUI>("Ex_弹窗内容").text = content;
        this.i("Ex_弹窗").i<Button>("Ex_YES").onClick.AddListener(unityOkAction);
        this.i("Ex_弹窗").i<Button>("Ex_No").onClick.AddListener(unityCancelAction);
        this.i("Ex_弹窗").lc(UIBase.LifeCycle.OnDisable, () => {
            this.i("Ex_弹窗").i<Button>("Ex_YES").onClick.RemoveListener(unityOkAction);
            this.i("Ex_弹窗").i<Button>("Ex_YES").onClick.RemoveListener(unityCancelAction);
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
