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
        var curDistance = DataSystem.I.GetDataByType<int>(DataType.Distance);
        var sceneDistance = DataSystem.I.GetDataByType<int>(DataType.SceneMaxDistance);
        var isTurnContinue = curDistance < sceneDistance;
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
        // 场景束后, 检查是否是最后一个场景, 如果是就直接结束
        if (IsCurrentSceneLastScene()) {
            sceneEnd = true;
            yield break;
        }
        // 选择下一个场景
        CommonFlowLogic.I.ShowSelectSceneDialog();
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
