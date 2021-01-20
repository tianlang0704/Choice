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
    static string CardQualityInfluenceIdentifier = "CardQualityInfluenceIdentifier";
    List<Card> dayCards = new List<Card>();

    void Awake()
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

    public void Init()
    {
        DataSystem.I.SetDataByType<List<int>>(DataType.DayCards, null);
    }

    public void ShuffleDayCards()
    {
        var allCardsIdList = CardLogic.I.AllCards
            .Where((c) => ConditionSystem.I.IsConditionMet(c.FillCondition))
            .Select((c) => c.Id)
            .ToList();
        DataSystem.I.SetDataByType(DataType.DayCards, allCardsIdList);
        SyncDayCardsToData();
    }

    public void SyncDayCardsToData()
    {
        dayCards.Clear();
        var idList = DataSystem.I.GetAttrDataByType<List<int>>(DataType.DayCards);
        if (idList == null) return;
        foreach (var id in idList)
        {
            dayCards.Add(CardLogic.I.AllCardsIdIndex[id]);
        }
    }

    public Card GetCardById(int id)
    {
        if (!CardLogic.I.AllCardsIdIndex.ContainsKey(id)) return null;
        return CardLogic.I.AllCardsIdIndex[id];
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
        var validQualityWeight = qualityList.Where((q)=>qualityWeight.ContainsKey(q)).ToDictionary((q)=>q, (q)=>qualityWeight[q]);
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
            Identifier = CardQualityInfluenceIdentifier,
            AttributeType = DataType.CardQualityWeight,
            Attr = qualityInfAttr,
            DurFre = new DurationAndFrequency() {
                // 每回合按比例慢慢加回来
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
                // 直到所有值都为0为止
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

    // 重置卡牌质量比重变化
    public void ResetQualityWeight()
    {
        DataInfluenceSystem.I.RemoveInfluence(CardQualityInfluenceIdentifier);
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
        // 处理答案数量
        var answerNumOffset = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.AnswerNumOffset);
        var offsetAbs = Mathf.Abs(answerNumOffset);
        var answers = turnCard.answers;
        if (answers.Count >= offsetAbs + 1) {
            var index = answers.Count - offsetAbs - 1;
            answers.RemoveRange(index, offsetAbs);
        }
        return turnCard;
    }

    public Card GetTurnCard()
    {
        return turnCard;
    }
}
