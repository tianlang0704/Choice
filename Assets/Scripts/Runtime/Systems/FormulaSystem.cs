using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jace;
using Random = UnityEngine.Random;

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
        calcEngine.AddFunction("GetTurnCardType", GetTurnCardType);
        calcEngine.AddFunction("RandomInt", RandomInt);
        calcEngine.AddFunction("RandomFloat", RandomFloat);
        calcEngine.AddFunction("RandomGoodsId", RandomGoodsId);
        calcEngine.AddFunction("RandomRelicsId", RandomRelicsId);
        calcEngine.AddFunction("RandomEquipsId", RandomEquipsId);   
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
                var value = DataSystem.I.GetDataByType<float>(type);
                v[name] = value;
            }
            // 影响属性
            foreach (int type in Enum.GetValues(typeof(DataType)))
            {
                if (type >= (int)DataType._ValueTypeMax) break;
                var nameWithInfluence = name + "_i";
                var valueWithInfluence = DataSystem.I.CopyAttrDataWithInfluenceByType<float>(type);
                v[nameWithInfluence] = valueWithInfluence;
            }            
        });
        UpdateVariable();
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

    public float CalcFormula(string formula, Dictionary<string, double> moreVariables = null)
    {
        if (moreVariables == null) {
            moreVariables = variables;
        } else {
            moreVariables = moreVariables
                .Concat(
                    variables.Where((kvp) => !moreVariables.ContainsKey(kvp.Key)))
                .ToDictionary((kvp)=>kvp.Key, (kvp)=>kvp.Value);
        }
        return Convert.ToSingle(calcEngine.Calculate(formula, moreVariables));
    }
    
    private double IsHaveItem(double id)
    {
        if (ItemLogic.I.IsHaveItem(Convert.ToInt32(id))) {
            return 1;
        } else {
            return 0;
        }
    }

    private double GetTurnCardType()
    {
        var turnCard = CardPoolLogic.I.GetTurnCardRaw();
        if (turnCard == null) return -1;
        return (double)turnCard.Type;
    }
    private double RandomInt(double min, double max)
    {
        return (double)Random.Range((int)min, (int)max);
    }
    private double RandomFloat(double min, double max)
    {
        return (double)Random.Range((float)min, (float)max);
    }
    private double RandomGoodsId()
    {
        var allList = ItemLogic.I.GetAllItemListByType(new List<ItemType>() {ItemType.Goods});
        if (allList == null || allList.Count <= 0) return 0;
        var randomIdx = Random.Range(0, allList.Count);
        return (double)allList[randomIdx].Id;
    }
    private double RandomRelicsId()
    {
        var allList = ItemLogic.I.GetAllItemListByType(new List<ItemType>() {ItemType.Relics});
        if (allList == null || allList.Count <= 0) return 0;
        var randomIdx = Random.Range(0, allList.Count);
        return (double)allList[randomIdx].Id;
    }

    private double RandomEquipsId()
    {
        var allList = ItemLogic.I.GetAllItemListByType(new List<ItemType>() {ItemType.Equips});
        if (allList == null || allList.Count <= 0) return 0;
        var randomIdx = Random.Range(0, allList.Count);
        return (double)allList[randomIdx].Id;
    }
}
