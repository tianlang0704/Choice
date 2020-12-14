using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Logic {
    AttrChange = 0,
    AttrInfluence,
}

public class LogicExecution {
    public Logic logic;
    public List<object> paramList;
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
        for (int i = 0; i < paramList.Length; i+=2)
        {
            Logic logic = (Logic)paramList[i];
            List<object> param = new List<object>() {paramList[i+1]};
            list.Add(new LogicExecution(){logic = logic, paramList = param});
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
        ExecuteCommonLogic(le.logic, le.paramList.ToArray());
    }

    public void ExecuteCommonLogic(Logic logic, params object[] paramList)
    {
        if (logic == Logic.AttrChange) {
            AttrChange(paramList);
        } else if (logic == Logic.AttrInfluence) {
            AttrInfluence(paramList);
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
}
