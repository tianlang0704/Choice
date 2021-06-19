using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DIS = DataInfluenceSystem;
using CLS = CommonLogicSystem;
using CS = ConditionSystem;
using System;

public class CardPoolLogic : SingletonBehaviour<CardPoolLogic>
{
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
    }

    public void UpdateCardType()
    {
        // 判断开始卡牌
        var sceneMap = DataSystem.I.CopyAttrDataWithInfluenceByType<List<CardType>>(DataType.SceneMap);
        if (sceneMap == null) {
            DataSystem.I.SetDataByType<int>(DataType.TurnCardType, (int)CardType.None);
            return;
        }
        var distance = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.Distance);
        if (distance < 0 || distance >= sceneMap.Count) {
            DataSystem.I.SetDataByType<int>(DataType.TurnCardType, (int)CardType.None);
            return;
        }
        var cardType = sceneMap[distance];
        DataSystem.I.SetDataByType<int>(DataType.TurnCardType, (int)cardType);
    }

    public void RerollTurnCard()
    {
        // 获取卡片类型
        var cardType = (CardType)DataSystem.I.GetDataByType<int>(DataType.TurnCardType);
        // 处理空卡
        if (cardType == CardType.Blank) {
            DataSystem.I.SetDataByType(DataType.TurnCardId, 0);
            return;
        }
        // 根据卡片类型获取卡牌列表
        var cardList = CardLogic.I.GetCardListByType(cardType);
        if (cardList.Count > 0) {
            cardList = cardList.Where((c) => ConditionSystem.I.IsConditionMet(c.FillCondition)).ToList();
        }
        // 处理空卡
        if (cardList.Count <= 0) {
            DataSystem.I.SetDataByType(DataType.TurnCardId, 0);
            Debug.LogWarning($"今天没抽到卡, 卡牌类型: {cardType.ToString()}");
            return;
        }
        // 理出符合抽取条件的卡
        // 抽卡
        var randomCard = GameUtil.RandomRemoveFromList(cardList);
        // 保存抽到的卡
        DataSystem.I.SetDataByType(DataType.TurnCardQuality, randomCard.Quality);
        DataSystem.I.SetDataByType(DataType.TurnCardId, randomCard.Id);
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
