// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneFlowLogic : SingletonBehaviour<SceneFlowLogic>
{
    private bool nextScene = false;
    private bool sceneEnd = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        nextScene = false;
        sceneEnd = false;
    }

    // 一天是否继续
    public bool IsSceneContinue()
    {
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        var maxTurn = DataSystem.I.GetDataByType<int>(DataType.MaxTurn);
        var isTurnContinue = curTurn < maxTurn;
        var isGameContinue = GameFlowLogic.I.IsGameContinue();
        return isTurnContinue && isGameContinue;
    }

    // 日循环
    public IEnumerator SceneLoop()
    {
        // 更新界面
        GameUILogic.I.UpdateView();
        // 检查是否继续
        if (!IsSceneContinue()) yield break;
        // 开始场景循环
        while(IsSceneContinue()) {
            yield return DayFlowLogic.I.DayLoop();
        }
        if (IsCurrentSceneLastScene()) {
            sceneEnd = true;
            yield break;
        }
        // 场景循环结束
        CommonFlowLogic.I.ShowSelectSceneDialog();
        // 等待进入下一场景
        yield return new WaitUntil(() => nextScene);
        nextScene = false;
        // 检查是否继续
        if (!IsSceneContinue()) yield break;
    }

    public bool IsCurrentSceneLastScene()
    {
        var currentScene = DataSystem.I.GetDataByType<int>(DataType.Scene);
        if (currentScene == ((int)SceneType.End) - 1) {
            return true;
        }
        return false;
    }

    public void NextScene()
    {
        nextScene = true;
    }

    public bool IsSceneEnd()
    {
        return sceneEnd;
    }
}
