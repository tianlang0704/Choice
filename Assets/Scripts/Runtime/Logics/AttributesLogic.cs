using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttributesLogic : SingletonBehaviour<AttributesLogic>
{
    public List<AttributeType> DisplayAttrTypes = new List<AttributeType>() {
        AttributeType.Mood,
        AttributeType.HP,
        AttributeType.Stamina,
        AttributeType.Gold,
    };

    public List<AttributeType> DeadlyAttrTypes = new List<AttributeType>() {
        AttributeType.Mood,
        AttributeType.HP,
        AttributeType.Stamina,
        AttributeType.Gold,
    };


    public List<float> DisplayAttrData {
        get { 
            return DataSystem.I.DataDic
                .Where((attrKvp) => DisplayAttrTypes.Contains((AttributeType)attrKvp.Key))
                .Select((attrKvp) => (float)attrKvp.Value)
                .ToList(); 
        }
    }

    public List<float> DisplayAttrDataChange {
        get { 
            return DataSystem.I.DataChange
                .Where((attrKvp) => DisplayAttrTypes.Contains((AttributeType)attrKvp.Key))
                .Select((attrKvp) => (float)attrKvp.Value)
                .ToList(); 
        }
    }

    protected void Awake()
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

    public bool IsAttrTypeDeadly(AttributeType type)
    {
        if (DeadlyAttrTypes.Contains(type)) return true;
        return false;
    }

    public bool IsDead()
    {
        var attrTypes = System.Enum.GetValues(typeof(AttributeType));
        foreach (int type in attrTypes) {
            var attrData = DataSystem.Instance.GetAttrDataByType(type);
            if (IsAttrTypeDeadly((AttributeType)type) && attrData <= 0) return true;
        }
        return false;
    }

    public void Init()
    {
        // 初始化显示属性
        foreach (var type in DisplayAttrTypes) {
            DataSystem.I.SetAttrDataByType(type, 10);
        }
        // 初始化其他属性
        DataSystem.I.SetAttrDataByType(AttributeType.Bag, 5);
        DataSystem.I.SetAttrDataByType(AttributeType.Luck, 0);
        DataSystem.I.SetAttrDataByType(AttributeType.Distance, 0);
        DataSystem.I.SetAttrDataByType(AttributeType.Day, 0);
    }
}
