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

    IEnumerator GameLoop()
    {
        // 初始化
        DataSystem.I.Init();        // 初始化数据
        AttributesLogic.I.Init();   // 初始化属性
        GameScenesLogic.I.Init();   // 初始化场景
        DayFlowLogic.I.Init();      // 初始化日循环
        DataSystem.I.SetAttrDataByType(AttributeType.Day, 1);
        while(!AttributesLogic.I.IsDead())
        {
            // 开始一天
            yield return DayFlowLogic.I.DayLoop();
            // 刷新一天
            DayFlowLogic.I.IncreaseDay();
            DurFreSystem.I.UpdateDay();
        }
        // 检查和提示死亡
        CommonFlowLogic.I.CheckAndNotifyDead();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
