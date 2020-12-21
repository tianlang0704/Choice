using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConditionType {
    Attr = 0,
    CardExist,
    ItemExist,
}

public class Condition {
    public ConditionType ConditionType;
    public string AttrFormula;
    public int IdValue;
}

public enum ConditionOperator {
    AND = 0,
    OR,
}
public class ConditionGroup {
    public List<Condition> conditionList;
    public ConditionOperator op;
}

public class ConditionSystem : SingletonBehaviour<ConditionSystem>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsConditionGroupMet(ConditionGroup cg)
    {
        if (cg.op == ConditionOperator.AND) {
            return IsAllConditionsMet(cg.conditionList);
        }else if (cg.op == ConditionOperator.OR) {
            return IsOneConditionMet(cg.conditionList);
        }
        return false;
    }

    public bool IsOneConditionMet(List<Condition> conList)
    {
        foreach (var item in conList)
        {
            if (IsConditionMet(item)) return true;
        }
        return false;
    }

    public bool IsAllConditionsMet(List<Condition> conList)
    {
        foreach (var item in conList) {
            if (!IsConditionMet(item)) return false;
        }
        return true;
    }

    public bool IsConditionMet(Condition condition)
    {
        bool res = false;
        if (condition.ConditionType == ConditionType.Attr){
            res = IsAttrConditonMet(condition);
        }
        return res;
    }

    public bool IsAttrConditonMet(Condition condition)
    {
        FormulaSystem.I.UpdateVariable();
        return FormulaSystem.I.CalcFormula(condition.AttrFormula) == 1f;
    }

    public ConditionGroup GetAttrConditionGroup(string checkFormula)
    {
        return new ConditionGroup() {
            conditionList = GetAttrConditionList(checkFormula),
            op = ConditionOperator.OR
        };
    }

    public List<Condition> GetAttrConditionList(string checkFormula)
    {
        return new List<Condition>() {
            new Condition() {
                ConditionType = ConditionType.Attr,
                AttrFormula = checkFormula,
            }
        };
    }
}
