﻿// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DayFlowLogic : SingletonBehaviour<DayFlowLogic>
{
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
        DataSystem.I.SetAttrDataByType(DataType.MaxTurn, 6);
    }

    // 一天是否继续
    public bool IsDayContinue()
    {
        var curTurn = DataSystem.I.GetAttrDataByType<int>(DataType.CurrentTurn);
        var maxTurn = DataSystem.I.GetAttrDataByType<int>(DataType.MaxTurn);
        return curTurn < maxTurn && !AttributesLogic.Instance.IsDead();
    }

    // 日循环
    public IEnumerator DayLoop()
    {
        // 重置现在回合为0
        DataSystem.I.SetAttrDataByType(DataType.CurrentTurn, 0);
        // 重新刷新卡牌
        CardPoolLogic.I.ShuffleDayCards();
        // 开始日循环
        while(IsDayContinue())
        {
            yield return TurnFLowLogic.I.TurnLoop();
        }
        // 刷新一天
        DayFlowLogic.I.IncreaseDay();
        DurFreSystem.I.UpdateDay();
    }

    // 增加天数
    public void IncreaseDay(int num = 1)
    {
        var curDay = DataSystem.I.GetAttrDataByType(DataType.Day);
        DataSystem.I.SetAttrDataByType(DataType.Day, curDay + num);
    }

    private bool ShowDayEndDialog()
    {
        // 检查是否完成一天
        var curTurn = DataSystem.I.GetAttrDataByType<int>(DataType.CurrentTurn);
        var maxTurn = DataSystem.I.GetAttrDataByType<int>(DataType.MaxTurn);
        if (curTurn >= maxTurn) {
            CommonFlowLogic.Instance.ShowDialog("一天的旅程安稳地结束了, 下一天会是什么样呢?", (answIdx) => {
            }, "呼木");
        }
        return true;
    }
}
