using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttrInfluence 
{
    public string Identifier;
    public int Priority = 0;
    public DataType AttributeType;
    public Attr Attr = null;
    public string Formula;
    public bool IsSet = false;
    public DurationAndFrequency DurFre = null;
    public Condition Condition = null;
    public AttrInfluence ShallowCopy()
    {
        var influCopy = (AttrInfluence)this.MemberwiseClone();
        if (Attr != null) {
            influCopy.Attr = Attr.ShallowCopy();
        }
        return influCopy;
    }
}

public class DataInfluenceSystem : SingletonBehaviour<DataInfluenceSystem>
{
    Dictionary<DataType, List<AttrInfluence>> influDic = new Dictionary<DataType, List<AttrInfluence>>();
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
        influDic.Clear();
        // var attr = new Attr();
        // attr.SetValue(CardQuality.Blue);
        // AddInfluence(new AttrInfluence() {
        //     AttributeType = DataType.TurnCardQuality,
        //     Attr = attr,
        //     IsSet = true,
        // });
    }

    float GetRandomAttr(int [] randArr)
    {
        return GetRandomAttr(randArr.Select((e)=>(float)e).ToList());    
    }

    float GetRandomAttr(float [] randArr)
    {
        return GetRandomAttr(randArr.ToList());
    }

    float GetRandomAttr(List<float> randList)
    {
        float min = randList.Count > 0 ? randList[0] : 0f;
        float max = randList.Count > 1 ? randList[1] : min;
        return UnityEngine.Random.Range(min, max);
    }
    // 获取属性影响
    public List<AttrInfluence> GetAttrInfluenceList(
        float Hp = 0, float Stamina = 0, float Mood = 0, float Gold = 0, 
        float Luck = 0, float Bag = 0, float Distance = 0, float Day = 0)
    {
        var infList = new List<AttrInfluence>() {
            GetAttrInfluence(DataType.HP, Hp),
            GetAttrInfluence(DataType.Stamina, Stamina),
            GetAttrInfluence(DataType.Mood, Mood),
            GetAttrInfluence(DataType.Gold, Gold),
            GetAttrInfluence(DataType.Luck, Luck),
            GetAttrInfluence(DataType.Bag, Bag),
            GetAttrInfluence(DataType.Distance, Distance),
            GetAttrInfluence(DataType.CurrentDay, Day),
        };
        return infList;
    }
    public List<AttrInfluence> GetAttrInfluenceList(
        string Hp = null, string Stamina = null, string Mood = null, string Gold = null, 
        string Luck = null, string Bag = null, string Distance = null, string Day = null)
    {
        var infList = new List<AttrInfluence>() {
            GetAttrInfluence(DataType.HP, Hp),
            GetAttrInfluence(DataType.Stamina, Stamina),
            GetAttrInfluence(DataType.Mood, Mood),
            GetAttrInfluence(DataType.Gold, Gold),
            GetAttrInfluence(DataType.Luck, Luck),
            GetAttrInfluence(DataType.Bag, Bag),
            GetAttrInfluence(DataType.Distance, Distance),
            GetAttrInfluence(DataType.CurrentDay, Day),
        };
        return infList;
    }
    public List<AttrInfluence> GetAttrInfluenceList(List<(DataType type, string formula)> formulaList, int turn = 0,bool isSet = false, int priority = 0, Condition condition = null)
    {
        return formulaList.Select((f) => GetAttrInfluence(f.type, f.formula, turn, isSet, priority, condition)).ToList();
    }
    public List<AttrInfluence> GetAttrInfluenceList(List<(DataType type, float value)> attrList, int turn = 0,bool isSet = false, int priority = 0, Condition condition = null)
    {
        return attrList.Select((f) => GetAttrInfluence(f.type, f.value, turn, isSet, priority, condition)).ToList();
    }
    public List<AttrInfluence> GetAttrInfluenceList<T>(List<(DataType type, T value)> attrList, int turn = 0,bool isSet = false, int priority = 0, Condition condition = null)
    {
        return attrList.Select((f) => GetAttrInfluence(f.type, f.value, turn, isSet, priority, condition)).ToList();
    }
    public List<AttrInfluence> GetAttrInfluenceList(DataType type, string formula, int turn = 0,bool isSet = false, int priority = 0, Condition condition = null)
    {
        return new List<AttrInfluence>() {GetAttrInfluence(type, formula, turn, isSet, priority, condition)};
    }
    // 取单个影响属性
    public AttrInfluence GetAttrInfluence(DataType type, string formula, int turn = 0, bool isSet = false, int priority = 0, Condition condition = null)
    {
        return new AttrInfluence() {
            IsSet = isSet,
            Priority = priority,
            AttributeType = type,
            Formula = formula,
            DurFre = new DurationAndFrequency() { Turn = turn },
            Condition = condition,
        };
    }
    public AttrInfluence GetAttrInfluence(DataType type, float value, int turn = 0, bool isSet = false, int priority = 0, Condition condition = null)
    {
        return new AttrInfluence() {
            IsSet = isSet,
            Priority = priority,
            AttributeType = type, 
            Attr = value,
            DurFre = new DurationAndFrequency() { Turn = turn },
            Condition = condition,
        };
    }
    public AttrInfluence GetAttrInfluence<T>(DataType type, T value, int turn = 0, bool isSet = false, int priority = 0, Condition condition = null)
    {
        var attr = new Attr();
        attr.SetValue<T>(value);
        return new AttrInfluence() {
            IsSet = isSet,
            Priority = priority,
            AttributeType = type, 
            Attr = attr,
            DurFre = new DurationAndFrequency() { Turn = turn },
            Condition = condition,
        };
    }
    public AttrInfluence GetAttrInfluence(string formula)
    {
        return new AttrInfluence() {
            Formula = formula,
        };
    }
    // 卡牌偏重
    public AttrInfluence GetCardWeightInfluence(
        List<(int id, float w)> paramList,
        int turn = 0, 
        int priority = 0
    ) {
        Dictionary<int, float> weightChangeDic = paramList.ToDictionary((p)=>p.id, (p)=>p.w);
        var attr = new Attr();
        attr.SetValue(weightChangeDic);
        var attrInfluence = new AttrInfluence(){
            Priority = priority,
            AttributeType = DataType.TurnCardWeight,
            Attr = attr,
            DurFre = new DurationAndFrequency() { Turn = turn },
        };
        return attrInfluence;
    }
    // 质量偏重
    public AttrInfluence GetQualityWeightInfluence(
        List<(CardQuality q, float w)> paramList,
        int turn = 0, 
        bool isSet = false,
        int priority = 0
    ) {
        Dictionary<CardQuality, float> weightChangeDic = paramList.ToDictionary((p)=>p.q, (p)=>p.w);
        var attr = new Attr();
        attr.SetValue(weightChangeDic);
        var attrInfluence = new AttrInfluence(){
            Priority = priority,
            AttributeType = DataType.TurnCardQualityWeight,
            Attr = attr,
            IsSet = isSet,
            DurFre = new DurationAndFrequency() { Turn = turn },
        };
        return attrInfluence;
    }
    // 幸运偏重
    public AttrInfluence GetLuckWeightInfluence(
        List<(LuckQualityGroup i, float w)> paramList,
        int turn = 0, 
        bool isSet = false,
        int priority = 0

    ) {
        Dictionary<LuckQualityGroup, float> weightChangeDic = paramList.ToDictionary((p)=>p.i, (p)=>p.w);
        var attr = new Attr();
        attr.SetValue(weightChangeDic);
        var attrInfluence = new AttrInfluence(){
            Priority = priority,
            AttributeType = DataType.TurnCardLuckWeight,
            Attr = attr,
            IsSet = isSet,
            DurFre = new DurationAndFrequency() { Turn = turn },
        };
        return attrInfluence;
    }
    // 幸运偏重
    public AttrInfluence GetCardTypeInfluence(
        List<CardType> paramList,
        int turn = 0, 
        bool isSet = false,
        int priority = 0

    ) {
        var attr = new Attr();
        attr.SetValue(paramList);
        var attrInfluence = new AttrInfluence(){
            Priority = priority,
            AttributeType = DataType.TurnCardTypeFilter,
            Attr = attr,
            IsSet = isSet,
            DurFre = new DurationAndFrequency() { Turn = turn },
        };
        return attrInfluence;
    }
    // 添加一个影响
    public void AddInfluence(Func<AttrInfluence> influFunc)
    {
        if (influFunc == null) return;
        var influ = influFunc();
        AddInfluence(influ);
    }
    public void AddInfluence(AttrInfluence influ)
    {
        if (influ == null) return;
        if (!influDic.ContainsKey(influ.AttributeType)) {
            influDic[influ.AttributeType] = new List<AttrInfluence>();
        }
        influDic[influ.AttributeType].Add(influ);
        if (influ.DurFre != null) {
            DurFreSystem.I.AddDurFreControl(influ.DurFre, () => {
                DataInfluenceSystem.I.RemoveInfluence(influ);
            });
        }
    }
    public void AddInfluence(Func<List<AttrInfluence>> influenceListFunc)
    {
        if (influenceListFunc == null) return;
        var influList = influenceListFunc();
        AddInfluence(influList);
    }
    public void AddInfluence(List<AttrInfluence> influenceList)
    {
        if (influenceList == null) return;
        influenceList.ForEach((influ) => {
            AddInfluence(influ);
        });
    }
    // 添加或者改变第一个影响
    public void AddOrChangeInfluence(AttrInfluence influ)
    {
        var existingQualityWeightList = DataInfluenceSystem.I.GetExistingInfluenceListForType(influ.AttributeType);
        if (existingQualityWeightList != null && existingQualityWeightList.Count > 0) {
            var existingQualityWeight = existingQualityWeightList[0];
            ApplyInfluence(existingQualityWeight.Attr, influ);
        } else {
            AddInfluence(influ);
        }
    }
    public void AddOrChangeInfluence(List<AttrInfluence> influenceList)
    {
        if (influenceList == null) return;
        influenceList.ForEach((influ) => {
            AddOrChangeInfluence(influ);
        });
    }
    // 移除一个影响
    public void RemoveInfluence(AttrInfluence influ)
    {
        if (influDic.ContainsKey(influ.AttributeType)) {
            influDic[influ.AttributeType].Remove(influ);
        }
        if (influ.DurFre != null) {
            DurFreSystem.I.RemoveDurFreControl(influ.DurFre);
        }
    }
    public void RemoveInfluence(List<AttrInfluence> influenceList)
    {
        if (influenceList == null || influenceList.Count <= 0) return;
        for (int i = influenceList.Count - 1; i >= 0; i--) {
            var influ = influenceList[i];
            RemoveInfluence(influ);
        }
    }
    public void RemoveInfluence(DataType type)
    {
        if (!influDic.ContainsKey(type)) return;
        var infList = influDic[type];
        RemoveInfluence(infList);
    }

    public void RemoveInfluence(string identifer)
    {
        List<AttrInfluence> influToRemoveList = new List<AttrInfluence>();
        foreach (var kvp in influDic) {
            foreach (var influ in kvp.Value) {
                if (influ.Identifier == identifer) {
                    influToRemoveList.Add(influ);
                }
            }
        }
        influToRemoveList.ForEach((i)=>RemoveInfluence(i));
    }

    public List<AttrInfluence> GetExistingInfluenceListForType(DataType type)
    {
        if (!influDic.ContainsKey(type)) return null;
        return influDic[type];
    }
    // 应用系统里某个类型的影响
    public void ApplyInfluenceByType(Attr baseAttr, DataType type)
    {
        if (!influDic.ContainsKey(type)) return;
        var list = influDic[type];
        ApplyInfluenceList(baseAttr, list);
    }
    // 应用影响列表
    public void ApplyInfluenceList(Attr baseAttr, List<AttrInfluence> influenceList)
    {
        if (influenceList == null) return;
        // influenceList = influenceList.OrderBy((i)=>i.Priority).ToList();
        influenceList.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        foreach (var influence in influenceList) {
            ApplyInfluence(baseAttr, influence);
        }
    }
    public AttrInfluence ModifyAndCopyInfluence(AttrInfluence baseInflu, List<AttrInfluence> newInfluList)
    {
        if (newInfluList == null || newInfluList.Count <= 0) return null;
        var newAttr = new Attr();
        ApplyInfluence(newAttr, baseInflu);
        foreach (var newInflu in newInfluList) {
            ApplyInfluence(newAttr, newInflu);
        }
        var resInflu = baseInflu.ShallowCopy();
        resInflu.Formula = null;
        resInflu.Attr = newAttr;
        return resInflu;
    }
    public AttrInfluence ConvertFormulaToAttrCopy(AttrInfluence influ)
    {
        if (influ.Formula == null) return influ;
        var influCopy = influ.ShallowCopy();
        return ConvertFormulaToAttr(influCopy);
    }
    public AttrInfluence ConvertFormulaToAttr(AttrInfluence influ)
    {
        if (influ.Formula == null) return influ;
        var value = FormulaSystem.I.CalcFormula(influ.Formula, GetAdditionalParams(influ));
        if (influ.Attr == null) {
            influ.Attr = new Attr();
        }
        influ.Attr.SetValue<float>(value);
        influ.Formula = null;
        return influ;
    }
    Dictionary<string, double> GetAdditionalParams(AttrInfluence influence, Attr baseAttr = null)
    {
        var p = new Dictionary<string, double>() {
            { "Value", DataSystem.I.GetDataByType<float>(influence.AttributeType) }
        };
        if (baseAttr != null) {
            p["Target"] =  baseAttr.GetValue<float>();
        }
        return p;
    }
    // 应用影响
    public void ApplyInfluence(Attr baseAttr, AttrInfluence influence)
    {
        if (influence.Condition != null && !ConditionSystem.I.IsConditionMet(influence.Condition, false, GetAdditionalParams(influence, baseAttr))) {
            return;
        }
        if((int)influence.AttributeType < (int)DataType._ValueTypeMax) {
            ApplyChangeToAttr(baseAttr, influence);
        } else if (influence.AttributeType == DataType.AttrMaxTable) {
            GameUtil.ApplyFloatDicAttr<DataType>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardWeight) {
            GameUtil.ApplyFloatDicAttr<int>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardLuckWeight) {
            GameUtil.ApplyFloatDicAttr<LuckQualityGroup>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardQualityList) {
            GameUtil.ApplyListAttr<CardQuality>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardQualityWeight) { 
            GameUtil.ApplyFloatDicAttr<CardQuality>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnAnswerLogicWeight) {
            GameUtil.ApplyListDicAttr<CardQuality, List<LogicExecution>>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnAnswerLogicList) {
            GameUtil.ApplyListAttr<List<LogicExecution>>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardTypeFilter) {
            GameUtil.ApplyListAttr<CardType>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.TurnCardValidIdList) {
            GameUtil.ApplyListAttr<int>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.HurtModifier || influence.AttributeType == DataType.IncomeModifier) {
            GameUtil.ApplyListAttr<AttrInfluence>(baseAttr, influence);
        } else if (influence.AttributeType == DataType.ItemNumModifier) {
            GameUtil.ApplyFloatDicAttr<int>(baseAttr, influence);
        }
    }
    // 应用属性影响列表
    public void ApplyChangeToAttr(Attr baseAttr, AttrInfluence influence) 
    {
        float changeAmount = 0f;
        if (influence.Formula != null) {
            changeAmount = FormulaSystem.I.CalcFormula(influence.Formula, GetAdditionalParams(influence, baseAttr));
        } else {
            changeAmount = influence.Attr;
        }
        if (influence.IsSet) {
            baseAttr.SetValue(changeAmount);
        } else {
            baseAttr.SetValue(baseAttr.GetValue<float>() + changeAmount);
        }
    }
}
