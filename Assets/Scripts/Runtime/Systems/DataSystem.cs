﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DataType {
    Any = -1,           // 用在一些需要匹配所有属性的地方
    HP = 0,             // 生命
    Stamina,            // 体力
    Mood,               // 心情
    Gold,               // 金币
    Luck,               // 幸运
    Bag,                // 负重
    _AttrTypeMax = 1000,// 属性类型最大值 
    Distance,           // 距离
    CurrentDay,         // 天数
    Scene,              // 场景
    Weather,            // 天气
    CurrentTurn,        // 现在回合数
    MaxTurn,            // 总回合数
    IncomeFactor,       // 收益因数
    HurtFactor,         // 伤害因数
    CostFactor,         // 费用因数
    AnswerNumOffset,    // 答案数量加减
    _ValueTypeMax = 10000, // 数值类型最大值
    AttrMaxTable,       // 数值最大表
    CardWeight,         // 卡片附加几率
    CardLuckWeight,     // 卡牌幸运权重
    CardQualityWeight,  // 卡牌质量权
    CardTypeFilter,     // 类型过滤器
    HurtModifier,       // 伤害修改
    IncomeModifier,     // 收入修改
    CostModifier,       // 费用修改
    ItemNumModifier,    // 道具数量修改
    DayCards,           // 一天卡池ID
    Items,              // 物品(包括 道具, 装备, 遗物, BUFF)
}

public class DataSystem : SingletonBehaviour<DataSystem>
{
    public delegate void Callback();
    public static float INVALID_ATTR = -99999999f;
    private Dictionary<DataType, Attr> dataDic = new Dictionary<DataType, Attr>();
    public Dictionary<DataType, Attr> DataDic {
        get {return dataDic;}
    }
    private Dictionary<DataType, Attr> dataChange = new Dictionary<DataType, Attr>();
    public Dictionary<DataType, Attr> DataChange {
        get {return dataChange;}
    }
    private Dictionary<DataType, Callback> dataChangeCallback = new Dictionary<DataType, Callback>();
    public Dictionary<DataType, Callback> DataChangeCallback {
        get {return dataChangeCallback;}
    }
    void Awake()
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

    // 初始化
    public void Init() 
    {    
        dataChangeCallback.Clear();
        // 初始化全部数据
        dataDic.Clear();
        var dataTypes = System.Enum.GetValues(typeof(DataType));
        foreach (int type in dataTypes) {
            dataDic[(DataType)type] = new Attr();
        }
        // 初始化改变数据
        dataChange.Clear();
        foreach (int type in dataTypes) {
            dataChange[(DataType)type] = new Attr();
        }
    }
    // 属性是否有值
    public bool IsAttrExist(DataType type)
    {
        if (!dataDic.ContainsKey(type)) return false;
        return true;
    }
    // 获取原属性
    public Attr GetDataByType(int type)
    {
        return GetDataByType((DataType)type);
    }
    // 获取原属性
    public Attr GetDataByType(DataType type)
    {
        if (!(IsAttrExist(type))) return null;
        return dataDic[(DataType)type];
    }
    // 获取原值
    public T GetDataByType<T>(int type)
    {
        return GetDataByType<T>((DataType)(type));
    }
    // 获取原值
    public T GetDataByType<T>(DataType type)
    {
        var attr = GetDataByType(type);
        return attr.GetValue<T>();
    }
    // 获取包含所有影响的值
    public T CopyAttrDataWithInfluenceByType<T>(int type)
    {
        return CopyAttrDataWithInfluenceByType<T>((DataType)type);
    }
    public T CopyAttrDataWithInfluenceByType<T>(DataType type)
    {
        var attr = new Attr();
        DataInfluenceSystem.I.ApplyInfluence(attr, ConvertDataToInfluence(type));
        DataInfluenceSystem.I.ApplyInfluenceByType(attr, type);
        return attr.GetValue<T>();
    }
    // 把数值转换成影响
    public AttrInfluence ConvertDataToInfluence(int type)
    {
        return ConvertDataToInfluence((DataType)type);
    }
    public AttrInfluence ConvertDataToInfluence(DataType type)
    {
        var attr = GetDataByType(type);
        return new AttrInfluence(){
            AttributeType = type,
            Attr = attr,
        };
    }
    // 设置值
    public void SetDataByType<T>(int type, T value)
    {
        SetDataByType((DataType)type, value);
    }
    // 设置值
    public void SetDataByType<T>(DataType type, T value)
    {
        dataDic[type].SetValue<T>(value);
        if (dataChangeCallback.ContainsKey(type)) {
            dataChangeCallback[type]();
        }
    }
    // 直接应用影响
    public void ApplyInfluence(AttrInfluence influ)
    {
        var type = influ.AttributeType;
        var attrChange = new Attr();
        DataInfluenceSystem.I.ApplyChangeToAttr(attrChange, influ);
        dataChange[type] += attrChange.GetValue<float>();
        var attr = GetDataByType(type);
        SetDataByType(type, attr.GetValue<float>() + attrChange.GetValue<float>());
    }
    public void ApplyInfluence(List<AttrInfluence> list)
    {
        foreach (var influence in list) {
            ApplyInfluence(influence);
        }
    }
    public void RestDataChange()
    {
        dataChange.Clear();
        var dataTypes = System.Enum.GetValues(typeof(DataType));
        foreach (int type in dataTypes) {
            dataChange[(DataType)type] = new Attr();
        }
    }
    // 设置回调
    public void AddCallback(DataType type, Callback cb)
    {
        if (!dataChangeCallback.ContainsKey(type)) {
            dataChangeCallback[type] = cb;
        } else {
            dataChangeCallback[type] += cb;
        }
    }
}
