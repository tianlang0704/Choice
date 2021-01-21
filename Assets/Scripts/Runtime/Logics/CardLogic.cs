using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using CS = ConditionSystem;
using LEList = System.Collections.Generic.List<(Logic, object, Condition)>;


public enum CardType { Any = -1, Basic = 0, Trap, Item, Rest, Event, SceneChange }
public enum CardQuality { Any = -1, Red = 0, White, Green, Blue, Purple, Gold }
public enum LuckQualityGroup { Any = -1, Low = 0, High }
public class Card 
{
    public int Id;
    public int DrawPriority = 0;
    public bool IsMaskable = true;
    public CardType Type = CardType.Basic;
    public CardQuality Quality = CardQuality.White;
    public Condition FillCondition;
    public Condition DrawCondition;
    public Condition SeeCondition = new Condition() { Formula = $"IsHaveItem({GameUtil.ItemId(10001)}) == 0" };
    public string content = "";
    public float baseWeight = 1;
    public List<Answer> answers;
    public Card ShallowCopy()
    {
        Card res = (Card)this.MemberwiseClone();
        var oriAnswers = res.answers;
        res.answers = new List<Answer>();
        for (int i = 0; i < oriAnswers.Count; i++) {
            var a = oriAnswers[i].ShallowCopy();
            res.answers.Add(a);
        }
        return res;
    }
}

public class Answer
{
    public string content;
    public Condition condition;
    public Func<List<LogicExecution>> logicListFunc;
    public Answer ShallowCopy()
    {
        return (Answer)this.MemberwiseClone();
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
                Quality = CardQuality.Any,
                DrawPriority = 1,
                IsMaskable = false,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                DrawCondition = new Condition() {Formula = "CurrentTurn >= 7"},
                content = "今天结束了, 请休息",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "休息一下",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, WeatherLogic.I.GetCurrentWeather().baseConsumption, null)
                        });},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(10002),
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
                Id = GameUtil.CardId(1),
                Quality = CardQuality.Any,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "金钱+-",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "金钱+",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,1,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "金钱-",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeHurt, DIS.I.GetAttrInfluenceList(0,0,0,-1,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "获取护符",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AddItem, (GameUtil.ItemId(2), 1), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = GameUtil.CardId(2),
                Quality = CardQuality.Any,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "心情+-",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "心情+",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,1,0,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "心情-",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChangeHurt, DIS.I.GetAttrInfluenceList(0,0,-1,0,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });},
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

    public void MaskCard(Card c)
    {
        c.content = "???";
        var oriAnswers = c.answers;
        for (int i = 0; i < oriAnswers.Count; i++) {
            var a = oriAnswers[i];
            a.content = "????";
        }
    }
}
