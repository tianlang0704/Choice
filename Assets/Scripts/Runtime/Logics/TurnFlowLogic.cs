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

    }

    // 回合循环
    public IEnumerator TurnLoop()
    {
        // 问答
        ShowTurnDialog();
        // 等待进入下一回合
        yield return new WaitUntil(() => nextTurn);
        nextTurn = false;
    }

    public void NextTurn()
    {
        nextTurn = true;
    }

    public void ShowTurnDialog() {
        // 抽卡
        var card = CardPoolLogic.Instance.GetCardForTurn();
        CommonFlowLogic.Instance.ShowDialog(card.content, (ansNum) => {
            // 获取答案
            Answer answer = card.answers[ansNum];
            // 执行逻辑列表
            // DataSystem.Instance.ApplyChangeToData(answer.influenceList);
            CommonLogicSystem.I.ExecuteCommonLogic(answer.logicList);
            // 更新界面
            GameUILogic.Instance.UpdateView();
            // 下一回合
            NextTurn();
        }, card.answers.Select((a)=>a.content).ToArray());
    }
}
