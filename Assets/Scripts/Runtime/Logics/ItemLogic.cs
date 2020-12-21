using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;

public enum ItemType
{
    Goods = 0,
    Equips,
    Relics,
}

public class Item
{
    public int Id;
    public ItemType Type;
    public string name;
    public string desc;
    public string icon;
    public List<AttrInfluence> influences;
    public List<LogicExecution> logics;
}

public class ItemLogic : SingletonBehaviour<ItemLogic>
{
    List<Item> allGoods;

    void Wake()
    {
        allGoods = new List<Item>() {
            new Item() {
                Id = 1,
                Type = ItemType.Goods,
                name = "测试道具",
                desc = "这是一个测试道具",
                influences = AIS.I.GetAttrInfluences(1f,0,0,0,0,0,0,0),
                logics = CommonLogicSystem.I.GetLogicList(
                    Logic.AttrChange, AIS.I.GetAttrInfluences(1f,0,0,0,0,0,0,0)
                )
            },
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
}
