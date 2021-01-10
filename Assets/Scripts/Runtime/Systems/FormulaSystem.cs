using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jace;

public class FormulaSystem : SingletonBehaviour<FormulaSystem>
{
    CalculationEngine calcEngine = new CalculationEngine(new JaceOptions() { 
        ExecutionMode = Jace.Execution.ExecutionMode.Interpreted,
        CacheEnabled = false,
        OptimizerEnabled = false,
    });
    Dictionary<string, double> variables = new Dictionary<string, double>();
    Dictionary<object, Action<CalculationEngine, Dictionary<string, double>>> updateVariableCallback = new Dictionary<object, Action<CalculationEngine, Dictionary<string, double>>>();

    protected void Awake()
    {
        calcEngine.AddFunction("IsHaveItem", IsHaveItem);
    }
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
        // 添加公式数据
        SetUpdateCallback(this, (e, v) => {
            // 基础属性
            foreach (int type in Enum.GetValues(typeof(DataType)))
            {
                if (type >= (int)DataType._ValueTypeMax) break;
                var name = Enum.GetName(typeof(DataType), type);
                var value = DataSystem.I.GetAttrDataByType<float>(type);
                v[name] = value;
                var nameWithInfluence = name + "_i";
                var valueWithInfluence = DataSystem.I.CopyAttrDataWithInfluenceByType<float>(type);
                v[nameWithInfluence] = valueWithInfluence;
            }
        });
    }
    // 添加更新变量回调
    public void SetUpdateCallback(object o, Action<CalculationEngine, Dictionary<string, double>> cb, bool updateNow = false)
    {
        updateVariableCallback[o] = cb;
        if (updateNow) {
            UpdateVariable();
        }
    }
    // 更新变量
    public void UpdateVariable()
    {
        variables.Clear();
        foreach (var kvp in updateVariableCallback)
        {
            kvp.Value(calcEngine, variables);
        }
    }

    public float CalcFormula(string formula)
    {
        return Convert.ToSingle(calcEngine.Calculate(formula, variables));
    }
    
    private double IsHaveItem(double id)
    {
        if (ItemLogic.I.IsHaveItem(Convert.ToInt32(id))) {
            return 1;
        } else {
            return 0;
        }
    }
}
