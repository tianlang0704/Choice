using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttributesLogic : SingletonBehaviour<AttributesLogic>
{
    public List<DataType> DisplayAttrTypes = new List<DataType>() {
        DataType.Mood,
        DataType.HP,
        DataType.Stamina,
        DataType.Gold,
    };

    public List<DataType> DeadlyAttrTypes = new List<DataType>() {
        DataType.Mood,
        DataType.HP,
        DataType.Stamina,
        DataType.Gold,
    };

    private Dictionary<DataType, float> maxValueDic = new Dictionary<DataType, float>();

    public List<float> DisplayAttrData {
        get { 
            return DataSystem.I.DataDic
                .Where((attrKvp) => DisplayAttrTypes.Contains((DataType)attrKvp.Key))
                .Select((attrKvp) => DataSystem.I.CopyAttrDataWithInfluenceByType<float>(attrKvp.Key))
                .ToList(); 
        }
    }

    public List<float> DisplayAttrDataChange {
        get { 
            return DataSystem.I.DataChange
                .Where((attrKvp) => DisplayAttrTypes.Contains((DataType)attrKvp.Key))
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

    public bool IsAttrTypeDeadly(DataType type)
    {
        if (DeadlyAttrTypes.Contains(type)) return true;
        return false;
    }

    public bool IsDead()
    {
        var attrTypes = System.Enum.GetValues(typeof(DataType));
        foreach (int type in attrTypes) {
            var attrData = DataSystem.Instance.GetAttrDataByType(type);
            if (IsAttrTypeDeadly((DataType)type) && attrData <= 0) return true;
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
        DataSystem.I.SetAttrDataByType(DataType.Bag, 5);
        DataSystem.I.SetAttrDataByType(DataType.Luck, 0);
        DataSystem.I.SetAttrDataByType(DataType.Distance, 0);
        DataSystem.I.SetAttrDataByType(DataType.Day, 0);
        // 初始化属性最大值
        maxValueDic[DataType.Mood] = 20;
        maxValueDic[DataType.HP] = 20;
        maxValueDic[DataType.Stamina] = 20;
        maxValueDic[DataType.Gold] = 20;
    }
}
