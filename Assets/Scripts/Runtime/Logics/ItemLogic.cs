using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using System;

public enum ItemType
{
    Any = -1, //匹配所有类型使用
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
    public Func<List<LogicExecution>> HaveLogicListFunc;
    public Func<List<LogicExecution>> UseLogicListFunc;
    public Func<List<LogicExecution>> TurnLogicListFunc;
    public Func<List<LogicExecution>> DayLogicListFunc;
    public Func<List<LogicExecution>> RemoveLogicListFunc;
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
                Id = GameUtil.ItemId(1),
                Type = ItemType.Goods,
                Name = "烤肉",
                Desc = "恢复生命+3",
                UseLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.AttrChange, DIS.I.GetAttrInfluenceList(3f,0,0,0,0,0,0,0), null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = GameUtil.ItemId(2),
                Type = ItemType.Goods,
                Name = "护身符",
                Desc = "抵御消耗1次",
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = GameUtil.ItemId(3),
                Type = ItemType.Goods,
                Name = "万能钥匙",
                Desc = "开锁",
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = GameUtil.ItemId(4),
                Type = ItemType.Goods,
                Name = "万能车票",
                Desc = "重新选择入口",
                UseLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.ShowSelectScene, null, null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = GameUtil.ItemId(5),
                Type = ItemType.Goods,
                Name = "尿遁符",
                Desc = "跳过本回合",
                UseLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                    (Logic.SkipTurn, null, null)
                }),
                CostType = DataType.Gold,
                CostNum = 3,
            },
            new Item() {
                Id = GameUtil.ItemId(6),
                Type = ItemType.Goods,
                Name = "衣服",
                Desc = "好像可以穿?",
            },
            new Item() {
                Id = GameUtil.ItemId(10001),
                Type = ItemType.Buff,
                Name = "狂犬病",
                Desc = "看不见卡牌",
            },
            new Item() {
                Id = GameUtil.ItemId(10002),
                Type = ItemType.Buff,
                Name = "不想活了",
                Desc = "只会出现红色牌",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetLuckWeightInfluence(new List<(int, float)>() {
                        (0, 1),
                        (1, 0),
                    }, 999, true, 1000),
                    DIS.I.GetQualityWeightInfluence(new List<(CardQuality, float)>() {
                        (CardQuality.Red, 1),
                        (CardQuality.White, 0),
                        (CardQuality.Green, 0),
                        (CardQuality.Blue, 0),
                        (CardQuality.Purple, 0),
                        (CardQuality.Gold, 0),
                    }, 999, true, 1000)
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10003),
                Type = ItemType.Buff,
                Name = "抬不起头来",
                Desc = "20%几率直接死亡",
                TurnLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic l, object p, Condition c)>(){
                    (Logic.AttrChangeHurt, DIS.I.GetAttrInfluenceList(-9999), new Condition() {Formula = "random() <= 0.2"})
                }),
            },
            new Item() {
                Id = GameUtil.ItemId(10004),
                Type = ItemType.Buff,
                Name = "人生失去了目标",
                Desc = "每回合丢失一件装备",
                TurnLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic l, object p, Condition c)>(){
                    (Logic.RemoveItem, (ItemLogic.I.GetRandomHaveItemByType(ItemType.Equips)?.Id ?? -1, 1), null),
                }),
            },
            new Item() {
                Id = GameUtil.ItemId(10005),
                Type = ItemType.Buff,
                Name = "世界是灰色的",
                Desc = "所有收益为0",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetAttrInfluence(DataType.IncomeFactor, 0, 999, true, 10000),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10006),
                Type = ItemType.Buff,
                Name = "我怎么那么倒霉",
                Desc = "红白绿概率提升10",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetLuckWeightInfluence(new List<(int, float)>(){
                        (0, 1),
                        (1, 0),
                    }, 999, true, 10000),
                    DIS.I.GetQualityWeightInfluence(new List<(CardQuality, float)>(){
                        (CardQuality.Red, 25),
                        (CardQuality.White, 40),
                        (CardQuality.Green, 35),
                    }, 999, true, 10000),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10007),
                Type = ItemType.Buff,
                Name = "提不起兴趣",
                Desc = "减少一个选项",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetAttrInfluence(DataType.AnswerNumOffset, -1, 999),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10008),
                Type = ItemType.Buff,
                Name = "心灵受到打击",
                Desc = "生命消耗+1",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.HurtModifier, DIS.I.GetAttrInfluenceList(-1))
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10009),
                Type = ItemType.Buff,
                Name = "不想移动",
                Desc = "距离不增加",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.IncomeModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Distance, 0f, 0, true, 10000) })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10010),
                Type = ItemType.Buff,
                Name = "提不起劲来",
                Desc = "体力消耗翻倍",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.HurtModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Stamina, "Target") })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10011),
                Type = ItemType.Buff,
                Name = "丢三落四",
                Desc = "30%概率丢失道具",
                TurnLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic l, object p, Condition c)>(){
                    (Logic.RemoveItem, (GetRandomHaveItemByType(new List<ItemType>(){ItemType.Goods, ItemType.Equips}).Id, 1), new Condition() {Formula = "random() <= 0.3"}),
                }),
            },
            new Item() {
                Id = GameUtil.ItemId(10012),
                Type = ItemType.Buff,
                Name = "奇迹要出现了",
                Desc = "所有消耗变成收益",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.HurtModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Stamina, "-1*Target", 0, true) })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10013),
                Type = ItemType.Buff,
                Name = "全世界我最大",
                Desc = "道具, 任务, 装备直接获得",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetAttrInfluence(DataType.CostFactor, 0, 999, true, 10000),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10014),
                Type = ItemType.Buff,
                Name = "天降祥瑞",
                Desc = "蓝+20, 紫+15, 金+10",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetLuckWeightInfluence(new List<(int, float)>(){
                        (0, 25),
                        (1, 75),
                    }, 999, true, 10002),
                    DIS.I.GetQualityWeightInfluence(new List<(CardQuality, float)>(){
                        (CardQuality.Blue, 35),
                        (CardQuality.Purple, 25),
                        (CardQuality.Gold, 15),
                    }, 999, true, 10002),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10015),
                Type = ItemType.Buff,
                Name = "诚心求佛",
                Desc = "心情不再低落",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.HurtModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Mood, 0, 0, true) })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10016),
                Type = ItemType.Buff,
                Name = "转角遇到爱",
                Desc = "金色+10概率, 费用为0",
                HaveInfluenceList = new List<AttrInfluence>() {
                    DIS.I.GetQualityWeightInfluence(new List<(CardQuality, float)>(){(CardQuality.Gold, 15)}, 999, false),
                    DIS.I.GetAttrInfluence(DataType.CostFactor, 0, 999, true, 0, new Condition() { Formula = "TurnCardQuality() == 5" }),
                },
            },
            new Item() {
                Id = GameUtil.ItemId(10017),
                Type = ItemType.Buff,
                Name = "喜鹊围绕",
                Desc = "获得道具时, 50%几率获得2倍数量",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, Dictionary<int, float> value)>() {
                    (DataType.ItemNumModifier, new Dictionary<int, float> { {-1, 2f} })
                }, 999, false, 0, new Condition() { Formula = "random() <= 0.5" }),
            },
            new Item() {
                Id = GameUtil.ItemId(10018),
                Type = ItemType.Buff,
                Name = "天生我才必有用",
                Desc = "所有收益+1",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.IncomeModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Any, 1, 0, false, 0, new Condition() { Formula = "Target>0"}) })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10019),
                Type = ItemType.Buff,
                Name = "抖擞精神",
                Desc = "体力每回合+3",
                TurnLogicListFunc = () => CLS.I.GetLogicList(new List<(Logic l, object p, Condition c)>(){
                    (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,3,0,0,0,0,0,0), null),
                }),
            },
            new Item() {
                Id = GameUtil.ItemId(10020),
                Type = ItemType.Buff,
                Name = "有个好梦",
                Desc = "生命恢复+1",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.IncomeModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.HP, 1) })
                }, 999),
            },
            new Item() {
                Id = GameUtil.ItemId(10021),
                Type = ItemType.Buff,
                Name = "身轻如燕",
                Desc = "路程翻倍",
                HaveInfluenceList = DIS.I.GetAttrInfluenceList(new List<(DataType type, List<AttrInfluence> value)>() {
                    (DataType.IncomeModifier, new List<AttrInfluence>() { DIS.I.GetAttrInfluence(DataType.Distance, "Target") })
                }, 999),
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
    // 初始化
    public void Init()
    {
        // 测试满道具
        var itemData = allItemList.Where((i)=>i.Type != ItemType.Buff).ToDictionary((i)=>i.Id, (i)=>1);
        DataSystem.I.SetDataByType<Dictionary<int, int>>(DataType.Items, itemData);
        SyncItemToData();
        // AddItem(GameUtil.ItemId(10021), 1, new DurationAndFrequency() { Turn = 3});
        GameUILogic.I.UpdateItems();
    }
    // 同步数据
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
    // 更新回合
    public void UpdateTurn()
    {
        var keyList = haveItemDic.Keys.ToList();
        foreach (var key in keyList) {
            var item = haveItemDic[key];
            if (item.TurnLogicListFunc == null || item.TurnLogicListFunc().Count <= 0) continue;
            var num = item.Num;
            for (int i = 0; i < num; i++) {
                CLS.I.ExecuteCommonLogic(item.TurnLogicListFunc());
            }
        }
    }
    // 更新日
    public void UpdateDay()
    {
        var keyList = haveItemDic.Keys.ToList();
        foreach (var key in keyList) {
            var item = haveItemDic[key];
            if (item.DayLogicListFunc == null || item.DayLogicListFunc().Count <= 0) continue;
            var num = item.Num;
            for (int i = 0; i < num; i++) {
                CLS.I.ExecuteCommonLogic(item.DayLogicListFunc);
            }
        }
    }
    // 添加道具
    public void AddItem(int id, int num, DurationAndFrequency durFreOverride = null)
    {
        var attr = new Attr();
        var dic = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<int, float>>(DataType.ItemNumModifier);
        if (dic != null) {
            if (dic.ContainsKey(id)) {
                num = (int)Mathf.Floor(num * dic[id]);
            } else if (dic.ContainsKey(-1)) {
                num = (int)Mathf.Floor(num * dic[-1]);
            }
        }
        AddToData(id, num);
        AddToHave(id, num, false, durFreOverride);
    }
    void AddToHave(int id, int num, bool isSetNum = false, DurationAndFrequency durFreOverride = null)
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
        CLS.I.ExecuteCommonLogic(haveItem.HaveLogicListFunc);
        DIS.I.AddInfluence(haveItem.HaveInfluenceList);
        // 更新频率和时长
        if (durFreOverride != null) {
            haveItem.DurFre = durFreOverride;
        }
        if (haveItem.DurFre != null) {
            var durFreCopy = haveItem.DurFre.ShallowCopy();
            DurFreSystem.I.AddDurFreControl(durFreCopy, () => {
                ConsumeItem(id, num);
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
        DataSystem.I.SetDataByType(DataType.Items, itemDic);
    }
    // 移除道具
    public void RemoveItem(int id, int num)
    {
        SubtractFromData(id, num);
        SubtractFromHave(id, num);
    }
    // 使用道具
    public void ConsumeItem(int id, int num, bool isCheckConsumable = false)
    {
        if (isCheckConsumable && !IsItemConsumable(id)) {
            return;
        }
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
        DataSystem.I.SetDataByType(DataType.Items, itemDic);
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
        CLS.I.ExecuteCommonLogic(haveItem.RemoveLogicListFunc);
        DIS.I.RemoveInfluence(haveItem.HaveInfluenceList);
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
        var useItemLogicList = useItem.UseLogicListFunc;
        if (useItemLogicList == null) return;
        if (num > 0) {
            for (int i = 0; i < num; i++) {
                CLS.I.ExecuteCommonLogic(useItemLogicList);
            }
        }
    }

    // 获取列表
    public List<Item> GetHaveItemListByType(List<ItemType> typeList = null)
    {
        var goods = haveItemDic
            .Select((i)=>i.Value)
            .Where((i)=>(typeList == null || typeList.Contains(i.Type)) && i.Num > 0)
            .ToList();
        return goods;
    }
    public List<Item> GetHaveItemListByType(ItemType type)
    {
        return GetHaveItemListByType(new List<ItemType>() { type });
    }
    // 随机一个已有道具
    public Item GetRandomHaveItemByType(List<ItemType> typeList = null)
    {
        var haveList = GetHaveItemListByType(typeList);
        if (haveList == null || haveList.Count <= 0) return null;
        var randomIdx = UnityEngine.Random.Range(0, haveList.Count);
        return haveList[randomIdx];
    }
    public Item GetRandomHaveItemByType(ItemType type)
    {
        return GetRandomHaveItemByType(new List<ItemType>() { type });
    }
    // 检查道具是否可以使用
    public bool IsItemConsumable(int id)
    {
        return allItemDic.ContainsKey(id) && allItemDic[id].UseLogicListFunc != null;
    }
}
