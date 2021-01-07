// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnFLowLogic : SingletonBehaviour<TurnFLowLogic>
{
    private bool nextTurn = false;
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
        DataSystem.I.SetAttrDataByType(DataType.MaxTurn, 7);
    }

    // 回合循环
    public IEnumerator TurnLoop()
    {
        // 增加回合数
        IncreaseTurn(1);
        // 更新界面
        GameUILogic.I.UpdateView();
        // 问答
        ShowTurnDialog();
        // 等待进入下一回合
        yield return new WaitUntil(() => nextTurn);
        nextTurn = false;
        // 检查死亡
        if (AttributesLogic.I.IsDead()) yield break;
        // 更新
        DurFreSystem.I.UpdateTurn();
        // 稍微等下下再结束本回合
        yield return new WaitForSeconds(0.1f);
    }

    // 下一回合
    public void NextTurn()
    {
        nextTurn = true;
    }

    // 增加回合数
    public void IncreaseTurn(int turnNum = 1)
    {
        var curTurn = DataSystem.I.GetAttrDataByType<int>(DataType.CurrentTurn);
        DataSystem.I.SetAttrDataByType(DataType.CurrentTurn, curTurn + turnNum);
    }

    // 显示回合提问
    public void ShowTurnDialog() {
        // 抽卡
        var card = CardPoolLogic.I.RerollTurnCard();
        // 处理没有卡用了
        // if (card == null) {

        // }
        CommonFlowLogic.I.ShowDialog(
            card.content, 
                (ansNum) => {
                // 获取答案
                Answer answer = card.answers[ansNum];
                // 执行逻辑列表
                if (answer.logicListFunc != null) {
                    var logicList = answer.logicListFunc();
                    CommonLogicSystem.I.ExecuteCommonLogic(logicList);
                }
                // 更新界面
                GameUILogic.I.UpdateView();
                // 下一回合
                NextTurn();
            }, 
            card.answers
                .Where((a)=>ConditionSystem.I.IsConditionMet(a.condition))
                .Select((a)=>a.content)
                .ToArray()
        );
    }

    // 跳过一回合
    public void SkipTurn(int num = 1)
    {
        CommonFlowLogic.I.CloseDialog();
        if (num > 1)
            IncreaseTurn(num - 1); // 会自增1回合
        NextTurn();
    }
}
