using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;

public class TownUILogic : UILogicBase<TownUILogic>
{
    Dragger dragger = new Dragger();
    const string resStrFormat = "<sprite name=\"{0}\">";

    // Start is called before the first frame update
    void Start()
    {
        uiRoot.i<Button>("Ex_开始按钮").onClick.AddListener(() => {
            CommonFlowLogic.Instance.StartGame();
        });
        // uiRoot.i<Button>("Ex_日志").onClick.AddListener(() => {
        //     CommonFlowLogic.Instance.Diary();
        // });
        // uiRoot.i<Button>("Ex_退出").onClick.AddListener(() => {
        //     CommonFlowLogic.Instance.Quit();
        // });
        // UpdateView();
    }

    void OnEnable()
    {
        dragger.dragEndHandler += OnDragEnd;
    }

    void OnDisable()
    {
        dragger.dragEndHandler -= OnDragEnd;
    }

    void OnDragEnd()
    {
        var start = gameObject.i<PlayableDirector>("Ex_开始");
        if (dragger.dragPosDelta.x < 300) {
            start.time = 0;
            start.Evaluate();
            start.Stop();
            return;
        }
        var end = gameObject.i<PlayableDirector>("Ex_离开");
        end.Play();
        StartCoroutine(CoroutineUtils.DelaySeconds(() => {
            CommonFlowLogic.Instance.StartGame();
        }, 0.26f));
    }

    // Update is called once per frame
    void Update()
    {
        dragger.UpdateDrag();
        UpdateGo();
    }

    void UpdateGo()
    {
        if (!dragger.isDragging) return;
        var start = gameObject.i<PlayableDirector>("Ex_开始");
        if (start.state != PlayState.Playing) {
            start.Play();
        }
        start.time = start.duration * dragger.dragPosDelta.x / 400;
    }

    // public void UpdateView() {

    // }
}
