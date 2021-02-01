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
        DataSystem.I.SetDataByType(DataType.TurnCardQualityWeight, qualityWeight);
        // 初始化属性影响卡牌
        var answerLogicWeight = new Dictionary<CardQuality, List<List<LogicExecution>>>() {
            {CardQuality.Red, new List<List<LogicExecution>>(){
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(-1,3)")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(-3,6)")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "RandomInt(-2,1)")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(-5,-1)")),
            }},
            {CardQuality.White, new List<List<LogicExecution>>(){
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.Stamina, "RandomInt(-5,-2)"),
                    (DataType.Distance, "RandomInt(5,11)"),
                })),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.Stamina, "-3"),
                    (DataType.Mood, "1"),
                })),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.TempRandom1,"RandomInt(1,5)"),
                    (DataType.Stamina, "TempRandom1-4"),
                    (DataType.Gold, "TempRandom1+1"),
                })),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.Gold, "RandomInt(-6,-1)"),
                    (DataType.HP, "RandomInt(1,4)"),
                })),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.Gold, "RandomInt(-5,-1)"),
                    (DataType.Stamina, "RandomInt(3,9)"),
                })),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(new List<(DataType type, string formula)>(){
                    (DataType.Gold, "RandomInt(-6,-2)"),
                    (DataType.Mood, "RandomInt(1,3)"),
                }))
            }},
            {CardQuality.Green, new List<List<LogicExecution>>(){
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.HP, "RandomInt(1,4)")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Stamina, "RandomInt(2,7)")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Mood, "1")),
                CLS.I.GetAttrHurtIncome(DIS.I.GetAttrInfluenceList(DataType.Gold, "RandomInt(1,6)")),
            }},
            {CardQuality.Blue, new List<List<LogicExecution>>(){}},
            {CardQuality.Purple, new List<List<LogicExecution>>(){}},
            {CardQuality.Gold, new List<List<LogicExecution>>(){}},
            {CardQuality.Any, new List<List<LogicExecution>>(){}},
        };
        DataSystem.I.SetDataByType(DataType.TurnAnswerLogicWeight, answerLogicWeight);
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
        DataSystem.I.SetDataByType(DataType.TurnCardLuckWeight, luckWeight);
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

    // 从幸运中随机选一个质量列表
    void RerollQualityList()
    {
        var luckWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<LuckQualityGroup, float>>(DataType.TurnCardLuckWeight);
        LuckQualityGroup? luckKey = luckWeight != null ? GameUtil.RandomWeightKey(luckWeight) : (LuckQualityGroup?)null;
        List<CardQuality> qualityList = null;
        if (luckKey == LuckQualityGroup.Low) {
            qualityList = new List<CardQuality>(){CardQuality.Red, CardQuality.White, CardQuality.Green};
        } else if (luckKey == LuckQualityGroup.High) {
            qualityList = new List<CardQuality>(){CardQuality.Blue, CardQuality.Purple, CardQuality.Gold};
        } else if (luckKey == LuckQualityGroup.Any) {
            // 故意留空为null
        }
        DataSystem.I.SetDataByType(DataType.TurnCardQualityList, qualityList);
    }

    // 从质量偏重中随机出一个质量
    void RerollCardQuality()
    {
        var qualityList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<CardQuality>>(DataType.TurnCardQualityList);
        var qualityWeight = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<CardQuality, float>>(DataType.TurnCardQualityWeight);
        Dictionary<CardQuality, float> validQualityWeight = qualityList?
            .Where((q)=>qualityWeight.ContainsKey(q))
            .ToDictionary((q)=>q, (q)=>qualityWeight[q])
            ?? qualityWeight;
        CardQuality randQuality = validQualityWeight != null ? GameUtil.RandomWeightKey(validQualityWeight) : CardQuality.Any;
        DataSystem.I.SetDataByType(DataType.TurnCardQuality, randQuality);
    }

    // 从质量列表中随机出符合质量的答案填充逻辑
    void RerollCardAnswerLogic()
    {
        // 计算答案属性类型和数值
        var quality = DataSystem.I.CopyAttrDataWithInfluenceByType<CardQuality>(DataType.TurnCardQuality);
        var cardAnswerLogicListList = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<CardQuality, List<List<LogicExecution>>>>(DataType.TurnAnswerLogicWeight);
        var cardAnswerLogicList = new List<List<LogicExecution>>(cardAnswerLogicListList[quality]);
        var answerNum = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.AnswerNum);
        var validLogicCombiList = GameUtil.RandomRemoveFromList(cardAnswerLogicList, answerNum);
        DataSystem.I.SetDataByType(DataType.TurnAnswerLogicList, validLogicCombiList);
    }

    // 从日卡牌中选取一个列表
    void RerollValidCardList()
    {
        // 获取回合质量
        var quality = DataSystem.I.CopyAttrDataWithInfluenceByType<CardQuality>(DataType.TurnCardQuality);
        // 获取回合填充逻辑列表
        var validLogicCombiList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<List<LogicExecution>>>(DataType.TurnAnswerLogicList);
        // 获取类型过滤
        var typeFilter = DataSystem.I.CopyAttrDataWithInfluenceByType<List<CardType>>(DataType.TurnCardTypeFilter);
        // 筛选条件
        var validCardIdList = dayCards
            .Where((c) => 
                quality == CardQuality.Any || 
                c.Quality == quality || 
                c.Quality == CardQuality.Any)                                               // 筛选质量
            .Where((c) => 
                typeFilter == null || 
                typeFilter.Contains(c.Type) || 
                c.Type == CardType.Any)                                                     // 筛选卡牌类型
            .Where((c) =>
                c.DrawPriority > 10000 ||
                CardLogic.I.FilterAnswerByLogicCombiList(c.answers, validLogicCombiList, false).Any()) // 筛选答案类型
            .Where((c) => ConditionSystem.I.IsConditionMet(c.DrawCondition))                // 筛选卡牌抽取条件
            .GroupBy((c) => c.DrawPriority)                                                 // 优先度分组
            .OrderBy((g) => g.Key)                                                          // 优先度分组排序
            .LastOrDefault()?                                                               // 选优先度最高的组
            .Select((c)=>c.Id)
            .ToList();
        DataSystem.I.SetDataByType(DataType.TurnCardValidIdList, validCardIdList);
    }

    // 从卡牌列表中选一张卡牌
    void PickTurnCard()
    {
        // 获取卡牌
        var validCardIdList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<int>>(DataType.TurnCardValidIdList);
        if (validCardIdList == null || validCardIdList.Count <= 0) {
            DataSystem.I.SetDataByType(DataType.TurnCardId, 0);
            return;
        }
        var validCards = validCardIdList.Select((id)=>CardLogic.I.GetCardById(id)).ToList();
        // 计算单张卡比重
        float cardWeightSum = 0;
        var weightTable = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<int, float>>(DataType.TurnCardWeight);
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
        UpdateQualityWeight(resCard.Quality);
        // 设置回合卡ID
        DataSystem.I.SetDataByType(DataType.TurnCardId, resCard == null ? 0 : resCard.Id);
    }

    // 更改质量比重
    void UpdateQualityWeight(CardQuality pickQuality)
    {
        var qualityList = DataSystem.I.CopyAttrDataWithInfluenceByType<List<CardQuality>>(DataType.TurnCardQualityList);
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
            AttributeType = DataType.TurnCardQualityWeight,
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

    public void RerollTurnCard()
    {
        // 从幸运计算可用质量列表
        RerollQualityList();
        // 从质量偏重中随机出一个质量
        RerollCardQuality();
        // 从质量逻辑列表中随机出符合质量的答案填充逻辑
        RerollCardAnswerLogic();
        // 从条件中选取可用卡牌
        RerollValidCardList();
        // 选本局的卡
        PickTurnCard();
    }

    public Card GetTurnCardRaw()
    {
        var cardId = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.TurnCardId);
        return CardLogic.I.GetCardById(cardId);
    }

    public Card GetTurnCardInstance()
    {
        var cardId = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.TurnCardId);
        return CardLogic.I.InstantiateTurnCard(cardId);
    }
}
