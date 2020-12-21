using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jace;

public class FormulaSystem : SingletonBehaviour<FormulaSystem>
{
    CalculationEngine calcEngine = new CalculationEngine();
    Dictionary<string, double> variables = new Dictionary<string, double>();

    protected void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // 更新变量
    public void UpdateVariable()
    {
        variables.Clear();
        foreach (int type in Enum.GetValues(typeof(AttributeType)))
        {
            if (type >= (int)AttributeType._ValueTypeMax) break;
            var name = Enum.GetName(typeof(AttributeType), type);
            var value = DataSystem.I.GetAttrDataByType<float>(type);
            variables[name] = value;
        }
    }

    public float CalcFormula(string formula)
    {
        return Convert.ToSingle(calcEngine.Calculate(formula, variables));
    }
}
