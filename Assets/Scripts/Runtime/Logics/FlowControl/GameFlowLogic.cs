// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameFlowLogic : SingletonBehaviour<GameFlowLogic>
{
    // Start is called before the first frame update
    void Start()
    {
        StartGameLoop();
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

    bool IsGameContinue()
    {
        return !AttributesLogic.I.IsDead();
    }

    IEnumerator GameLoop()
    {
        // 初始化
        DataSystem.I.Init();            // 初始化数据
        DataInfluenceSystem.I.Init();   // 初始化数据影响
        AttributesLogic.I.Init();       // 初始化属性
        GameScenesLogic.I.Init();       // 初始化场景
        CardPoolLogic.I.Init();         // 初始化卡牌池
        ItemLogic.I.Init();             // 初始化道具
        DayFlowLogic.I.Init();          // 初始化日循环
        WeatherLogic.I.Init();          // 初始化天气
        DataSystem.I.SetAttrDataByType(DataType.Day, 1);
        while(IsGameContinue())
        {
            // 开始一天
            yield return DayFlowLogic.I.DayLoop();
        }
        // 检查和提示死亡
        CommonFlowLogic.I.CheckAndNotifyDead();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
