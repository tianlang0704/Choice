using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttributeType {
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
    DayCards,           // 一天卡池
    _ValueTypeMax = 999, // 数值类型属性
    CardWeight,         // 卡片附加几率
}

public class DataSystem : SingletonBehaviour<DataSystem>
{
    public static float INVALID_ATTR = -99999999f;
    private Dictionary<AttributeType, Attr> dataDic = new Dictionary<AttributeType, Attr>();
    public Dictionary<AttributeType, Attr> DataDic {
        get {return dataDic;}
    }
    private Dictionary<AttributeType, Attr> dataChange = new Dictionary<AttributeType, Attr>();
    public Dictionary<AttributeType, Attr> DataChange {
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
        var dataTypes = System.Enum.GetValues(typeof(AttributeType));
        foreach (int type in dataTypes) {
            if (System.Enum.GetName(typeof(AttributeType), type).FirstOrDefault() == '_') continue;
            dataDic[(AttributeType)type] = new Attr();
        }
        // 初始化改变数据
        foreach (int type in dataTypes) {
            dataChange[(AttributeType)type] = new Attr();
        }
    }

    public bool IsAttrExist(AttributeType type)
    {
        if (!dataDic.ContainsKey(type)) return false;
        return true;
    }

    public Attr GetAttrDataByType(int type)
    {
        return GetAttrDataByType((AttributeType)type);
    }

    public Attr GetAttrDataByType(AttributeType type)
    {
        if (!(IsAttrExist(type))) return null;
        return dataDic[(AttributeType)type];
    }

    public AttrInfluence GetInfluenceFromType(int type)
    {
        return GetInfluenceFromType((AttributeType)type);
    }

    public AttrInfluence GetInfluenceFromType(AttributeType type)
    {
        var attr = GetAttrDataByType(type);
        return new AttrInfluence(){
            attributeType = type,
            attr = attr,
        };
    }

    public T GetAttrDataByType<T>(AttributeType type)
    {
        var attr = GetAttrDataByType(type);
        return attr.GetValue<T>();
    }

    public T CopyAttrDataWithInfluenceByType<T>(AttributeType type)
    {
        var attr = new Attr();
        DataInfluenceSystem.I.ApplyInfluenceByType(attr, type);
        DataInfluenceSystem.I.ApplyInfluence(attr, GetInfluenceFromType(type));
        return attr.GetValue<T>();
    }

    public void SetAttrDataByType<T>(int type, T value)
    {
        SetAttrDataByType((AttributeType)type, value);
    }

    public void SetAttrDataByType<T>(AttributeType type, T value)
    {
        dataDic[type].SetValue<T>(value);
    }

    public void ApplyInfluenceList(List<AttrInfluence> list)
    {
        foreach (var influence in list)
        {
            var type = influence.attributeType;
            var attr = GetAttrDataByType(type);
            DataInfluenceSystem.I.ApplyChangeToAttr(attr, influence);
            var attrChange = new Attr();
            DataInfluenceSystem.I.ApplyChangeToAttr(attrChange, influence);
            dataChange[type] = attrChange;
        }
    }
}
