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
        DataSystem.I.SetDataByType(DataType.MaxTurn, 7);
    }

    // 回合循环
    public IEnumerator TurnLoop()
    {
        // 更新回合
        IncreaseTurn(1);
        DurFreSystem.I.UpdateTurn();
        ItemLogic.I.UpdateTurn();
        // 更新界面
        GameUILogic.I.UpdateView();
        // 检查是否继续
        if (!IsTurnContinue()) yield break;
        // 问答
        ShowTurnDialog();
        GameUILogic.I.UpdateView();
        // 等待进入下一回合
        DataSystem.I.RestDataChange();
        yield return new WaitUntil(() => nextTurn);
        nextTurn = false;
        // 检查是否继续
        if (!IsTurnContinue()) yield break;
        // 稍微等下下再结束本回合
        yield return new WaitForSeconds(0.1f);
    }

    // 回合是否继续
    public bool IsTurnContinue()
    {
        return !AttributesLogic.I.IsDead();
    }

    // 下一回合
    public void NextTurn()
    {
        var isPreventNextTurnOnce = DataSystem.I.CopyAttrDataWithInfluenceByType<float>(DataType.IsPreventNextTurnOnce);
        if (isPreventNextTurnOnce == 0) {
            nextTurn = true;
        } else {
            DataSystem.I.SetDataByType(DataType.IsPreventNextTurnOnce, 0f);
        }
    }

    // 增加回合数
    public void IncreaseTurn(int turnNum = 1)
    {
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        DataSystem.I.SetDataByType(DataType.CurrentTurn, curTurn + turnNum);
    }

    // 显示回合提问
    public void ShowTurnDialog() {
        // 抽卡
         CardPoolLogic.I.RerollTurnCard();
        var card = CardPoolLogic.I.GetTurnCardInstance() ?? CardLogic.I.GetCardCopyById(GameUtil.CardId(10002)); // 抽到卡或者通用提示卡
        // // 处理没有卡用了
        // if (card == null) {
        //     // 更新界面
        //     GameUILogic.I.UpdateView();
        //     // 下一回合
        //     NextTurn();
        //     return;
        // }
        // 显示卡片
        var color = GameUtil.CardQualityToColor(card.Quality);
        CommonFlowLogic.I.ShowDialogWithColor(
            card.content, 
            color,
            (ansNum) => {
                // 获取答案
                Answer answer = card.answers[ansNum];
                // 执行逻辑列表
                if (answer.logicListFuncList != null) {
                    var logicList = answer.logicListFuncList
                        .Select((func)=>func())
                        .SelectMany((logicExeutionList)=>logicExeutionList)
                        .ToList();
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
