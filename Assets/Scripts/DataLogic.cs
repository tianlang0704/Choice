using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLogic : SingletonBehaviour<DataLogic>
{
    public enum DataType {
        Oil = 0,
        Water,
        HP,
        Knowledge,
    }
    public List<int> dataList = new List<int>();
    public List<int> dataChange = new List<int>();
    override protected void Awake()
    {
        base.Awake();
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

    void InitData() {
        // 初始化全部数据为100
        var dataTypes = System.Enum.GetValues(typeof(DataLogic.DataType));
        foreach (int type in dataTypes) {
            dataList.Add(0);
            dataList[type] = 100;
        }
        // 初始化改变数据为0
        foreach (int type in dataTypes) {
            dataChange.Add(0);
            dataChange[type] = 0;
        }
    }

    public void ApplyChangeToData(int[] changeArr) {
        foreach (int type in System.Enum.GetValues(typeof(DataLogic.DataType))) {
            if (type >= changeArr.Length) break;
            var typeChange = changeArr[type];
            var curValue = dataList[type];
            dataList[type] = curValue + typeChange;
            dataChange[type] = typeChange;
        }
    }
}
