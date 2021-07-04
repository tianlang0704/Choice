// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DayFlowLogic : SingletonBehaviour<DayFlowLogic>
{
    bool isNextDay = false;
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
        DataSystem.I.SetDataByType(DataType.DayMaxTurn, 7);
    }

    // 一天是否继续
    public bool IsDayContinue()
    {
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        var dayMaxTurn = DataSystem.I.GetDataByType<int>(DataType.DayMaxTurn);
        curTurn = curTurn == 0 ? 1 : curTurn;
        var curDay = DataSystem.I.GetDataByType<int>(DataType.CurrentDay);
        var inDayTime = (float)((float)curTurn / dayMaxTurn) - curDay;
        var isDayContinue = inDayTime < 0;
        return SceneFlowLogic.I.IsSceneContinue() && isDayContinue;
    }

    // 日循环
    public IEnumerator DayLoop()
    {
        // 增加一天
        var curDay = DataSystem.I.GetDataByType(DataType.CurrentDay);
        DataSystem.I.SetDataByType(DataType.CurrentDay, curDay + 1);
        // 刷新一天
        DurFreSystem.I.UpdateDay();
        ItemLogic.I.UpdateDay();
        // 更新界面
        GameUILogic.I.UpdateView();
        // 检查是否继续
        if (!IsDayContinue()) yield break;
        // 开始日循环
        while(IsDayContinue())
        {
            yield return TurnFLowLogic.I.TurnLoop();
        }
        // 一天结束
        CommonFlowLogic.I.ShowRest(() => {
            // 下一回合
            NextDay();
        });
        // 等下一天
        yield return new WaitUntil(() => isNextDay);
        isNextDay = false;
    }

    public void NextDay()
    {
        isNextDay = true;
    }
}
