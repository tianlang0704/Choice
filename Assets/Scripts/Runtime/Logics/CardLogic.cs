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
    public List<DataType> typeList; 
    public List<Func<List<LogicExecution>>> logicListFuncList;
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
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, WeatherLogic.I.GetCurrentWeather().baseConsumption, null)
                        });}},
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
                        content = "金钱{0:+#;-#;0}",
                        typeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            // (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,1,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "金钱{0:+#;-#;0}",
                        typeList = new List<DataType>() {DataType.Gold},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            // (Logic.AttrChangeHurt, DIS.I.GetAttrInfluenceList(0,0,0,-1,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "获取护符",
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AddItem, (GameUtil.ItemId(2), 1), null),
                        });}},
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
                        content = "心情{0:+#;-#;0}",
                        typeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            // (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,1,0,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
                        });}},
                    },
                    new Answer() {
                        content = "心情{0:+#;-#;0}",
                        typeList = new List<DataType>() {DataType.Mood},
                        logicListFuncList = new List<Func<List<LogicExecution>>>() {() => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            // (Logic.AttrChangeHurt, DIS.I.GetAttrInfluenceList(0,0,-1,0,0,0,0,0), null),
                            (Logic.AttrChangeIncome, DIS.I.GetAttrInfluenceList(0,0,0,0,0,0,1,0), null),
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

    public void MaskCard(Card c)
    {
        c.content = "???";
        var oriAnswers = c.answers;
        for (int i = 0; i < oriAnswers.Count; i++) {
            var a = oriAnswers[i];
            a.content = "????";
        }
    }
    public Dictionary<Answer, List<LogicExecution>> FilterAnswerByDataTypeList(List<Answer> answers, List<List<LogicExecution>> leListList)
    {
        // 从逻辑列表中获取所有属性改变逻辑
        var validLEListList = leListList.Where((lewList)=>lewList.Where((le)=>le.Logic < Logic._ATTR_MAX_).Any()).ToList();
        var validLEListListTemp = new List<List<LogicExecution>>(validLEListList);
        var answersTemp = new List<Answer>(answers);
        var res = new Dictionary<Answer, List<LogicExecution>>();
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
            var filterResList = FilterAnswerByDataTypeList(answersTemp, targetTypeList);
            // 从合适的答案中随机一个
            var filterRes = filterResList.Count > 0 ? 
                filterResList[Random.Range(0, filterResList.Count)] : 
                null;
            // 如果找到对应的属性, 就设置记录
            if (filterRes != null) {
                validLEListListTemp.RemoveAt(i);
                answersTemp.Remove(filterRes);
                res[filterRes] = leList;
            }
        }
        return res;
    }
    public List<Answer> FilterAnswerByDataTypeList(List<Answer> answers, List<DataType> types)
    {
        return answers.Where((a)=>a.typeList == null || a.typeList.All(types.Contains) && a.typeList.Count == types.Count).ToList();
    }

    public Card InstantiateTurnCard(Card rawCard)
    {
        if (rawCard == null) return null;
        var cardInstance = rawCard.ShallowCopy();
        
        // 处理答案和数值
        var answerLogicList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<List<LogicExecution>>>(DataType.CardAnswerLogicList);
        var answersToLogicListTable = FilterAnswerByDataTypeList(cardInstance.answers, answerLogicList);
        var answerNum = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.AnswerNum);
        var chosenAnswerList = GameUtil.RandomRemoveFromList(answersToLogicListTable.Keys.ToList(), answerNum, (answer) => {
            var logicList = answersToLogicListTable[answer];
            answer.logicListFuncList.Add(() => {
                return logicList;
            });
            return answer;
        });
        cardInstance.answers = chosenAnswerList;

        // 处理答案上的数字显示
        foreach (var answer in cardInstance.answers) {
            if (answer.typeList == null || answer.typeList.Count <= 0) continue;
            // if (!answer.content.Contains("%v")) continue;
            if (answer.logicListFuncList == null) continue;
            // 如果要显示就要在这里做计算
            var attrLogicListList = answer.logicListFuncList
                .Select((f)=>f())
                .Where((list)=>list.Any((le)=>le.Logic < Logic._ATTR_MAX_))
                .ToList();
            var attrInfluList = attrLogicListList
                .SelectMany((l)=>l)
                .SelectMany((l)=>(List<AttrInfluence>)l.Param)
                .Select((i)=>{
                    DataInfluenceSystem.I.ConvertFormulaToAttr(i);
                    i.Formula = null;
                    return i;
                })
                .ToList();
            // 显示计算数值
            var valueList = answer.typeList
                .Select((t)=>{
                    var typeInflueList = attrInfluList.Where((i)=>i.AttributeType == t).ToList();
                    var resAttr = new Attr();
                    DIS.I.ApplyInfluenceList(resAttr, typeInflueList);
                    return (object)resAttr.GetValue<float>();
                }).ToList();
            answer.content = string.Format(answer.content, valueList.ToArray());
        }

        // 处理看不见
        if (cardInstance.IsMaskable && !ConditionSystem.I.IsConditionMet(cardInstance.SeeCondition)) {
            MaskCard(cardInstance);
        }
        return cardInstance;
    }
}
