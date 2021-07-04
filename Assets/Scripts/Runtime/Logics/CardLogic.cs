using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using CS = ConditionSystem;
using LEList = System.Collections.Generic.List<(Logic, object, Condition)>;
using Random = UnityEngine.Random;

public enum CardType { None = -10000, End = -3, Blank = -2, Any = -1, Start = 0, Rest, Event, SceneChange, Shop, Special}
public enum CardQuality { Any = -1, Red = 0, White, Green, Blue, Purple, Gold }
public enum LuckQualityGroup { Any = -1, Low = 0, High }
public class Card 
{
    public int Id;
    public int DrawPriority = 0;
    public bool IsMaskable = true;
    public CardType Type = CardType.Event;
    public CardQuality Quality = CardQuality.White;
    public Condition FillCondition;
    // public Condition DrawCondition;
    public Condition SeeCondition = new Condition() { Formula = $"IsHaveItem({GameUtil.ItemId(10001)}) == 0" };
    public string content = "";
    public float baseWeight = 1;
    public List<Answer> answers;
    public Card ShallowCopy()
    {
        Card copy = (Card)this.MemberwiseClone();
        copy.answers = new List<Answer>();
        for (int i = 0; i < answers.Count; i++) {
            var a = answers[i].ShallowCopy();
            copy.answers.Add(a);
        }
        return copy;
    }
}

public class WhiteCard : Card
{
    public int secondChoice = 0;
}

public class Answer
{
    public string content;
    public Condition condition;
    public List<DataType> fillTypeList;
    public List<DataType> showTypeList;
    public List<Func<List<LogicExecution>>> logicListFuncList;
    public Answer ShallowCopy()
    {
        var copy = (Answer)this.MemberwiseClone();
        if (fillTypeList != null) {
            copy.fillTypeList = new List<DataType>(fillTypeList);
        }
        if (logicListFuncList != null) {
            copy.logicListFuncList = new List<Func<List<LogicExecution>>>();
            foreach (var listFunc in logicListFuncList) {
                copy.logicListFuncList.Add(new Func<List<LogicExecution>>(listFunc));
            }
            // copy.logicListFuncList = new List<Func<List<LogicExecution>>>(logicListFuncList);
        }
        return copy;
    }
}

public class CardLogic : SingletonBehaviour<CardLogic>
{
    public List<Card> AllCards;
    public Dictionary<int, Card> AllCardsIdIndex = new Dictionary<int, Card>();

    void Awake()
    {
        AllCards = new List<Card>(){
            new Card() {
                Id = GameUtil.CardId(10001),
                Type = CardType.Rest,
                Quality = CardQuality.Any,
                DrawPriority = 10001,
                IsMaskable = false,
                content = "今天结束了, 请休息",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "休息一下",
                        // logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                        //     (Logic.AttrChange, WeatherLogic.I.GetCurrentWeather().baseConsumption, null),
                        //     (Logic.AttrChange, DIS.I.GetAttrInfluenceList(0,999,0,0,0,0,0,0), null),
                        //     (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.HP, "Value*0.2"), null),
                        // });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10002),
                Type = CardType.Special,
                Quality = CardQuality.Any,
                IsMaskable = false,
                FillCondition = new Condition() {Formula = "0"},
                content = "今天没有抽到卡",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "跳过今天",
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10003),
                Type = CardType.Start,
                Quality = CardQuality.Any,
                DrawPriority = 10001,
                IsMaskable = false,
                content = "开始冒险",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "GO",
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10004),
                Type = CardType.End,
                Quality = CardQuality.Any,
                DrawPriority = 10001,
                IsMaskable = false,
                content = "耍完了, 回城吧",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "THE END",
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10005),
                Type = CardType.Blank,
                Quality = CardQuality.Any,
                DrawPriority = 10001,
                IsMaskable = false,
                answers = new List<Answer>() {
                    new Answer() {
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.Gold, "1"), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.Stamina, "-2"), null),
                        });}},
                    },
                    new Answer() {
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.Distance, "3"), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.Stamina, "-4"), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(1),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "2触发：这只狗子，在后面一直跟着你。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "被咬",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "快跑",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "好害怕",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "丢钱",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(2),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "31-2这个野孩子，在后面用石子追你",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "砸中了",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "快躲",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "好烦哦",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "给他点钱",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(3),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路过一伙刁民将你拦下，想要从此过，留下买路钱。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "被墙角",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "快跑啊",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "被吐唾沫",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "交钱、交钱",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(4),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "你留意到，路边的杂草中有个什么（喵）",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "哎呀、挠到了",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "搭个窝窝",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "不管，不管",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个肉肉给它",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(5),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "日头暴晒，口渴难耐。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "好渴啊",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "好累啊",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "好难啊",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个水",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(6),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "走着走着，石子崴了脚。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "不得虚",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "揉一揉",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "好倒霉啊",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买点药，摸摸",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(7),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "7·1走着走着，又崴了脚。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "嘶嘶嘶嘶、疼！",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "单腿蹦着走！",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "霉死我了！",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "弄个绑腿！",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(8),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "41-1羊娃带着羊群，一边吹哨，一边猛追。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "顶了个大跟头",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "块！往树上爬",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "哎呀哎呀！救命啊！",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "错了错了！我给钱",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(9),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "16-1 河中碎石，甚是光滑，小心为上",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "拉了个口子",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "摸石头走",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "过河好难啊",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "花钱，渡河！",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "草原蚊虫好多，整个腿上全是包",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "么的事",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "麻烦点，扫蚊子",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "嗡嗡嗡嗡嗡嗡嗡",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买！6神花露水！",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(11),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "远处听到了马声，人声，喧嚣声，不好，马贼",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "舍命不舍财",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "快快快，躲起来",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "完了！完了！完了！",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "交钱，卖平安",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(12),
                Quality = CardQuality.Red,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "踩到了个东西",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "捕兽夹",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "狗尾巴",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "大便",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "钱袋子",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(13),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "伤的这么重，坐下休息下（出现的buff情况，大概需出现，越严重，概率越大）",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "没得事，这算啥，接着走",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "休息，休息",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "世界如此美好，我却如此暴躁",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "大哥大姐，行行好吧",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(14),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "3触发:黑狗叼着一袋不知道，什么东西，向你跑了过来。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "大药水",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "肉包子",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "向日葵",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "钱袋子",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(15),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路边有个凉茶店，坐下",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "补枸杞",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "可口茶",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "听个曲",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "卖个艺",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(16),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "前面有间客栈，有个老板娘，丰韵挺实。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "来口奶喝喝",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "打个盹儿",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "聊聊风花",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "打打零工",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(17),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "31-1他还是，悄悄跟着你",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "给你塞了点草药，抹在你的伤口上。",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "偷偷塞给你一口吃的。",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "给你摘了点花，冲你傻傻一笑。",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "不知道从弄哪来了点钱，塞到你了你的包里。",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(18),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "41-1结伴而行，闲庭漫步。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "嚼了口草，土办法",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "帮你托书包",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "教你唱山歌",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "帮他放羊",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(19),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "41-3羊娃指着前方，那里有条河，河水清澈甘甜可口。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "救命水",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "洗洗脚",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "真清凉",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "捞点石头卖钱",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(20),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "哎呦 猫猫过来蹭了蹭你",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "舔一舔你的脸",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "趴在你腿边",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "咕噜咕噜咕噜咕噜",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "叼了几个克金币",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(21),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "草原广阔，忍不住放声歌唱。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "新鲜空气",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "顺风路",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "放声歌唱",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(22),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "红的，绿的，黄的，白的，紫的小野花，全是小蝴蝶",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "扑蝴蝶～",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "摘下来绑个花冠戴",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(23),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "我在马路边，见到五分钱",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "买药摸摸",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个水喝",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "拾金不昧好儿郎",
                        fillTypeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "偷偷藏在兜里面",
                        fillTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(24),
                Quality = CardQuality.Green,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "24-2 马儿嘶鸣一声，好像是叫你上来",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "上！找大夫",
                        fillTypeList = new List<DataType>() {DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "上！找爸爸",
                        fillTypeList = new List<DataType>() {DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(25),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "5-2小猫爬出来，发出咕咕的声音。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "逗逗它",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买点肉肉",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(26),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "你走着走着，发现有个野孩子，一直跟着你。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "找他打零工",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "去他家帮忙",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "喊他买药",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "找他买的水喝",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "给点钱花",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(27),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路边有个破旧的土地庙，香火了了。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "扫扫灰，除除草",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "捐个香火，跪拜一下。",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(28),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "野花野草，匆匆绿绿。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "慢慢走，慢慢看",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(29),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "从后面有个农夫赶着驴车与你擦肩。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "帮工",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "帮他推车",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "坐上歇会",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买点果子",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(30),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "炎炎夏日，路中间趴着一条黑狗，抬头望着你。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "陪它玩会",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个肉包给他吃",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(31),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "看到颗歪脖树，前路难行，弄个拐杖吧",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "捡点柴火",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "爬树上看风景",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(32),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "马儿，在吃草",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "跟它走走",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买点草料",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(33),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "兔子！追！晚上有的肉吃了",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "抓它！",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "抓！",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(34),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "被河水挡住了前方的道路",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "玩水！",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "搭船",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(35),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "看到了放羊娃带着一堆羊从那边过来",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "帮他放羊",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "陪着一起走，有个陪伴",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "问他买点羊奶",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个馍馍",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买个铃铛",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(36),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "绿草青青，一马平川。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "打工",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "游玩",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买草药",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "搭驴车",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买点果子",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(37),
                Quality = CardQuality.White,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "看到前面有个旅行者，赶上去。",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "赶路",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Distance},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "拎包服务",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "免费拎包",
                        fillTypeList = new List<DataType>() {DataType.Stamina, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买药膏",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.HP},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买红牛",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Stamina},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "买地图",
                        fillTypeList = new List<DataType>() {DataType.Gold, DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(1,0,0,0,0,0,0,0), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(38),
                Quality = CardQuality.Blue,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路上捡到了个",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "哇～",
                        showTypeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeHurtIncome, DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-6,-2)"), null),
                            (Logic.AddItem, (DIS.I.GetAttrInfluence("RandomGoodsId()"), DIS.I.GetAttrInfluence("1")), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(39),
                Quality = CardQuality.Blue,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "走街货郎，看看他卖的是什么",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "买点",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AddItem, (DIS.I.GetAttrInfluence("RandomGoodsId()"), DIS.I.GetAttrInfluence("1")), null),
                        });}},
                    },
                }
            },
            // new Card() {
            //     Id = GameUtil.CardId(40),
            //     Quality = CardQuality.Blue,
            //     FillCondition = new Condition() {Formula = "Scene == 1"},
            //     content = "前面有个岔路。",
            //     answers = new List<Answer>() {
            //         new Answer() {
            //             content = "接着直走",
            //             logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
            //                 (Logic.ShowSelectScene, null, null),
            //                 (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.IsPreventNextTurnOnce, "1"), null),
            //             });}},
            //         },
            //     }
            // },
            new Card() {
                Id = GameUtil.CardId(41),
                Quality = CardQuality.Gold,
                Type = CardType.Shop,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "当铺, 看看设么可卖的",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "卖",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.ShowShopType, (new List<ItemType>() {ItemType.Goods, ItemType.Equips}, false), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.IsPreventNextTurnOnce, "1"), null),
                        });}},
                    },
                    new Answer() {
                        content = "买装备",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.ShowShopType, (new List<ItemType>() {ItemType.Equips}, true), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.IsPreventNextTurnOnce, "1"), null),
                        });}},
                    },
                    new Answer() {
                        content = "买道具",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.ShowShopType, (new List<ItemType>() {ItemType.Goods}, true), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.IsPreventNextTurnOnce, "1"), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(42),
                Quality = CardQuality.Gold,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "前面霞光异彩，必有好物",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "哇～",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.ShowShopType, (new List<ItemType>() {ItemType.Goods, ItemType.Equips}, true), null),
                            (Logic.AttrChange, DIS.I.GetAttrInfluenceList(DataType.IsPreventNextTurnOnce, "1"), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(43),
                Quality = CardQuality.Gold,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "前面是谁呀～追上去",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "遇到人物",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AddItem, (DIS.I.GetAttrInfluence("RandomRelicsId()"), DIS.I.GetAttrInfluence("1")), null),
                        });}},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(44),
                Quality = CardQuality.Gold,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "寻宝地图",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "随机获取装备",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AddItem, (DIS.I.GetAttrInfluence("RandomEquipsId()"), DIS.I.GetAttrInfluence("1")), null),
                        });}},
                    },
                }
            },
        };
        foreach (var card in AllCards) {
            AllCardsIdIndex[card.Id] = card;
        }
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
        // 初始化答案数量
        DataSystem.I.SetDataByType(DataType.AnswerNum, 2);
    }

    public Card GetCardById(int id)
    {
        if (!AllCardsIdIndex.ContainsKey(id)) return null;
        return AllCardsIdIndex[id];
    }

    public Card GetCardCopyById(int id)
    {
        if (!AllCardsIdIndex.ContainsKey(id)) return null;
        return AllCardsIdIndex[id].ShallowCopy();
    }

    public List<Card> GetCardListByType(CardType cardType)
    {
        return AllCards.Where((c) => c.Type == cardType).ToList();
    }

    public void MaskCard(Card c)
    {
        c.content = "???";
        var oriAnswers = c.answers;
        for (int i = 0; i < oriAnswers.Count; i++) {
            var a = oriAnswers[i];
            a.content = "????";
        }
    }
    // 从答案列表中根据填充类型和填充列表选出可用的答案
    public Dictionary<Answer, List<LogicExecution>> FilterAnswerByLogicCombiList(
        List<Answer> answers, 
        List<List<LogicExecution>> leListList, 
        bool isIncludeNull = true
    ){
        // 目标列表为空则直接返回所有答案
        if (leListList == null) {
            return answers.ToDictionary((a)=>a, (a)=>(List<LogicExecution>)null);
        }
        // 从逻辑列表中获取所有属性改变逻辑
        var res = new Dictionary<Answer, List<LogicExecution>>();
        var validLEListList = leListList.Where((lewList)=>lewList.Where((le)=>le.Logic < Logic._ATTR_MAX_).Any()).ToList();
        if (validLEListList.Count <= 0) {
            // 有列表但是列表为空, 返回所有不可填属性值卡牌
            res = answers.Where((a)=>a.fillTypeList == null).ToDictionary((a)=>a, (a)=>(List<LogicExecution>)null);
        } else {
            // 有列表也有内容, 就过滤对应的可填属性值卡牌
            res = FilterMatchAnswer(answers, validLEListList, isIncludeNull);
        }
        return res;
    }
    public Dictionary<Answer, List<LogicExecution>> FilterMatchAnswer(
        List<Answer> answers, 
        List<List<LogicExecution>> leListList, 
        bool isIncludeNull = true
    ){
        var res = new Dictionary<Answer, List<LogicExecution>>();
        var validLEListListTemp = new List<List<LogicExecution>>(leListList);
        var answersTemp = new List<Answer>(answers);
        // 为每个属性改变寻找配对的答案
        for (int i = validLEListListTemp.Count - 1; i >= 0; i--) {
            // 检查答案数量
            if (answersTemp.Count <= 0) break;
            // 找到第一个符合条件的逻辑
            var leList = validLEListListTemp[i];
            var leToUse = leList.Where((le)=>le.Logic < Logic._ATTR_MAX_).FirstOrDefault();
            if (leToUse == null) continue;
            // 从逻辑获得对应的属性
            var targetTypeList = (leToUse.Param as List<AttrInfluence>)?.Select((influ)=>influ.AttributeType).ToList();
            if (targetTypeList == null || targetTypeList.Count <= 0) continue;
            // 获取答案对应的属性和逻辑
            var filterAnswerList = FilterAnswerByDataTypeList(answersTemp, targetTypeList, isIncludeNull);
            // 从合适的答案中随机一个
            var filterResAnswer = filterAnswerList.Count > 0 ? 
                filterAnswerList[Random.Range(0, filterAnswerList.Count)] : 
                null;
            // 如果找到对应的属性, 就设置记录
            if (filterResAnswer != null) {
                if (filterResAnswer.fillTypeList == null || filterResAnswer.fillTypeList.Count <= 0) {
                    res[filterResAnswer] = null;
                } else {
                    validLEListListTemp.RemoveAt(i);
                    answersTemp.Remove(filterResAnswer);
                    res[filterResAnswer] = leList;
                }
            }
        }
        return res;
    }
    public List<Answer> FilterAnswerByDataTypeList(List<Answer> answers, List<DataType> types, bool isIncludeNull = true)
    {
        return answers
            .Where((a)=>(
                isIncludeNull && a.fillTypeList == null) || 
                (a.fillTypeList != null && a.fillTypeList.All(types.Contains) && a.fillTypeList.Count == types.Count))
            .ToList();
    }

    public Card InstantiateTurnCard(int id)
    {
        // 实例化一张卡
        if (!AllCardsIdIndex.ContainsKey(id)) return null;
        var rawCard = AllCardsIdIndex[id];
        var cardInstance = rawCard.ShallowCopy();
        // 处理答案中的填充数值
        var answerLogicList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<List<LogicExecution>>>(DataType.TurnAnswerLogicList);
        var answersToLogicListTable = FilterAnswerByLogicCombiList(cardInstance.answers, answerLogicList);
        var answerNum = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.AnswerNum);
        var chosenAnswerList = GameUtil.RandomRemoveFromList(answersToLogicListTable.Keys.ToList(), answerNum, (answer) => {
            if (answer.fillTypeList == null || answer.fillTypeList.Count <= 0) return answer;
            var logicList = answersToLogicListTable[answer];
            if (logicList == null) return answer;
            answer.logicListFuncList.Add(() => {
                return logicList;
            });
            return answer;
        });
        cardInstance.answers = chosenAnswerList;
        // 处理答案上的数字显示
        foreach (var answer in cardInstance.answers) {
            if (answer.logicListFuncList == null) continue;
            // 区分属性影响逻辑和其他逻辑
            var leGroup = answer.logicListFuncList
                .Select((f)=>new KeyValuePair<Func<List<LogicExecution>>, List<LogicExecution>>(f, f()))
                .SelectMany((k)=>k.Value)
                .GroupBy((le)=>le.Logic)
                .ToDictionary((g)=>g.Key,(g)=>g.ToList());
            // 属性影响逻辑
            var attrList = leGroup
                .Where((k)=>k.Key < Logic._ATTR_MAX_)
                .SelectMany((k)=>(k.Value))
                .Select((le)=>le.ShallowCopy())
                .Select((l)=>{
                    var changeList = (List<AttrInfluence>)l.Param;
                    for (int i = 0; i < changeList.Count; i++) {
                        var change = changeList[i];
                        change = DataInfluenceSystem.I.ConvertFormulaToAttrCopy(change);
                        changeList[i] = change;
                    }
                    return l;
                })
                .ToList();
            // 其他逻辑
            var logicList = leGroup
                .Where((k)=>k.Key > Logic._ATTR_MAX_)
                .SelectMany((k)=>k.Value)
                .Select((le)=>{
                    if (le.Logic == Logic.AddItem) {
                        le = le.ShallowCopy();
                        var param = ((AttrInfluence itemId, AttrInfluence itemNum))le.Param;
                        le.Param = (DataInfluenceSystem.I.ConvertFormulaToAttrCopy(param.itemId), DataInfluenceSystem.I.ConvertFormulaToAttrCopy(param.itemNum));
                    }
                    return le;
                })
                .ToList();
            // 把计算好的逻辑列表再设置回去
            answer.logicListFuncList = new List<Func<List<LogicExecution>>>() {
                () => attrList,
                () => logicList,
            };
            // 显示计算数值
            var attrStr = AttributesLogic.I.BuildAttrString(attrList, answer.showTypeList ?? answer.fillTypeList);
            if (attrStr != null) {
                answer.content += "\n" + attrStr;
            }
            // 显示逻辑表示
            var logicStr = CLS.I.BuildLogicString(logicList);
            if (logicStr != null) {
                answer.content += "\n" + logicStr;
            }
        }
        // 处理看不见
        if (cardInstance.IsMaskable && !ConditionSystem.I.IsConditionMet(cardInstance.SeeCondition)) {
            MaskCard(cardInstance);
        }
        return cardInstance;
    }

}
