using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttrInfluence 
{
    public int InstenceId;
    public DataType AttributeType;
    public Attr Attr = null;
    public string Formula;
    public bool IsSet = false;
    public DurationAndFrequency DurFre = null;
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

    // public List<AttrInfluence> GetInfluencesFromID(int [] idArr)
    // {
    //     if (idArr.Length == 0) return null;
    //     var rand = UnityEngine.Random.Range(0f,1f);
    //     var step = (1 / (float)idArr.Length);
    //     int idx = Mathf.FloorToInt(rand / step);
    //     return GetInfluencesFromID(idArr[idx]);
    // }

    // public List<AttrInfluence> GetInfluencesFromID(int id)
    // {
    //     var infProfile = ProfilesManager.Instance.GetProfileByID<ProfileInfluenceData>(id);
    //     if (infProfile == null) return null;
    //     var infList = new List<AttrInfluence>() {
    //         new AttrInfluence() {attributeType = AttributeType.HP, attr = GetRandomAttr(infProfile.Oil)},
    //         new AttrInfluence() {attributeType = AttributeType.Mood, attr = GetRandomAttr(infProfile.Hp)},
    //         new AttrInfluence() {attributeType = AttributeType.Stamina, attr = GetRandomAttr(infProfile.Water)},
    //         new AttrInfluence() {attributeType = AttributeType.Gold, attr = GetRandomAttr(infProfile.Knowledge)},
    //         new AttrInfluence() {attributeType = AttributeType.Bag, attr = GetRandomAttr(infProfile.Bag)},
    //         new AttrInfluence() {attributeType = AttributeType.Day, attr = GetRandomAttr(infProfile.Day)},
    //         new AttrInfluence() {attributeType = AttributeType.Luck, attr = GetRandomAttr(infProfile.Luck)},
    //         new AttrInfluence() {attributeType = AttributeType.Distance, attr = GetRandomAttr(infProfile.Distance)},
    //     };
    //     return infList;
    // } 

    // 获取属性影响
    public List<AttrInfluence> GetAttrInfluences(
        float Hp = 0, float Stamina = 0, float Mood = 0, float Gold = 0, 
        float Luck = 0, float Bag = 0, float Distance = 0, float Day = 0)
    {
        var infList = new List<AttrInfluence>() {
            new AttrInfluence() {AttributeType = DataType.HP, Attr = Hp},
            new AttrInfluence() {AttributeType = DataType.Stamina, Attr = Stamina},
            new AttrInfluence() {AttributeType = DataType.Mood, Attr = Mood},
            new AttrInfluence() {AttributeType = DataType.Gold, Attr = Gold},
            new AttrInfluence() {AttributeType = DataType.Luck, Attr = Luck},
            new AttrInfluence() {AttributeType = DataType.Bag, Attr = Bag},
            new AttrInfluence() {AttributeType = DataType.Distance, Attr = Distance},
            new AttrInfluence() {AttributeType = DataType.Day, Attr = Day},
        };
        return infList;
    }
    public List<AttrInfluence> GetAttrInfluences(
        string Hp = null, string Stamina = null, string Mood = null, string Gold = null, 
        string Luck = null, string Bag = null, string Distance = null, string Day = null)
    {
        var infList = new List<AttrInfluence>() {
            new AttrInfluence() {AttributeType = DataType.HP, Formula = Hp},
            new AttrInfluence() {AttributeType = DataType.Stamina, Formula = Stamina},
            new AttrInfluence() {AttributeType = DataType.Mood, Formula = Mood},
            new AttrInfluence() {AttributeType = DataType.Gold, Formula = Gold},
            new AttrInfluence() {AttributeType = DataType.Luck, Formula = Luck},
            new AttrInfluence() {AttributeType = DataType.Bag, Formula = Bag},
            new AttrInfluence() {AttributeType = DataType.Distance, Formula = Distance},
            new AttrInfluence() {AttributeType = DataType.Day, Formula = Day},
        };
        return infList;
    }

    // 通过公式获取单个影响属性
    public List<AttrInfluence> GetAttrInfluences(List<(DataType type, string formula)> formulaList)
    {
        return formulaList.Select((f) => GetAttrInfluence(f.type, f.formula).First()).ToList();
    }
    public List<AttrInfluence> GetAttrInfluence(DataType type, string formula)
    {
        return new List<AttrInfluence>() {new AttrInfluence() {
            AttributeType = type,
            Formula = formula
        }};
    }

    // 卡牌偏重
    public List<AttrInfluence> GetCardWeightInfluence(int turn, params object[] paramList)
    {
        Dictionary<int, float> weightChangeDic = new Dictionary<int, float>();
        for (int i = 0; i < paramList.Length; i+=2) {
            int cardId = Convert.ToInt32(paramList[i]);
            float weightChange = Convert.ToSingle(paramList[i+1]);
            weightChangeDic[cardId] = weightChange;
        }
        var attr = new Attr();
        attr.SetValue(weightChangeDic);
        var attrInfluence = new AttrInfluence(){
            AttributeType = DataType.CardWeight,
            Attr = attr,
            DurFre = new DurationAndFrequency() { Turn = turn }
        };
        return new List<AttrInfluence>{attrInfluence};
    }
    // 添加一个影响
    public void AddInfluence(AttrInfluence influ)
    {
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
        for (int i = influenceList.Count - 1; i <= 0; i++) {
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
        foreach (var influence in influenceList)
        {
            ApplyInfluence(baseAttr, influence);
        }
    }
    // 应用影响
    public void ApplyInfluence(Attr baseAttr, AttrInfluence influence)
    {
        ApplyChangeToAttr(baseAttr, influence);
        ApplyChangeToCardWeight(baseAttr, influence);
        ApplyChangeToQualityWeight(baseAttr, influence);
    }
    // 应用属性影响列表
    public void ApplyChangeToAttr(Attr baseAttr, AttrInfluence influence) 
    {
        var typeInt = (int)influence.AttributeType;
        if(typeInt >= (int)DataType._ValueTypeMax) return;
        float changeAmount = 0f;
        if (influence.Formula != null) {
            changeAmount = FormulaSystem.I.CalcFormula(influence.Formula, new Dictionary<string, double>() {
                { "Value", DataSystem.I.GetAttrDataByType<float>(influence.AttributeType) }
            });
        } else {
            changeAmount = influence.Attr;
        }
        if (influence.IsSet) {
            baseAttr.SetValue(changeAmount);
        } else {
            baseAttr.SetValue(baseAttr.GetValue<float>() + changeAmount);
        }
    }

    // 应用卡片偏重
    public void ApplyChangeToCardWeight(Attr baseAttr, AttrInfluence influence)
    {
        if (influence.AttributeType != DataType.CardWeight &&
            influence.AttributeType != DataType.CardLuckWeight
        ) return;
        if (influence.Attr.Type != Attr.DataType.CUSTOM) return;
        // 从属性中获取现在表
        var curWeights = baseAttr.GetValue<Dictionary<int, float>>();
        if (curWeights == null) {
            curWeights = new Dictionary<int, float>();
            baseAttr.SetValue(curWeights);
        }
        // 获取新表
        var newAttr = influence.Attr;
        var newWeights = newAttr.GetValue<Dictionary<int, float>>();
        // 应用偏重
        GameUtil.ApplyDicWeight(curWeights, newWeights, influence.IsSet);
    }

    // 应用卡片质量偏重
    public void ApplyChangeToQualityWeight(Attr baseAttr, AttrInfluence influence)
    {
        if (influence.AttributeType != DataType.CardQualityWeight) return;
        if (influence.Attr.Type != Attr.DataType.CUSTOM) return;
        // 从属性中获取现在表
        var curWeights = baseAttr.GetValue<Dictionary<CardQuality, float>>();
        if (curWeights == null) {
            curWeights = new Dictionary<CardQuality, float>();
            baseAttr.SetValue(curWeights);
        }
        // 获取新表
        var newAttr = influence.Attr;
        var newWeights = newAttr.GetValue<Dictionary<CardQuality, float>>();
        // 应用偏重
        GameUtil.ApplyDicWeight(curWeights, newWeights, influence.IsSet);
    }
}
