using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType {
    Oil = 0,        // 油
    Water,          // 水
    Hp,             // 体力
    Knowledge,      // 知识
    Luck,           // 幸运
    Bag,            // 负重
    Distance,       // 距离
    Day,            // 天数
}

public class AttrInfluence 
{
    public AttributeType attributeType;
    public Attr attr;
}

public class Attr
{
    public enum AttrDataType {
        FLOAT = 0,
        STR_FLO_DIC,
    }
    public Attr(AttrDataType type = AttrDataType.FLOAT) {
        if (type == AttrDataType.STR_FLO_DIC) {
            strFloDic = new Dictionary<string, float>();
        }
    }
    public AttrDataType type = AttrDataType.FLOAT;
    public float floatValue = 0;
    public Dictionary<string, float> strFloDic;
}

public class AttributeDataSystem : SingletonBehaviour<AttributeDataSystem>
{
    public static float INVALID_ATTR = -99999999f;
    private List<Attr> dataList = new List<Attr>();
    public List<Attr> DataList {
        get {return dataList;}
    }
    private List<Attr> dataChange = new List<Attr>();
    public List<Attr> DataChange {
        get {return dataChange;}
    }

    void Awake()
    {
        InitData();
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
    public void InitData() 
    {
        DataList.Clear();
        DataChange.Clear();
        // 初始化全部数据为100
        var dataTypes = System.Enum.GetValues(typeof(AttributeType));
        foreach (int type in dataTypes) {
            DataList.Add(new Attr() {floatValue = 5});
        }
        // 初始化改变数据为0
        foreach (int type in dataTypes) {
            DataChange.Add(new Attr() {floatValue = 0});
        }
    }

    // 从应用影响列表
    public void ApplyChangeToData(List<AttrInfluence> influenceList) 
    {
        if (influenceList == null) return;
        foreach (var influence in influenceList)
        {
            var typeInt = (int)influence.attributeType;
            if (influence.attr.type == Attr.AttrDataType.FLOAT) {
                var changeAmount = influence.attr.floatValue;
                DataChange[typeInt].floatValue = changeAmount;
                var curValue = DataList[typeInt].floatValue;
                DataList[typeInt].floatValue = curValue + changeAmount;
            } else {
                print("ApplyChangeToData Attr类型未实现");
            }
        }
    }

    
    public Attr GetAttrDataByType(int type)
    {
        if (type >= DataList.Count) return null;
        return DataList[type];
    }
}
