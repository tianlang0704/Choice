using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DataType {
    HP = 0,             // 生命
    Stamina,            // 体力
    Mood,               // 心情
    Gold,               // 金币
    Luck,               // 幸运
    Bag,                // 负重
    Distance,           // 距离
    Day,                // 天数
    Scene,              // 场景
    Weather,            // 天气
    CurrentTurn,        // 现在回合数
    MaxTurn,            // 总回合数
    IncomeFactor,       // 收益因数
    HurtFactor,         // 伤害因数
    _ValueTypeMax = 999, // 数值类型属性
    CardWeight,         // 卡片附加几率
    DayCards,           // 一天卡池ID
    Items,              // 物品(包括 道具, 装备, 遗物)
}

public class DataSystem : SingletonBehaviour<DataSystem>
{
    public static float INVALID_ATTR = -99999999f;
    private Dictionary<DataType, Attr> dataDic = new Dictionary<DataType, Attr>();
    public Dictionary<DataType, Attr> DataDic {
        get {return dataDic;}
    }
    private Dictionary<DataType, Attr> dataChange = new Dictionary<DataType, Attr>();
    public Dictionary<DataType, Attr> DataChange {
        get {return dataChange;}
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
        dataDic.Clear();
        dataChange.Clear();
        // 初始化全部数据
        var dataTypes = System.Enum.GetValues(typeof(DataType));
        foreach (int type in dataTypes) {
            if (System.Enum.GetName(typeof(DataType), type).FirstOrDefault() == '_') continue;
            dataDic[(DataType)type] = new Attr();
        }
        // 初始化改变数据
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
    public Attr GetAttrDataByType(int type)
    {
        return GetAttrDataByType((DataType)type);
    }
    // 获取原属性
    public Attr GetAttrDataByType(DataType type)
    {
        if (!(IsAttrExist(type))) return null;
        return dataDic[(DataType)type];
    }
    // 获取原值
    public T GetAttrDataByType<T>(int type)
    {
        return GetAttrDataByType<T>((DataType)(type));
    }
    // 获取原值
    public T GetAttrDataByType<T>(DataType type)
    {
        var attr = GetAttrDataByType(type);
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
        DataInfluenceSystem.I.ApplyInfluenceByType(attr, type);
        DataInfluenceSystem.I.ApplyInfluence(attr, ConvertDataToInfluence(type));
        return attr.GetValue<T>();
    }
    // 把数值转换成影响
    public AttrInfluence ConvertDataToInfluence(int type)
    {
        return ConvertDataToInfluence((DataType)type);
    }
    public AttrInfluence ConvertDataToInfluence(DataType type)
    {
        var attr = GetAttrDataByType(type);
        return new AttrInfluence(){
            AttributeType = type,
            Attr = attr,
        };
    }
    // 设置值
    public void SetAttrDataByType<T>(int type, T value)
    {
        SetAttrDataByType((DataType)type, value);
    }
    // 设置值
    public void SetAttrDataByType<T>(DataType type, T value)
    {
        dataDic[type].SetValue<T>(value);
    }
    // 直接应用影响
    public void ApplyInfluenceList(List<AttrInfluence> list)
    {
        foreach (var influence in list)
        {
            var type = influence.AttributeType;
            var attrChange = new Attr();
            DataInfluenceSystem.I.ApplyChangeToAttr(attrChange, influence);
            dataChange[type] = attrChange;
            var attr = GetAttrDataByType(type);
            attr.SetValue(attr.GetValue<float>() + attrChange.GetValue<float>());
            // DataInfluenceSystem.I.ApplyChangeToAttr(attr, influence);
        }
    }
}
