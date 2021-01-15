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
    Buff,
    Unkown = 999,
}

public class Item
{
    public int Id;
    public int Num;
    public ItemType Type;
    public string Name;
    public string Desc;
    public string Icon;
    public DurationAndFrequency DurFre;
    public List<AttrInfluence> HaveInfluenceList;
    public List<LogicExecution> HaveLogicList;
    public List<LogicExecution> UseLogicList;
    public List<LogicExecution> RemoveLogicList;
    public DataType CostType;
    public float CostNum;
    public Item ShallowCopy() {
        return MemberwiseClone() as Item;
    }
}

public class ItemLogic : SingletonBehaviour<ItemLogic>
{
    List<Item> allItemList;
    Dictionary<int, Item> allItemDic = new Dictionary<int, Item>();
    Dictionary<int, Item> haveItemDic = new Dictionary<int, Item>();

    void Awake()
    {
        allItemList = new List<Item>() {
            new Item() {
                Id = 1,
                Type = ItemType.Goods,
                Name = "烤肉",
                Desc = "恢复生命+3",
                UseLogicList = CommonLogicSystem.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.AttrChange, AIS.I.GetAttrInfluences(3f,0,0,0,0,0,0,0), null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = 2,
                Type = ItemType.Goods,
                Name = "护身符",
                Desc = "抵御消耗1次",
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = 3,
                Type = ItemType.Goods,
                Name = "万能钥匙",
                Desc = "开锁",
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = 4,
                Type = ItemType.Goods,
                Name = "万能车票",
                Desc = "重新选择入口",
                UseLogicList = CommonLogicSystem.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.ShowSelectScene, null, null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = 5,
                Type = ItemType.Goods,
                Name = "尿遁符",
                Desc = "跳过本回合",
                UseLogicList = CommonLogicSystem.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.SkipTurn, null, null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = 6,
                Type = ItemType.Buff,
                Name = "狂犬病",
                Desc = "看不见卡牌",
                DurFre = new DurationAndFrequency() { turn = 2 }
            },
            new Item() {
                Id = 7,
                Type = ItemType.Goods,
                Name = "衣服",
                Desc = "好像可以穿?",
            },
        };
        allItemList.ForEach((i) => allItemDic[i.Id] = i);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        // 测试满道具
        var itemData = allItemList.Where((i)=>i.Type != ItemType.Buff).ToDictionary((i)=>i.Id, (i)=>1);
        DataSystem.I.SetAttrDataByType<Dictionary<int, int>>(DataType.Items, itemData);
        SyncItemToData();
        GameUILogic.I.UpdateItems();
    }

    public void SyncItemToData()
    {
        var itemDic = DataSystem.I.GetAttrDataByType<Dictionary<int, int>>(DataType.Items);
        if (itemDic == null) return;
        // 重新同步数据
        haveItemDic.Clear();
        foreach (var itemKvp in itemDic) {
            AddToHave(itemKvp.Key, itemKvp.Value, true);
        }
    }
    // 检查是否拥有道具ID
    public bool IsHaveItem(int id)
    {
        var itemDic = DataSystem.I.GetAttrDataByType<Dictionary<int, int>>(DataType.Items);
        return itemDic.ContainsKey(id) && itemDic[id] > 0;
    }
    // 添加道具
    public void AddItem(int id, int num)
    {
        AddToData(id, num);
        AddToHave(id, num);
    }
    void AddToHave(int id, int num, bool isSetNum = false)
    {
        // 创建或获取现有道具
        Item haveItem = null;
        if (haveItemDic.ContainsKey(id)) {
            haveItem = haveItemDic[id];
        } else if (allItemDic.ContainsKey(id)) {
            haveItem = allItemDic[id].ShallowCopy();
        }
        if (haveItem == null) {
            return;
        }
        // 更新数量
        if (isSetNum) {
            haveItem.Num = num;
        } else {
            haveItem.Num += num;
        }
        // 更新拥有道具
        haveItemDic[id] = haveItem;
        // 更新拥有效果
        CommonLogicSystem.I.ExecuteCommonLogic(haveItem.HaveLogicList);
        DataInfluenceSystem.I.AddInfluence(haveItem.HaveInfluenceList);
        // 更新频率和时长
        if (haveItem.DurFre != null) {
            var durFreCopy = haveItem.DurFre.ShallowCopy();
            DurFreSystem.I.AddDurFreControl(durFreCopy, () => {
                ConsumeItem(id, 1);
            });
        }
    }
    void AddToData(int id, int num)
    {
        // 获取ID对应的记录
        var itemDic = DataSystem.I.GetAttrDataByType<Dictionary<int, int>>(DataType.Items);
        if (itemDic == null)
            itemDic = new Dictionary<int, int>();
        // 添加数量
        var existNum = 0;
        if (itemDic.ContainsKey(id))
            existNum = itemDic[id];
        existNum += num;
        // 存值
        itemDic[id] = existNum;
        DataSystem.I.SetAttrDataByType(DataType.Items, itemDic);
    }

    // 使用道具
    public void ConsumeItem(int id, int num)
    {
        int actualNum = SubtractFromData(id, num);
        SubtractFromHave(id, num);
        ExecuteUseLogic(id, actualNum);
    }
    int SubtractFromData(int id, int num, bool isLimitToHave = false)
    {
        // 获取ID对应的记录, 没有就直接返回
        var itemDic = DataSystem.I.GetAttrDataByType<Dictionary<int, int>>(DataType.Items);
        if (itemDic == null)
            return 0;
        // 减值, 如果设定不能为负数就最多只减拥有量
        var existNum = 0;
        if (itemDic.ContainsKey(id))
            existNum = itemDic[id];
        var actualNum = num;
        if (isLimitToHave) {
            actualNum = Mathf.Min(existNum, num);
        }
        existNum -= actualNum;
        // 存储回去
        itemDic[id] = existNum;
        DataSystem.I.SetAttrDataByType(DataType.Items, itemDic);
        // 返回实际减少数量
        return actualNum;
    }
    int SubtractFromHave(int id, int num, bool isLimitToHave = false)
    {
        if (!haveItemDic.ContainsKey(id)) 
            return 0;
        var actualNum = num;
        var haveItem = haveItemDic[id];
        if (isLimitToHave) {
            actualNum = Mathf.Min(haveItem.Num, num);
        }
        haveItem.Num -= actualNum;
        // 更新效果
        CommonLogicSystem.I.ExecuteCommonLogic(haveItem.RemoveLogicList);
        DataInfluenceSystem.I.RemoveInfluence(haveItem.HaveInfluenceList);
        // 更新频率和时长
        if (haveItem.DurFre != null) {
            DurFreSystem.I.RemoveDurFreControl(haveItem.DurFre);
        }
        return actualNum;
    }

    // 运行道具使用逻辑
    void ExecuteUseLogic(int id, int num)
    {
        if (!allItemDic.ContainsKey(id)) return;
        var useItem = allItemDic[id];
        var useItemLogicList = useItem.UseLogicList;
        if (useItemLogicList == null) return;
        if (num > 0) {
            for (int i = 0; i < num; i++) {
                CommonLogicSystem.I.ExecuteCommonLogic(useItemLogicList);
            }
        }
    }

    public List<Item> GetGoods()
    {
        var goods = haveItemDic.Select((i)=>i.Value).Where((i)=>i.Type == ItemType.Goods && i.Num > 0).ToList();
        return goods;
    }
}
