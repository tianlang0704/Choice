using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using CS = ConditionSystem;
using LEList = System.Collections.Generic.List<(Logic, object, Condition)>;

public class CardPoolLogic : SingletonBehaviour<CardPoolLogic>
{
    List<Card> allCards;
    Dictionary<int, Card> allCardsIdIndex = new Dictionary<int, Card>();

    List<Card> dayCards = new List<Card>();

    void Awake()
    {
        allCards = new List<Card>(){
            new Card() {
                Id = 10001,
                Quality = CardQuality.Special,
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
                Id = 10002,
                Quality = CardQuality.Special,
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
                Id = 1,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "炎炎夏日，路中间趴着一条黑狗，抬头望着你.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "直接走过去. -1生命, 20%几率狂犬病(接下来2回合不可看到答案).",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(-1,0,0,0,0,0,1,0), CS.I.GetCondition("HP > 1")),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 2, 100), null),
                            (Logic.AddItem, (6, 1, 0.2f), null),
                        });},
                    },
                    new Answer() {
                        content = "一口气冲过去. -3体力, +5行程.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,-3f,0,0,0,0,5,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 2, 100), null),
                        });},
                    },
                    new Answer() {
                        content = "过去摸摸它. -1心情.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,-1,0,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 3, 100), null),
                        });},
                    },
                    new Answer() {
                        content = "食物, 喂给他吃.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,0,0,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 3, 100), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = 2,
                baseWeight = 0,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "这只狗子, 在后面一直跟着你.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "把它赶走. -2生命, 50%狂犬病(接下来2回合不可看到答案).",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(-2,0,0,0,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 4, 100), null),
                        });},
                    },
                    new Answer() {
                        content = "接着跑, 一定要甩开它! -5体力, +10里程.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,-5,0,0,0,0,11,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 4, 100), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = 3,
                baseWeight = 0,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "这只狗子, 在后面一直跟着你.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "买点吃的给它吧. -3金币.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,0,-3,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 4, 100), null),
                        });},
                    },
                    new Answer() {
                        content = "过去摸摸它. +2心情",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,2,0,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 4, 100), null),
                        });},
                    },
                    new Answer() {
                        content = "食物, 喂给她吃. +3心情",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,3,0,0,0,1,0), null),
                            (Logic.AttrInfluence, AIS.I.GetCardWeightInfluence(1, 4, 100), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = 4,
                baseWeight = 0,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "黑狗吊着一袋不知道什么东西, 向你跑了过来.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "药包(+2生命)",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(2,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "钱袋子(+5金币)",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,0,5,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "大朗烧饼(+4体力)",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,4,0,0,0,0,1,0), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = 5,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路边有个凉茶店.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "坐下休息一下, +2体力. 被老板骂走, -1心情.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,2,-1,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "来碗茶歇歇脚. -2金币, +3体力.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,3,0,-2,0,0,1,0), null),
                        });},
                    },
                }
            },
            new Card() {
                Id = 6,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "你走着走着, 发现有个野孩子, 一直跟着你.",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "捡起木棍吓跑他. -2体力",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,-2,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "加速甩开他. -4体力.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,-4,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "可怜的孩子, 去买点吃的吧. -3金币.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,0,-3,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "不理他.",
                    },
                    new Answer() {
                        condition = new Condition() { Formula = "IsHaveItem(1)" },
                        content = "烤肉: 喂给他吃. ",
                    },
                    new Answer() {
                        condition = new Condition() { Formula = "IsHaveItem(7)" },
                        content = "衣物: 这个给你吧. ",
                    },
                }
            },
            new Card() {
                Id = 7,
                FillCondition = new Condition() {Formula = "Scene == 1"},
                content = "路过一个村庄, 一伙刁民将你拦下. ",
                answers = new List<Answer>() {
                    new Answer() {
                        content = "随意给一个破烂, 心情-1.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,-1,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "把玉坠给他们, 生命+2.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(2,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "无视他们, 被追着打了一顿, 生命-1.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(-1,0,0,0,0,0,1,0), null),
                        });},
                    },
                    new Answer() {
                        content = "拼命逃跑, 逃过一劫, 心情+1.",
                        logicListFunc = () => { return CLS.I.GetLogicList(new List<(Logic, object, Condition)>() {
                            (Logic.AttrChange, AIS.I.GetAttrInfluences(0,0,1,0,0,0,1,0), null),
                        });},
                    },
                }
            },
        };
        foreach (var card in allCards) {
            allCardsIdIndex[card.Id] = card;
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
        DataSystem.I.SetAttrDataByType<List<int>>(DataType.DayCards, null);
    }

    public void ShuffleDayCards()
    {
        var allCardsIdList = allCards
                                .Where((c) => ConditionSystem.I.IsConditionMet(c.FillCondition))
                                .Select((c) => c.Id)
                                .ToList();
        DataSystem.I.SetAttrDataByType(DataType.DayCards, allCardsIdList);
        SyncDayCardsToData();
    }

    public void SyncDayCardsToData()
    {
        dayCards.Clear();
        var idList = DataSystem.I.GetAttrDataByType<List<int>>(DataType.DayCards);
        if (idList == null) return;
        foreach (var id in idList)
        {
            dayCards.Add(allCardsIdIndex[id]);
        }
    }

    public Card GetCardById(int id)
    {
        if (!allCardsIdIndex.ContainsKey(id)) return null;
        return allCardsIdIndex[id];
    }

    Card PickCard()
    {
        // 计算幸运比重
        var luckWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<int, float>>(DataType.CardLuckWeight);
        int luckKey = GameUtil.RandomWeightKey(luckWeight);
        // 计算质量比重
        List<CardQuality> qualityList;
        if (luckKey == 0) {
            qualityList = new List<CardQuality>(){CardQuality.Red, CardQuality.White, CardQuality.Green};
        } else {
            qualityList = new List<CardQuality>(){CardQuality.Blue, CardQuality.Purple, CardQuality.Gold};
        }
        var qualityWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<CardQuality, float>>(DataType.CardQualityWeight);
        var validQualityWeight = qualityList.ToDictionary((q)=>q, (q)=>qualityWeight[q]);
        var randQuality = GameUtil.RandomWeightKey(validQualityWeight);
        // 筛选条件
        var validCards = dayCards
            .Where((c) => c.Quality == randQuality || c.Quality == CardQuality.Special)     // 筛选质量
            .Where((c) => ConditionSystem.I.IsConditionMet(c.DrawCondition))                // 筛选满足条件的卡
            .GroupBy((c) => c.DrawPriority)                                                 // 优先度分组
            .OrderBy((g) => g.FirstOrDefault().DrawPriority)                                // 优先度分组排序
            .LastOrDefault()                                                                // 选优先度最高的组
            ?.ToList();
        if (validCards == null || validCards.Count <= 0) return null;
        // 计算单张卡比重
        float cardWeightSum = 0;
        var weightTable = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<int, float>>(DataType.CardWeight);
        validCards.ForEach((card) => {
            cardWeightSum += card.baseWeight;
            if (weightTable != null) {
                cardWeightSum += weightTable.FirstOrDefault((kvp)=>kvp.Key == card.Id).Value;
            }
        });
        Card resCard = null;
        float random = Random.Range(0, cardWeightSum);
        foreach (var card in validCards) {
            var cardWeight = card.baseWeight;
            if (weightTable != null) {
                cardWeight += weightTable.FirstOrDefault((kvp)=>kvp.Key == card.Id).Value;
            }
            random -= cardWeight;
            if (random <= 0) {
                resCard = card.ShallowCopy();
                break;
            }
        }
        // 更改质量比重
        UpdateQualityWeight(resCard.Quality, qualityList);
        return resCard;
    }

    // 更改质量比重
    void UpdateQualityWeight(CardQuality pickQuality, List<CardQuality> qualityList)
    {
        // 计算质量比重变化
        float minus = -5f;
        float plus = -1 * minus / (qualityList.Count - 1);
        var qualityInfValue = new Dictionary<CardQuality, float>();
        foreach (var quality in qualityList) {
            if (quality == pickQuality) {
                qualityInfValue[quality] = minus;
            }else{
                qualityInfValue[quality] = plus;
            }
        }
        // 创建一个新的影响
        var qualityInfAttr = new Attr();
        qualityInfAttr.SetValue(qualityInfValue);
        var qualityInf = new AttrInfluence() {
            AttributeType = DataType.CardQualityWeight,
            Attr = qualityInfAttr,
            DurFre = new DurationAndFrequency() {
                UpdateCallback = (updateType) => {
                    if (updateType == DruFreUpdateType.Turn) {
                        var keyList = qualityInfValue.Keys.ToList();
                        foreach (var key in keyList) {
                            var value = qualityInfValue[key];
                            var maxValue = 99999f;
                            var reduceAmount = 0f;
                            if (pickQuality == key) {
                                reduceAmount = -1;
                            } else {
                                reduceAmount = -1 * Mathf.Abs(plus/minus);
                            }
                            var sign = Mathf.Sign(value);
                            var newValue = value + reduceAmount * sign;
                            var clampedValue = Mathf.Sign(newValue) * Mathf.Clamp(Mathf.Abs(newValue), 0, maxValue);
                            qualityInfValue[key] = clampedValue;
                        }
                    }
                },
                Predicate = () => {
                    bool res = true;
                    foreach (var kvp in qualityInfValue) {
                        if (kvp.Value != 0f) {
                            res = false;
                            break;
                        }
                    }
                    return res;
                }
            }
        };
        DataInfluenceSystem.I.AddInfluence(qualityInf);
    }

    Card turnCard;
    public Card RerollTurnCard()
    {
        // 选本局的卡
        turnCard = PickCard();
        if (turnCard == null) return null;
        // 处理看不见
        if (turnCard.IsMaskable && !ConditionSystem.I.IsConditionMet(turnCard.SeeCondition)) {
            CardLogic.I.MaskCard(turnCard);
        }
        return turnCard;
    }

    public Card GetTurnCard()
    {
        return turnCard;
    }
}
