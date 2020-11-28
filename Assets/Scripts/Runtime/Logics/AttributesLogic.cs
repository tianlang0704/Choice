using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesLogic : SingletonBehaviour<AttributesLogic>
{
    public List<int> dataList = new List<int>();
    public List<int> dataChange = new List<int>();
    protected void Awake()
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

    public void InitData() {
        dataList.Clear();
        dataChange.Clear();
        // 初始化全部数据为100
        var dataTypes = System.Enum.GetValues(typeof(AttributeType));
        foreach (int type in dataTypes) {
            dataList.Add(0);
            dataList[type] = 5;
        }
        // 初始化改变数据为0
        foreach (int type in dataTypes) {
            dataChange.Add(0);
            dataChange[type] = 0;
        }
    }

    public void ApplyChangeToData(List<Influence> influenceList) {
        foreach (var influence in influenceList)
        {
            var typeInt = (int)influence.attributeType;
            var changeAmount = influence.amount;
            var curValue = dataList[typeInt];
            dataList[typeInt] = curValue + (int)changeAmount;
            dataChange[typeInt] = (int)changeAmount;
        }
    }

    public bool IsDead()
    {
        var dataTypes = System.Enum.GetValues(typeof(AttributeType));
        foreach (int type in dataTypes) {
            if (dataList[type] <= 0) return true;
        }
        return false;
    }
}
