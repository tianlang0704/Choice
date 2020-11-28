using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class TownUILogic : SingletonBehaviour<TownUILogic>
{
    const string resStrFormat = "<sprite name=\"{0}\">";

    public UIBase uiRoot;

    // Start is called before the first frame update
    void Start()
    {
        uiRoot.i<Button>("Ex_开始按钮").onClick.AddListener(() => {
            CommonFlowLogic.Instance.StartGame();
        });
        uiRoot.i<Button>("Ex_日志").onClick.AddListener(() => {
            CommonFlowLogic.Instance.Diary();
        });
        uiRoot.i<Button>("Ex_退出").onClick.AddListener(() => {
            CommonFlowLogic.Instance.Quit();
        });
        UpdateView();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateView() {

    }
}
