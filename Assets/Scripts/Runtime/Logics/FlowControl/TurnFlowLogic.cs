﻿// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnFLowLogic : SingletonBehaviour<TurnFLowLogic>
{
    private bool nextTurn = false;
    private bool workOrRun = false;
    private bool isWork = false;
    private int skipTurn = 0;
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

    // 回合是否继续
    public bool IsTurnContinue()
    {
        return DayFlowLogic.I.IsDayContinue();
    }

    // 回合循环
    public IEnumerator TurnLoop()
    {
        // 更新回合
        DurFreSystem.I.UpdateTurn();
        ItemLogic.I.UpdateTurn();
        // 更新界面
        GameUILogic.I.UpdateView();
        // 检查是否继续
        if (!IsTurnContinue()) yield break;
        // 更新本回合卡牌类型
        CardPoolLogic.I.UpdateCardType();
        // 检查是否跳过
        if (skipTurn <= 0) {
            // 选择打工还是跑路
            isWork = false;
            CommonFlowLogic.I.ShowWorkOrRun((a) => {
                workOrRun = true;
                if (a % 2 == 0) {
                    isWork = true;
                }
                IncreaseTurnAndDistance(!isWork);
            });
            yield return new WaitUntil(() => workOrRun);
            workOrRun = false;
            yield return new WaitForSeconds(0.1f);
            GameUILogic.I.UpdateView();
        } else {
            skipTurn -= 1;
            IncreaseTurnAndDistance(!isWork);
        }
        // 如果不是打工, 继续跑路下一个回合
        if (!isWork) {
            var cardType = (CardType)DataSystem.I.GetDataByType<int>(DataType.TurnCardType);
            if (cardType != CardType.Blank) {
                // 不是白卡就抽接下来的卡
                CardPoolLogic.I.RerollTurnCard();
                GameUILogic.I.UpdateView();
                CommonFlowLogic.I.ShowTurnDialog();
                // 等待进入下一回合
                yield return new WaitUntil(() => nextTurn);
                nextTurn = false;
            }
        }
        // 清除数据改变
        DataSystem.I.RestDataChange();
        GameUILogic.I.UpdateView();
        // 稍微等下下再结束本回合
        yield return new WaitForSeconds(0.1f);
    }

    public void IncreaseTurnAndDistance(bool isIncreaseDistance = true)
    {
        if (isIncreaseDistance) {
            AttributesLogic.I.ChangeDistance(1);
        }
        AttributesLogic.I.ChangeTurn(1);
        GameUILogic.I.UpdateView();
    }

    // 跳过一回合
    public void SkipTurn(int num = 1)
    {
        CommonFlowLogic.I.CloseDialog();
        if (num > 1) {
            AttributesLogic.I.ChangeTurn(num - 1); // 会自增1回合
        }
        NextTurn();
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

    public void PassWorkOrRun()
    {
        workOrRun = true;
    }
}
