using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DIS = DataInfluenceSystem;

public enum Logic {
    AttrChange = 0,     // 永久改变数值
    AttrChangeHurt,     // 永久改变数值, 但是先检查护身符, 受伤害因数影响
    AttrChangeIncome,   // 永久改变数值, 受收益因数影响
    AttrChangeCost,     // 永久改变数值, 费用
    AttrInfluence,      // 暂时影响数值
    _ATTR_MAX_ = 99,     // 属性改变分割值
    AddItem,            // 添加道具
    AddItemWithDuration,// 添加道具
    UseItem,            // 使用道具
    RemoveItem,         // 移除道具
    SkipTurn,           // 跳过回合
    SetScene,           // 设置场景
    ShowSelectScene,    // 显示选择场景
}

public class LogicExecution {
    public Logic Logic;
    public object Param;
    public Condition Condition;
}

public class CommonLogicSystem : SingletonBehaviour<CommonLogicSystem>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<LogicExecution> GetAttrIncome(List<AttrInfluence> influ, Condition c = null)
    {
        return GetLogicList(new List<(Logic l, object p, Condition c)>(){(Logic.AttrChangeIncome, influ, c)});
    }

    public List<LogicExecution> GetAttrHurt(List<AttrInfluence> influ, Condition c = null)
    {
        return GetLogicList(new List<(Logic l, object p, Condition c)>(){(Logic.AttrChangeHurt, influ, c)});
    }

    public List<LogicExecution> GetLogicList(List<(Logic l, object p, Condition c)> exeList)
    {
        var leList = exeList.Select((o) => {
            return new LogicExecution() { Logic = o.l, Param = o.p, Condition = o.c};
        }).ToList();
        return leList;
    }

    public void ExecuteCommonLogic(Func<List<LogicExecution>> leListFunc)
    {
        if (leListFunc == null) return;
        var leList = leListFunc();
        if (leList.Count <= 0) return;
        ExecuteCommonLogic(leList);
    }

    public void ExecuteCommonLogic(List<LogicExecution> leList)
    {
        if (leList == null) return;
        foreach (var le in leList) {
            ExecuteCommonLogic(le);
        }
    }

    public void ExecuteCommonLogic(LogicExecution le)
    {
        if (le.Condition != null) {
            if (!ConditionSystem.I.IsConditionMet(le.Condition)) return;
        }
        ExecuteCommonLogic(le.Logic, le.Param);
    }

    public void ExecuteCommonLogic(Logic logic, object param)
    {
        if (logic == Logic.AttrChange) {
            AttrChange(param);
        } else if (logic == Logic.AttrChangeHurt) {
            AttrChangeHurt(param);
        } else if (logic == Logic.AttrChangeIncome) {
            AttrChangeIncome(param);
        } else if (logic == Logic.AttrChangeCost) {
            AttrChangeCost(param);
        } else if (logic == Logic.AttrInfluence) {
            AttrInfluence(param);
        } else if (logic == Logic.AddItem) {
            AddItem(param);
        } else if (logic == Logic.AddItemWithDuration) {
            AddItemWithDuration(param);
        } else if (logic == Logic.UseItem) {
            UseItem(param);
        } else if (logic == Logic.RemoveItem) {
            RemoveItem(param);
        } else if (logic == Logic.SkipTurn) {
            SkipTurn(param);
        } else if (logic == Logic.SetScene) {
            SetScene(param);
        } else if (logic == Logic.ShowSelectScene) {
            ShowSelectSceneDialog(param);
        }
    }

    public void AttrInfluence(object param)
    {
        if (param == null) return;
        var influenceList = param as List<AttrInfluence>;
        DataInfluenceSystem.I.AddInfluence(influenceList);
    }

    public void AttrChange(object param)
    {
        if (param == null) return;
        var influenceList = param as List<AttrInfluence>;
        AttributesLogic.I.ApplyInfluence(influenceList);
    }

    public void AttrChangeHurt(object param)
    {
        if (param == null) return;
        // 检测是否有护身符, 有就使用, 没有就改变数值
        var itemId = GameUtil.ItemId(2);
        if (ConditionSystem.I.IsConditionMet(new Condition() {Formula = $"IsHaveItem({itemId})"})) {
            ItemLogic.I.ConsumeItem(itemId, 1);
        } else {
            var influenceList = param as List<AttrInfluence>;
            AttributesLogic.I.ApplyInfluenceHurt(influenceList);
        }
    }

    public void AttrChangeIncome(object param)
    {
        if (param == null) return;
        // 检测是否有护身符, 有就使用, 没有就改变数值
        var influenceList = param as List<AttrInfluence>;
        AttributesLogic.I.ApplyInfluenceIncome(influenceList);
    }

    public void AttrChangeCost(object param)
    {
        if (param == null) return;
        // 检测是否有护身符, 有就使用, 没有就改变数值
        var influenceList = param as List<AttrInfluence>;
        AttributesLogic.I.ApplyInfluenceCost(influenceList);
    }

    public void AddItem(object param)
    {
        if (param == null) return;
        (int itemID, int num) = ((int, int))param;
        ItemLogic.I.AddItem(itemID, num);
    }

    public void AddItemWithDuration(object param)
    {
        if (param == null) return;
        (int itemID, int num, DurationAndFrequency durFre) = ((int, int, DurationAndFrequency))param;
        ItemLogic.I.AddItem(itemID, num, durFre);
    }

    public void UseItem(object param)
    {
        if (param == null) return;
        (int itemID, int num) = ((int, int))param;
        ItemLogic.I.ConsumeItem(itemID, num);
    }

    public void RemoveItem(object param)
    {
        if (param == null) return;
        (int itemID, int num) = ((int, int))param;
        ItemLogic.I.RemoveItem(itemID, num);
    }
    
    public void SkipTurn(object param)
    {
        var turnNum = 1;
        if (param != null)
            turnNum = (int)param;
        TurnFLowLogic.I.SkipTurn(turnNum);
    }

    public void SetScene(object param)
    {
        if (param == null) return;
        var id = (int)param;
        GameScenesLogic.I.SetSceneById(id);
    }

    public void ShowSelectSceneDialog(object param)
    {
        CommonFlowLogic.I.ShowSelectSceneDialog();
    }
}
