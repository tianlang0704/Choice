﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DIS = DataInfluenceSystem;

public class AttributesLogic : SingletonBehaviour<AttributesLogic>
{
    public List<DataType> DisplayAttrTypes = new List<DataType>() {
        DataType.HP,
        DataType.Stamina,
        DataType.Mood,
        DataType.Gold,
    };

    public Dictionary<DataType, string> AttrLabel = new Dictionary<DataType, string>() {
        {DataType.HP, "生命"},
        {DataType.Stamina, "体力"},
        {DataType.Mood, "心情"},
        {DataType.Gold, "金币"},
    };

    public List<DataType> DeadlyAttrTypes = new List<DataType>() {
        // DataType.Mood,
        DataType.HP,
        // DataType.Stamina,
        // DataType.Gold,
    };

    public Dictionary<DataType, float> DisplayAttrData {
        get { 
            return DataSystem.I.DataDic
                .Where((attrKvp) => DisplayAttrTypes.Contains((DataType)attrKvp.Key))
                .Select((attrKvp) => {
                    var value = DataSystem.I.CopyAttrDataWithInfluenceByType<float>(attrKvp.Key);
                    var kvp = new KeyValuePair<DataType, float>(attrKvp.Key, value);
                    return kvp;
                })
                .ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
        }
    }

    // public List<float> DisplayAttrDataChange {
    //     get { 
    //         return DataSystem.I.DataChange
    //             .Where((attrKvp) => DisplayAttrTypes.Contains((DataType)attrKvp.Key))
    //             .Select((attrKvp) => (float)attrKvp.Value)
    //             .ToList(); 
    //     }
    // }
    private Dictionary<int, Dictionary<float, List<int>>> moodToBuff;

    protected void Awake()
    {
        moodToBuff = new Dictionary<int, Dictionary<float, List<int>>>() {
            {0, new Dictionary<float, List<int>>() {
                {1f, new List<int>() {GameUtil.ItemId(10002), GameUtil.ItemId(10003), GameUtil.ItemId(10004)}},
            }},
            {1, new Dictionary<float, List<int>>() {
                {0.3f, new List<int>() {GameUtil.ItemId(10002), GameUtil.ItemId(10003), GameUtil.ItemId(10004)}},
                {0.7f, new List<int>() {GameUtil.ItemId(10005), GameUtil.ItemId(10006)}},
            }},
            {2, new Dictionary<float, List<int>>() {
                {0.15f, new List<int>() {GameUtil.ItemId(10002), GameUtil.ItemId(10003), GameUtil.ItemId(10004)}},
                {0.30f, new List<int>() {GameUtil.ItemId(10005), GameUtil.ItemId(10006)}},
                {0.55f, new List<int>() {GameUtil.ItemId(10007), GameUtil.ItemId(10008)}},
            }},
            {3, new Dictionary<float, List<int>>() {
                {0.15f, new List<int>() {GameUtil.ItemId(10005), GameUtil.ItemId(10006)}},
                {0.30f, new List<int>() {GameUtil.ItemId(10007), GameUtil.ItemId(10008)}},
                {0.55f, new List<int>() {GameUtil.ItemId(10009), GameUtil.ItemId(10010), GameUtil.ItemId(10011)}},
            }},
            {10, new Dictionary<float, List<int>>() {
                {1f, new List<int>() {GameUtil.ItemId(10012), GameUtil.ItemId(10013), GameUtil.ItemId(10014)}},
            }},
            {9, new Dictionary<float, List<int>>() {
                {0.3f, new List<int>() {GameUtil.ItemId(10012), GameUtil.ItemId(10013), GameUtil.ItemId(10014)}},
                {0.7f, new List<int>() {GameUtil.ItemId(10015), GameUtil.ItemId(10016)}},
            }},
            {8, new Dictionary<float, List<int>>() {
                {0.15f, new List<int>() {GameUtil.ItemId(10012), GameUtil.ItemId(10013), GameUtil.ItemId(10014)}},
                {0.30f, new List<int>() {GameUtil.ItemId(10015), GameUtil.ItemId(10016)}},
                {0.55f, new List<int>() {GameUtil.ItemId(10017), GameUtil.ItemId(10018)}},
            }},
            {7, new Dictionary<float, List<int>>() {
                {0.15f, new List<int>() {GameUtil.ItemId(10015), GameUtil.ItemId(10016)}},
                {0.30f, new List<int>() {GameUtil.ItemId(10017), GameUtil.ItemId(10018)}},
                {0.55f, new List<int>() {GameUtil.ItemId(10019), GameUtil.ItemId(10020), GameUtil.ItemId(10021)}},
            }},
        };
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
        // 初始化显示属性
        DataSystem.I.SetDataByType(DataType.HP, 5);
        DataSystem.I.SetDataByType(DataType.Stamina, 5);
        DataSystem.I.SetDataByType(DataType.Mood, 50);
        DataSystem.I.SetDataByType(DataType.Gold, 0);
        // 初始化其他属性
        DataSystem.I.SetDataByType(DataType.Bag, 5);
        DataSystem.I.SetDataByType(DataType.Luck, 5);
        DataSystem.I.SetDataByType(DataType.Distance, 0);
        DataSystem.I.SetDataByType(DataType.CurrentDay, 0);
        DataSystem.I.SetDataByType(DataType.CurrentTurn, 0);
        DataSystem.I.SetDataByType(DataType.HurtFactor, 1);
        DataSystem.I.SetDataByType(DataType.IncomeFactor, 1);
        DataSystem.I.SetDataByType(DataType.CostFactor, 1);
        DataSystem.I.SetDataByType(DataType.MedicineCount, 0);
        // 初始化属性最大值
        Dictionary<DataType, float> maxValueDic = new Dictionary<DataType, float>();
        maxValueDic[DataType.Mood] = 100;
        maxValueDic[DataType.HP] = 10;
        maxValueDic[DataType.Stamina] = 10;
        // maxValueDic[DataType.Gold] = 10;
        DataSystem.I.SetDataByType(DataType.AttrMaxTable, maxValueDic);
        // 心情变化回调
        DataSystem.I.AddCallback(DataType.Mood, () => {
            RollMoodBuff();
        });
    }
    public bool IsAttrTypeDeadly(DataType type)
    {
        if (DeadlyAttrTypes.Contains(type)) return true;
        return false;
    }
    public void ApplyInfluence(AttrInfluence influ, float factor = 1f)
    {
        if (influ == null) return;
        if ((int)influ.AttributeType < (int)DataType._AttrTypeMax) {
            var type = influ.AttributeType;
            var maxValueDic = DataSystem.I.GetDataByType<Dictionary<DataType, float>>(DataType.AttrMaxTable);
            if (maxValueDic != null && maxValueDic.ContainsKey(type)) {
                // 计算最终值
                Attr tempAttr = new Attr();
                var curValueInflu = DataSystem.I.ConvertDataToInfluence(type);
                DataInfluenceSystem.I.ApplyInfluence(tempAttr, curValueInflu);
                DataInfluenceSystem.I.ApplyInfluence(tempAttr, influ);
                // 限制值范围
                var maxValue = maxValueDic[type];
                var unclampedValue = (tempAttr - curValueInflu.Attr) * factor;
                var clampedValue = Mathf.Clamp(curValueInflu.Attr + unclampedValue, 0, maxValue);
                tempAttr.SetValue(clampedValue - curValueInflu.Attr);
                // 重新设置影响值
                influ = influ.ShallowCopy();
                influ.Formula = null;
                influ.Attr = tempAttr;
            }
        }
        DataSystem.I.ApplyInfluence(influ);
    }
    public void ApplyInfluence(List<AttrInfluence> influList, float factor = 1f)
    {
        foreach (var influence in influList) {
            ApplyInfluence(influence, factor);
        }
    }
    public void ApplyInfluence(List<AttrInfluence> influList, DataType factorType, DataType modifierType)
    {
        var factor = DataSystem.I.CopyAttrDataWithInfluenceByType<float>(factorType);
        var modifierList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<AttrInfluence>>(modifierType);
        foreach (var influence in influList) {
            var influToUse = influence;
            // 查看是否需要修改伤害值
            var validModifier = modifierList?.Where((i)=>i.AttributeType == influToUse.AttributeType || i.AttributeType == DataType.Any).ToList();
            if (validModifier != null && validModifier.Count > 0) {
                 influToUse = DataInfluenceSystem.I.ModifyAndCopyInfluence(influToUse, validModifier);
            }
            ApplyInfluence(influToUse, factor);
        }
    }
    public void ApplyInfluenceIncome(List<AttrInfluence> influList)
    {
        ApplyInfluence(influList, DataType.IncomeFactor, DataType.IncomeModifier);
    }
    public void ApplyInfluenceHurt(List<AttrInfluence> influList)
    {
        ApplyInfluence(influList, DataType.HurtFactor, DataType.HurtModifier);
    }
    public void ApplyInfluenceCost(List<AttrInfluence> influList)
    {
        ApplyInfluence(influList, DataType.CostFactor, DataType.CostModifier);
    }

    // 获取是否死亡
    public bool IsDead()
    {
        var attrTypes = System.Enum.GetValues(typeof(DataType));
        foreach (int type in attrTypes) {
            var attrData = DataSystem.Instance.GetDataByType(type);
            if (IsAttrTypeDeadly((DataType)type) && attrData <= 0) return true;
        }
        return false;
    }

    // 增减距离
    public void ChangeDistance(int add)
    {
        var distance = DataSystem.I.GetDataByType<int>(DataType.Distance);
        DataSystem.I.SetDataByType(DataType.Distance, distance + add);
        var distanceTotal = DataSystem.I.GetDataByType<int>(DataType.DistanceTotal);
        DataSystem.I.SetDataByType(DataType.DistanceTotal, distanceTotal + add);
    }

    // 增减回合
    public void ChangeTurn(int add)
    {
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        DataSystem.I.SetDataByType(DataType.CurrentTurn, curTurn + add);
    }

    // 随机心情BUFF
    public void RollMoodBuff()
    {
        // 检查心情变化
        var moodChange = 0;
        if (DataSystem.I.DataChange.ContainsKey(DataType.Mood)) {
            moodChange = DataSystem.I.DataChange[DataType.Mood];
        }
        if (moodChange == 0) return;
        // 从现在心情取出BUFF
        var curMood = DataSystem.I.GetDataByType<float>(DataType.Mood);
        var floorMood = (int)Mathf.Floor(curMood);
        if (!moodToBuff.ContainsKey(floorMood)) return; // 没有buff就不做任何事情
        var buffDic = moodToBuff[floorMood];
        // 通过Buff组几率来取一个BUFF组
        List<int> buffList = null;
        float random = UnityEngine.Random.Range(0f, 1f);
        var keys = buffDic.Keys.ToList();
        foreach (var key in keys) {
            random -= key;
            if (random < 0) {
                buffList = buffDic[key];
            }
        }
        if (buffList == null || buffList.Count <= 0) return;
        // 平均从BUFF组中随机一个BUFF出来
        var buffRandom = UnityEngine.Random.Range(0, buffList.Count);
        var buff = buffList[buffRandom];
        // 算时长
        var turnMultiplier = 3;
        var buffDuration = Mathf.Abs(moodChange) * turnMultiplier;
        var durFre = new DurationAndFrequency() { Turn = buffDuration };
        // 添加
        ItemLogic.I.AddItem(buff, 1, durFre);
    }

    // 获取属性的标签
    public string GetLabelFromAttr(DataType type)
    {
        if (!AttrLabel.Keys.Contains(type)) {
            return Enum.GetName(typeof(DataType), type);
        }
        return AttrLabel[type];
    }
    public string BuildAttrString(List<LogicExecution> leList, List<DataType> showTypes = null)
    {
        if (leList == null || leList.Count <= 0) return null;
        // 显示计算数值
        var attrInfluList = leList.SelectMany((l)=>((List<AttrInfluence>)l.Param));
        var valueDic = attrInfluList
            .GroupBy((a)=>a.AttributeType)
            .Where((g)=>showTypes != null && showTypes.Contains(g.Key))
            .Select((g)=>{
                var typeInflueList = g.Select((a)=>a).ToList();
                var resAttr = new Attr();
                DIS.I.ApplyInfluenceList(resAttr, typeInflueList);
                return new KeyValuePair<DataType,float>(g.Key, resAttr.GetValue<float>());
            })
            .ToDictionary((k)=>k.Key, (k)=>k.Value);
        // 组装显示字符串
        var format = "";
        var keyList = valueDic.Keys.ToList();
        for (int i = 0; i < keyList.Count; i++) {
            var type = keyList[i];
            format = $"{format}{GetLabelFromAttr(type)}:{{{i}:+#;-#;0}}";
            if (i < keyList.Count - 1) {
                format = format + ",";
            }
        }
        // 填入显示数值
        var valueList = valueDic.Values.Select((v)=>(object)v).ToList();
        return string.Format(format, valueList.ToArray());
    }

    public string DataTypeToUnitString(DataType dataType)
    {
        if (dataType == DataType.HP) {
            return "HP";
        } else if (dataType == DataType.Stamina) {
            return "PP";
        } else if (dataType == DataType.Gold) {
            return "$";
        } else if (dataType == DataType.Mood) {
            return "MP";
        }
        return "";
    }
}
