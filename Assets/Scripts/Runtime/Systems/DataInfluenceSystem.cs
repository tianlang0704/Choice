using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttrInfluence 
{
    public int instenceId;
    public DataType attributeType;
    public Attr attr;
    public bool isAdd = true;
    public DurationAndFrequency durFre = null;
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

    public List<AttrInfluence> GetAttrInfluences(
        float Hp = 0, float Stamina = 0, float Mood = 0, float Gold = 0, 
        float Luck = 0, float Bag = 0, float Distance = 0, float Day = 0)
    {
        var infList = new List<AttrInfluence>() {
            new AttrInfluence() {attributeType = DataType.HP, attr = Hp},
            new AttrInfluence() {attributeType = DataType.Stamina, attr = Stamina},
            new AttrInfluence() {attributeType = DataType.Mood, attr = Mood},
            new AttrInfluence() {attributeType = DataType.Gold, attr = Gold},
            new AttrInfluence() {attributeType = DataType.Luck, attr = Luck},
            new AttrInfluence() {attributeType = DataType.Bag, attr = Bag},
            new AttrInfluence() {attributeType = DataType.Distance, attr = Distance},
            new AttrInfluence() {attributeType = DataType.Day, attr = Day},
        };
        return infList;
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
        attr.SetCustomValue(weightChangeDic);
        var attrInfluence = new AttrInfluence(){
            attributeType = DataType.CardWeight,
            attr = attr,
            durFre = new DurationAndFrequency() { turn = turn }
        };
        return new List<AttrInfluence>{attrInfluence};
    }
    // 添加一个影响
    public void AddInfluence(List<AttrInfluence> influenceList)
    {
        if (influenceList == null) return;
        influenceList.ForEach((influ) => {
            if (!influDic.ContainsKey(influ.attributeType)) {
                influDic[influ.attributeType] = new List<AttrInfluence>();
            }
            influDic[influ.attributeType].Add(influ);
            if (influ.durFre != null) {
                DurFreSystem.I.AddInfluenceDurFreControl(influ);
            }
        });
    }
    // 移除一个影响
    public void RemoveInfluence(List<AttrInfluence> influenceList)
    {
        if (influenceList == null) return;
        influenceList.ForEach((influ) => {
            if (influDic.ContainsKey(influ.attributeType)) {
                influDic[influ.attributeType].Remove(influ);
            }
            if (influ.durFre != null) {
                DurFreSystem.I.RemoveInfluenceDurFreControl(influ);
            }
        });
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
    }
    // 应用属性影响列表
    public void ApplyChangeToAttr(Attr baseAttr, AttrInfluence influence) 
    {
        var typeInt = (int)influence.attributeType;
        if(typeInt >= (int)DataType._ValueTypeMax) return;
        if (influence.attr.Type == Attr.DataType.FLOAT || influence.attr.Type == Attr.DataType.INT) {
            var changeAmount = influence.attr;
            if (!influence.isAdd) {
                changeAmount *= -1;
            }
            baseAttr.SetValue(baseAttr + changeAmount);
        } else if(influence.attr.Type == Attr.DataType.CUSTOM){
            print("不能改变CUSTOM类型属性, 其实这里也不应该出现CUSTOM类型属性");
        }
    }

    // 应用卡片偏重
    public void ApplyChangeToCardWeight(Attr baseAttr, AttrInfluence influence)
    {
        if (influence.attributeType != DataType.CardWeight) return;
        if (influence.attr.Type != Attr.DataType.CUSTOM) return;
        // 从属性中获取现在表
        Dictionary<int, float> curWeights = baseAttr.GetCustomValue<Dictionary<int, float>>();
        if (curWeights == null) {
            curWeights = new Dictionary<int, float>();
            baseAttr.SetCustomValue<Dictionary<int, float>>(curWeights);
        }
        // 获取新表
        var newAttr = influence.attr;
        var newWeights = newAttr.GetCustomValue<Dictionary<int, float>>();
        // 循环加值
        foreach (var newKvp in newWeights)
        {
            // 找旧值
            float value;
            if (curWeights.ContainsKey(newKvp.Key)){
                value = curWeights[newKvp.Key];
            } else {
                value = 0f;
            }
            // 改变值
            if (influence.isAdd) {
                value += newKvp.Value;
            } else {
                value -= newKvp.Value;
            }
            // 设置回表
            curWeights[newKvp.Key] = value;
        }
    }
}
