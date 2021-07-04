using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;
using System.Linq;

public class DialogSwipeEvent : DialogSwipe, ICardDialog
{
    
    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        
    }

    override protected void OnEnable()
    {
        base.OnEnable();
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();   
    }

    // ICardDialog
    Card card;
    public void SetCard(Card c)
    {
        this.card = c;
        UpdateOptions();
        UpdateContent();
    }

    void UpdateContent()
    {
        var contentView = gameObject.i<TextMeshProUGUI>("Ex_弹窗内容");
        contentView.text = this.card.content;
    }

    bool LEChangeAttr(LogicExecution le)
    {
        return le.Logic == Logic.AttrChange ||
            le.Logic == Logic.AttrChangeCost ||
            le.Logic == Logic.AttrChangeHurt ||
            le.Logic == Logic.AttrChangeHurtIncome ||
            le.Logic == Logic.AttrChangeIncome;
    }

    bool InfluenceToShow(AttrInfluence influ)
    {
        var dataType = influ.AttributeType;
        var isTypeMatch = AttributesLogic.I.DisplayAttrTypes.Any((d) => dataType == d);
        var valueInflu = DataInfluenceSystem.I.ConvertFormulaToAttrCopy(influ);
        var value = valueInflu.Attr.GetValue<float>();
        return isTypeMatch && value > 0;
    }

    void UpdateOptions()
    {
        var answers = card.answers;
        var count = answers.Count;
        var leftHost = gameObject.i<Transform>("Ex_左边数值");
        foreach (Transform child in leftHost.transform) {
            ObjectPoolManager.Instance.RecycleGameObject(child.gameObject);
        }
        while (count > 0) {
            var leftAnswer = answers[0];
            if (leftAnswer.logicListFuncList.Count <= 0) break;
            // 简介
            var leftLabel = gameObject.i<TextMeshProUGUI>("Ex_左边简介");
            leftLabel.text = leftAnswer.content;
            // 数值
            var attrInfluList = leftAnswer.logicListFuncList.SelectMany((leListFunc) => leListFunc()
                .Where((le) => LEChangeAttr(le))
                .SelectMany((le) => (List<AttrInfluence>)le.Param))
                .ToList();
            foreach (var attrInflu in attrInfluList) {
                if (!InfluenceToShow(attrInflu)) continue;
                AddValueView(leftHost, attrInflu);
            }
            break;
        }

        var rightHost = gameObject.i<Transform>("Ex_右边数值");
        foreach (Transform child in rightHost.transform) {
            ObjectPoolManager.Instance.RecycleGameObject(child.gameObject);
        }
        while (count > 1) {
            var rightAnswer = answers[1];
            if (rightAnswer.logicListFuncList.Count <= 0) break;
            // 简介
            var rightLabel = gameObject.i<TextMeshProUGUI>("Ex_右边简介");
            rightLabel.text = rightAnswer.content;
            // 数值
            var attrInfluList = rightAnswer.logicListFuncList.SelectMany((leListFunc) => leListFunc()
                .Where((le) => LEChangeAttr(le))
                .SelectMany((le) => (List<AttrInfluence>)le.Param))
                .ToList();
            foreach (var attrInflu in attrInfluList) {
                if (!InfluenceToShow(attrInflu)) continue;
                AddValueView(rightHost, attrInflu);
            }
            break;
        }
    }

    void AddValueView(Transform host, AttrInfluence attrInfluence)
    {
        var influ = DataInfluenceSystem.I.ConvertFormulaToAttrCopy(attrInfluence);
        var itemView = ObjectPoolManager.Instance.GetGameObject<UIViewBase>(Constants.UIBasePath + Constants.UIValueViewPath);
        itemView.gameObject.SetActive(true);
        itemView.transform.SetParent(host, false);

        var value = influ.Attr.GetValue<float>();
        var valueView = itemView.i<TextMeshProUGUI>("Ex_数值");
        valueView.text = $"{string.Format("{0:F1}", value)}";

        var unitStr = AttributesLogic.I.DataTypeToUnitString(attrInfluence.AttributeType);
        var unitView = itemView.i<TextMeshProUGUI>("Ex_单位");
        unitView.text = unitStr;
    }
}
