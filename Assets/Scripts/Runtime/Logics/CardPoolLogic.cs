using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using CS = ConditionSystem;

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
        // 一天的卡牌ID
        DataSystem.I.SetDataByType<List<int>>(DataType.DayCards, null);
        // 初始化质量影响卡牌
        var qualityWeight = new Dictionary<CardQuality, float>(){
            {CardQuality.Red, 15f},
            {CardQuality.White, 30f},
            {CardQuality.Green, 25f},
            {CardQuality.Blue, 15f},
            {CardQuality.Purple, 10f},
            {CardQuality.Gold, 5f},
        };
        DataSystem.I.SetDataByType(DataType.CardQualityWeight, qualityWeight);
        // 初始化属性影响卡牌
        var cardAttrWeight = new Dictionary<CardQuality, List<List<LogicExecution>>>() {
            {CardQuality.Red, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.White, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.Green, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.Blue, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.Purple, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.Gold, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
            {CardQuality.Any, new List<List<LogicExecution>>(){
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-3,0)")),
                CLS.I.GetAttrIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(0,3)")),
                CLS.I.GetAttrHurt(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-3,0)")),
            }},
        };
        DataSystem.I.SetDataByType(DataType.CardAttributeWeight, cardAttrWeight);
        // 初始化幸运影响卡牌
        UpdateCardWeightFromLuck();
        // 幸运变化回调
        DataSystem.I.AddCallback(DataType.Luck, () => {
            UpdateCardWeightFromLuck();
        });
    }

    // 从幸运中获取卡牌质量偏重
    public void UpdateCardWeightFromLuck()
    {
        var luck = DataSystem.I.GetDataByType<float>(DataType.Luck);
        var variWeight = Mathf.Lerp(0, 80, luck/40f);
        Dictionary<LuckQualityGroup, float> luckWeight = new Dictionary<LuckQualityGroup, float>();
        luckWeight[LuckQualityGroup.Low] = 80 - variWeight;
        luckWeight[LuckQualityGroup.High] = 20 + variWeight;
        DataSystem.I.SetDataByType(DataType.CardLuckWeight, luckWeight);
    }

    // 通过卡牌填充条件
    public void ShuffleDayCards()
    {
        var allCardsIdList = CardLogic.I.AllCards
            .Where((c) => ConditionSystem.I.IsConditionMet(c.FillCondition))
            .Select((c) => c.Id)
            .ToList();
        DataSystem.I.SetDataByType(DataType.DayCards, allCardsIdList);
        SyncDayCardsToData();
    }

    // 从数据中初始化日卡牌
    public void SyncDayCardsToData()
    {
        dayCards.Clear();
        var idList = DataSystem.I.GetDataByType<List<int>>(DataType.DayCards);
        if (idList == null) return;
        foreach (var id in idList) {
            var card = CardLogic.I.GetCardById(id);
            if (card == null) continue;
            dayCards.Add(card);
        }
    }

    // 重新从答案列表中随机出
    List<List<LogicExecution>> RerollCardAnswerAttr(CardQuality quality)
    {
        // 计算答案属性类型和数值
        var cardAnswerLogicListList = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<CardQuality, List<List<LogicExecution>>>>(DataType.CardAttributeWeight);
        var cardAnswerLogicList = new List<List<LogicExecution>>(cardAnswerLogicListList[quality]);
        var answerNum = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.AnswerNum);
        var validLogicList = GameUtil.RandomRemoveFromList(cardAnswerLogicList, answerNum);
        DataSystem.I.SetDataByType(DataType.CardAnswerLogicList, validLogicList);
        return validLogicList;
    }

    // 从日卡牌中选取一个列表
    (List<Card>, List<CardQuality>) GetCardListFromWeights()
    {
        // 计算幸运比重
        var luckWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<LuckQualityGroup, float>>(DataType.CardLuckWeight);
        LuckQualityGroup? luckKey = luckWeight != null ? GameUtil.RandomWeightKey(luckWeight) : (LuckQualityGroup?)null;
        // 计算运气比重
        List<CardQuality> qualityList = null;
        if (luckKey == LuckQualityGroup.Low) {
            qualityList = new List<CardQuality>(){CardQuality.Red, CardQuality.White, CardQuality.Green};
        } else if (luckKey == LuckQualityGroup.High) {
            qualityList = new List<CardQuality>(){CardQuality.Blue, CardQuality.Purple, CardQuality.Gold};
        } else if (luckKey == LuckQualityGroup.Any) {
            // 故意留空为null
        }
        // 计算质量比重
        var qualityWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<CardQuality, float>>(DataType.CardQualityWeight);
        Dictionary<CardQuality, float> validQualityWeight = qualityList?
            .Where((q)=>qualityWeight.ContainsKey(q))
            .ToDictionary((q)=>q, (q)=>qualityWeight[q])
            ?? qualityWeight;
        CardQuality randQuality = validQualityWeight != null ? GameUtil.RandomWeightKey(validQualityWeight) : CardQuality.Any;
        // 计算答案属性类型和数值
        var validLogicListList = RerollCardAnswerAttr(randQuality);
        // 计算类型
        var typeFilter = DataSystem.I.CopyAttrDataWithInfluenceByType<List<CardType>>(DataType.CardTypeFilter);
        // 筛选条件
        var validCards = dayCards
            .Where((c) => 
                randQuality == CardQuality.Any || 
                c.Quality == randQuality || 
                c.Quality == CardQuality.Any)                                               // 筛选质量
            .Where((c) => 
                typeFilter == null || 
                typeFilter.Contains(c.Type) || 
                c.Type == CardType.Any)                                                     // 筛选卡牌类型
            .Where((c) => { 
                var answersToTest = c.answers.Where((a)=>a.typeList != null).ToList();
                if (answersToTest.Count <= 0) return true;
                return CardLogic.I.FilterAnswerByDataTypeList(answersToTest, validLogicListList).Any();
            })                                                                              // 筛选答案类型
            .Where((c) => ConditionSystem.I.IsConditionMet(c.DrawCondition))                // 筛选卡牌抽取条件
            .GroupBy((c) => c.DrawPriority)                                                 // 优先度分组
            .OrderBy((g) => g.FirstOrDefault().DrawPriority)                                // 优先度分组排序
            .LastOrDefault()?                                                               // 选优先度最高的组
            .ToList();
        return (validCards, qualityList);
    }

    // 从卡牌列表中选一张卡牌
    Card PickCard()
    {
        // 获取卡牌
        (List<Card> validCards, List<CardQuality> qualityList) = GetCardListFromWeights();
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
        // 选一张卡牌
        Card resCard = null;
        float random = Random.Range(0, cardWeightSum);
        foreach (var card in validCards) {
            var cardWeight = card.baseWeight;
            if (weightTable != null) {
                cardWeight += weightTable.FirstOrDefault((kvp)=>kvp.Key == card.Id).Value;
            }
            random -= cardWeight;
            if (random <= 0) {
                resCard = card;
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
        if (qualityList == null) return;
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
        return CardLogic.I.InstantiateTurnCard(turnCard);
    }

    public Card GetTurnCard()
    {
        return turnCard;
    }
}
