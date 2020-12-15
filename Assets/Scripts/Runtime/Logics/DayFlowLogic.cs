// using System;
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
        DataSystem.I.SetAttrDataByType(AttributeType.MaxTurn, 6);
    }

    // 日循环
    public IEnumerator DayLoop()
    {
        // 重置现在回合为0
        DataSystem.I.SetAttrDataByType(AttributeType.CurrentTurn, 0);
        // 重新刷新卡牌
        CardPoolLogic.I.ShuffleDayCards();
        // 开始日循环
        var curTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.CurrentTurn);
        var maxTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.MaxTurn);
        while(curTurn < maxTurn)
        {
            yield return TurnFLowLogic.I.TurnLoop();
            if (AttributesLogic.Instance.IsDead()) yield break;
            // 刷新回合记录
            TurnFLowLogic.I.IncreaseTurn();
            curTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.CurrentTurn);
            maxTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.MaxTurn);
            DurFreSystem.I.UpdateTurn();
            // 更新界面
            GameUILogic.Instance.UpdateView();
            // 稍微等下下再进入下一回合
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void IncreaseDay()
    {
        var curDay = DataSystem.I.GetAttrDataByType(AttributeType.Day);
        DataSystem.I.SetAttrDataByType(AttributeType.Day, curDay + 1);
    }

    private bool ShowDayEndDialog()
    {
        // 检查是否完成一天
        var curTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.CurrentTurn);
        var maxTurn = DataSystem.I.GetAttrDataByType<int>(AttributeType.MaxTurn);
        if (curTurn >= maxTurn) {
            CommonFlowLogic.Instance.ShowDialog("一天的旅程安稳地结束了, 下一天会是什么样呢?", (answIdx) => {
            }, "呼木");
        }
        return true;
    }
}
