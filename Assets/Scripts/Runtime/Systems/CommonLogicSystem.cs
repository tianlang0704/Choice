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
    public List<object> paramList;
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

    public List<LogicExecution> GetLogicList(params object[] paramList)
    {
        var list = new List<LogicExecution>();
        for (int i = 0; i < paramList.Length; i+=3)
        {
            Logic logic = (Logic)paramList[i];
            var p = i+1 >= paramList.Count() ? null : paramList[i+1];
            List<object> param = new List<object>();
            if (p != null) param.Add(p);
            var c = i+2 >= paramList.Count() ? null : paramList[i+2];
            Condition condition = c as Condition;
            list.Add(new LogicExecution(){
                logic = logic, 
                paramList = param,
                condition = condition
            });
        }
        return list;
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
        ExecuteCommonLogic(le.logic, le.paramList.ToArray());
    }

    public void ExecuteCommonLogic(Logic logic, params object[] paramList)
    {
        if (logic == Logic.AttrChange) {
            AttrChange(paramList);
        } else if (logic == Logic.AttrChangeShield) {
            AttrChangeShield(paramList);
        } else if (logic == Logic.AttrInfluence) {
            AttrInfluence(paramList);
        } else if (logic == Logic.AddItem) {
            AddItem(paramList);
        } else if (logic == Logic.UseItem) {
            UseItem(paramList);
        } else if (logic == Logic.SkipTurn) {
            SkipTurn(paramList);
        } else if (logic == Logic.SetScene) {
            SetScene(paramList);
        } else if (logic == Logic.ShowSelectScene) {
            ShowSelectSceneDialog(paramList);
        }
    }

    public void AttrInfluence(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        var influenceList = paramList[0] as List<AttrInfluence>;
        DataInfluenceSystem.I.AddInfluence(influenceList);
    }

    public void AttrChange(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        var influenceList = paramList[0] as List<AttrInfluence>;
        DataSystem.I.ApplyInfluenceList(influenceList);
    }

    public void AttrChangeShield(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        // 检测是否有护身符, 有就使用, 没有就改变数值
        if (ConditionSystem.I.IsConditionMet(new Condition() {Formula = $"IsHaveItem(2)"})) {
            ItemLogic.I.ConsumeItem(2, 1);
        } else {
            AttrChange(paramList);
        }
    }

    public void AddItem(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        var itemID = (int)paramList[0];
        int num = (int)(paramList[1] ?? 1);
        ItemLogic.I.AddItem(itemID, num);
    }

    public void UseItem(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        var itemID = (int)paramList[0];
        int num = (int)(paramList[1] ?? 1);
        ItemLogic.I.ConsumeItem(itemID, num);
    }
    
    public void SkipTurn(params object[] paramList)
    {
        var turnNum = 1;
        if (paramList.Length > 0)
            turnNum = (int)paramList[0];
        TurnFLowLogic.I.SkipTurn(turnNum);
    }

    public void SetScene(params object[] paramList)
    {
        if (paramList.Length <= 0) return;
        var id = (int)paramList[0];
        GameScenesLogic.I.SetSceneById(id);
    }

    public void ShowSelectSceneDialog(params object[] paramList)
    {
        CommonFlowLogic.I.ShowSelectSceneDialog();
    }
}
