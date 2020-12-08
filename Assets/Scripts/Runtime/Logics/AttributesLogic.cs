using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttributesLogic : SingletonBehaviour<AttributesLogic>
{
    public List<AttributeType> DisplayAttrTypes = new List<AttributeType>() {
        AttributeType.Hp,
        AttributeType.Oil,
        AttributeType.Water,
        AttributeType.Knowledge,
    };

    public List<AttributeType> DeadlyAttrTypes = new List<AttributeType>() {
        AttributeType.Hp,
        AttributeType.Oil,
        AttributeType.Water,
        AttributeType.Knowledge,
    };

    public List<float> DisplayAttrData {
        get { 
            return AttributeDataSystem.Instance.DataList
                .Where((attr, idx) => DisplayAttrTypes.Contains((AttributeType)idx))
                .Select((attr) => attr.floatValue)
                .ToList(); 
        }
    }

    public List<float> DisplayAttrDataChange {
        get { 
            return AttributeDataSystem.Instance.DataChange
                .Where((attr, idx) => DisplayAttrTypes.Contains((AttributeType)idx))
                .Select((attr) => attr.floatValue)
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
            var attrData = AttributeDataSystem.Instance.GetAttrDataByType(type);
            if (IsAttrTypeDeadly((AttributeType)type) && attrData.floatValue <= 0) return true;
        }
        return false;
    }
}
