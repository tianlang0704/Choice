// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameFlowLogic : SingletonBehaviour<GameFlowLogic>
{
    bool startGame = false;
    bool endGame = false;
    // Start is called before the first frame update
    void Start()
    {
        StartGameLoop();
    }

    void Init()
    {
        startGame = false;
        endGame = false;
    }

    // 每次游戏开始
    void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }

    void StopGameLoop()
    {
        StopCoroutine(GameLoop());
    }

    public bool IsGameContinue()
    {
        return !AttributesLogic.I.IsDead() && !SceneFlowLogic.I.IsSceneEnd();
    }

    IEnumerator GameLoop()
    {
        // 初始化
        DataSystem.I.Init();            // 初始化数据
        DataInfluenceSystem.I.Init();   // 初始化数据影响
        AttributesLogic.I.Init();       // 初始化属性
        FormulaSystem.I.Init();         // 初始化公式系统
        ConditionSystem.I.Init();       // 初始化条件系统
        GameScenesLogic.I.Init();       // 初始化场景
        CardLogic.I.Init();             // 初始化卡牌
        CardPoolLogic.I.Init();         // 初始化卡牌池
        ItemLogic.I.Init();             // 初始化道具
        WeatherLogic.I.Init();          // 初始化天气
        Init();                         // 初始化游戏循环
        SceneFlowLogic.I.Init();        // 初始化场景循环
        DayFlowLogic.I.Init();          // 初始化日循环
        TurnFLowLogic.I.Init();         // 初始化回合循环
        GameScenesLogic.I.SetSceneById(1);
        // 显示开始游戏
        GameUILogic.I.UpdateView();
        CommonFlowLogic.I.ShowStartDialog(() => {
            startGame = true;
        });
        yield return new WaitUntil(() => startGame);
        // 开始新游戏
        while(IsGameContinue()) {
            // 开始一天
            yield return SceneFlowLogic.I.SceneLoop();
        }
        // 检查和提示死亡
        var isDead = AttributesLogic.Instance.IsDead();
        if (isDead) {
            CommonFlowLogic.I.CheckAndNotifyDead();
        }
        // 提示结束
        var isEnd = SceneFlowLogic.I.IsCurrentSceneLastScene();
        if (isEnd) {
            CommonFlowLogic.I.ShowEndGame(() => {
                endGame = true;
            });
            yield return new WaitUntil(() => endGame);
            CommonFlowLogic.I.Town();
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
