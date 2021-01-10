using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Logic {
    AttrChange = 0,     // 永久改变数值
    AttrChangeShield,   // 永久改变数值, 但是先检查护身符
    AttrInfluence,      // 暂时影响数值
    AddItem,            // 添加道具
    UseItem,            // 使用道具
    SkipTurn,           // 跳过回合
    SetScene,           // 设置场景
    ShowSelectScene,    // 显示选择场景
}

public class LogicExecution {
    public Logic logic;
    public object param;
    public Condition condition;
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

    public List<LogicExecution> GetLogicList(List<(Logic l, object p, Condition c)> exeList)
    {
        var leList = exeList.Select((o) => {
            return new LogicExecution() { logic = o.l, param = o.p, condition = o.c};
        }).ToList();
        return leList;
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
        if (le.condition != null) {
            if (!ConditionSystem.I.IsConditionMet(le.condition)) return;
        }
        ExecuteCommonLogic(le.logic, le.param);
    }

    public void ExecuteCommonLogic(Logic logic, object param)
    {
        if (logic == Logic.AttrChange) {
            AttrChange(param);
        } else if (logic == Logic.AttrChangeShield) {
            AttrChangeShield(param);
        } else if (logic == Logic.AttrInfluence) {
            AttrInfluence(param);
        } else if (logic == Logic.AddItem) {
            AddItem(param);
        } else if (logic == Logic.UseItem) {
            UseItem(param);
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
        DataSystem.I.ApplyInfluenceList(influenceList);
    }

    public void AttrChangeShield(object param)
    {
        if (param == null) return;
        // 检测是否有护身符, 有就使用, 没有就改变数值
        if (ConditionSystem.I.IsConditionMet(new Condition() {Formula = $"IsHaveItem(2)"})) {
            ItemLogic.I.ConsumeItem(2, 1);
        } else {
            var influenceList = param as List<AttrInfluence>;
            AttrChange(influenceList);
        }
    }

    public void AddItem(object param)
    {
        if (param == null) return;
        (int itemID, int num, float pos) = ((int, int, float))param;
        var rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < pos) {
            ItemLogic.I.AddItem(itemID, num);   
        }
    }

    public void UseItem(object param)
    {
        if (param == null) return;
        (int itemID, int num, float pos) = ((int, int, float))param;
        if (pos >= 1 || (pos < 1 && UnityEngine.Random.Range(0, 1) < pos)) {
            ItemLogic.I.ConsumeItem(itemID, num);   
        }
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
