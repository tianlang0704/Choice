using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Condition {
    public string Formula;
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

    public bool IsConditionMet(Condition condition)
    {
        if (condition == null) return true;
        FormulaSystem.I.UpdateVariable();
        return FormulaSystem.I.CalcFormula(condition.Formula) == 1f;
    }

    public Condition GetCondition(string formula)
    {
        return new Condition(){Formula = formula};
    }
}
