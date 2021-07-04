using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;

interface ICardDialog {
    void SetCard(Card c);
}

public class DialogSwipeBlank : DialogSwipe, ICardDialog
{
    public bool IsWork = false;
    public int Duration = 0;
    Transform scrollRect;
    // Start is called before the first frame update
    override protected void Awake()
    {
        scrollRect = gameObject.i<Transform>("Ex_时长滚动");
        base.Awake();
    }

    void Start()
    {
        
    }

    override protected void OnEnable()
    {
        base.OnEnable();

        UpdateIndexCount();
    }

    void UpdateIndexCount()
    {
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        var maxTurnInDay = DataSystem.I.GetDataByType<int>(DataType.DayMaxTurn);
        var turnInDay = curTurn % maxTurnInDay;
        var curIndex = turnInDay;
        IndexCount = MaxIndexCount - curIndex;
        IndexCount = Mathf.Clamp(IndexCount, 0, MaxIndexCount);

        var hourContent = gameObject.i<Transform>("Ex_Content");
        for (int i = 0; i <= curIndex; i++) {
            var curItem = hourContent.GetChild(i);
            var tmpGUI = curItem.gameObject.i<TextMeshProUGUI>("Ex_文字");
            tmpGUI.enabled = false;
        }
    }

    override protected void OnDisable()
    {
        base.OnDisable();
    }

    protected override void DragEndCallback()
    {
        SnapToWholeNumber();
        UpdateResult();
        base.DragEndCallback();
    }

    void UpdateResult()
    {
        if (dragger.dragPosDelta.x < 0) {
            IsWork = false;
        } else {
            IsWork = true;
        }
        Duration = CurIndex + 1;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        UpdateTimePos();
        UpdateTimeLabelVisual();
        UpdateCardData();
    }

    void UpdateCardData()
    {
        if (this.card == null) {
            return;
        }
        
        // 工作
        var work = this.card.answers[0];
        var workLogicList = work.logicListFuncList[0]();
        var workProfit = GetValueFromLogic(workLogicList[0]) * CurFactor;
        var workProfitView = gameObject.i<UIViewBase>("Ex_打工收益");
        var workProfitValueView = workProfitView.i<TextMeshProUGUI>("Ex_数值");
        workProfitValueView.text = $"{string.Format("{0:F1}", workProfit)}";
        var workCost = GetValueFromLogic(workLogicList[1]) * CurFactor;
        var workCostView = gameObject.i<UIViewBase>("Ex_打工消耗");
        var workCostValueView = workCostView.i<TextMeshProUGUI>("Ex_数值");
        workCostValueView.text = $"{string.Format("{0:F1}", workCost)}";

        // 跑路
        var run = this.card.answers[1];
        var runLogicList = run.logicListFuncList[0]();
        var runProfit = GetValueFromLogic(runLogicList[0]) * CurFactor;
        var runProfitView = gameObject.i<UIViewBase>("Ex_赶路收益");
        var runProfitValueView = runProfitView.i<TextMeshProUGUI>("Ex_数值");
        runProfitValueView.text = $"{string.Format("{0:F1}", runProfit)}";
        var runCost = GetValueFromLogic(runLogicList[1]) * CurFactor;
        var runCostView = gameObject.i<UIViewBase>("Ex_赶路消耗");
        var runCostValueView = runCostView.i<TextMeshProUGUI>("Ex_数值");
        runCostValueView.text = $"{string.Format("{0:F1}", runCost)}";
    }

    float GetValueFromLogic(LogicExecution le)
    {
        if (le.Logic != Logic.AttrChange) return 0;
        var param = le.Param;
        var influenceList = param as List<AttrInfluence>;
        var influ = influenceList[0];
        var valueInflu = DataInfluenceSystem.I.ConvertFormulaToAttrCopy(influ);
        var value = valueInflu.Attr.GetValue<float>();
        return value;
    }

    [NonSerialized]
    public int MaxIndexCount = 6;
    [NonSerialized]
    public int IndexCount = 6;
    [NonSerialized]
    public int CurIndex = 0;
    [NonSerialized]
    public float CurFactor = 1f;
    void UpdateTimePos()
    {
        if (!dragger.isDragging) return;
        if (Mathf.Abs(dragger.dragPosDelta.x) > 40) {
            SnapToWholeNumber();
            return;
        }
        // 更新滚动位置
        var scroll = gameObject.i<ScrollRect>("Ex_时长滚动");
        var normalizedDeltaY = -dragger.dragPosDeltaFrame.y / scroll.content.rect.height;
        scroll.verticalNormalizedPosition += normalizedDeltaY;
        CurIndex = NormalYToIdx(scroll.verticalNormalizedPosition);
        // 更新倍率
        CurFactor = (float)(1f + (float)CurIndex * 0.3);
    }

    int curVisualIndex = -1;
    Color selectedColor = new Color(0.6862745f, 0.6901961f, 0.6941177f, 1f);
    Color outsideColor = new Color(0.6862745f, 0.6901961f, 0.6941177f, 0.5f);
    Vector3 selectedScale = new Vector3(1.42f, 1.42f, 1.42f);

    Vector3 outsideScale = Vector3.one;
    void UpdateItemText(Transform curItem)
    {
        var curPos = scrollRect.InverseTransformPoint(curItem.position);
        var absDelta = Mathf.Abs(curPos.y);
        var progress = absDelta / 100;
        var curScale = Vector3.Lerp(selectedScale, outsideScale, progress);
        curItem.localScale = curScale;
        var curColor = Color.Lerp(selectedColor, outsideColor, progress);
        var tmpGUI = curItem.gameObject.i<TextMeshProUGUI>("Ex_文字");
        tmpGUI.color = curColor;
    }
    void UpdateTimeLabelVisual()
    {
        var hourContent = gameObject.i<Transform>("Ex_Content");
        var curChildIndex = MaxIndexCount - CurIndex + 1;
        var curHourItem = hourContent.GetChild(curChildIndex);
        UpdateItemText(curHourItem);
        if (CurIndex < MaxIndexCount) {
            var lessChild = hourContent.GetChild(MaxIndexCount - CurIndex);
            UpdateItemText(lessChild);
        }
        if (CurIndex > 0) {
            var lessChild = hourContent.GetChild(MaxIndexCount - CurIndex + 2);
            UpdateItemText(lessChild);
        }
    }

    void SnapToWholeNumber()
    {
        var scroll = gameObject.i<ScrollRect>("Ex_时长滚动");
        CurIndex = NormalYToIdx(scroll.verticalNormalizedPosition);
        var normVPos = IdxToNormalY(CurIndex);
        scroll.verticalNormalizedPosition = normVPos;
    }

    int NormalYToIdx(float normalY)
    {
        float step = (float)1 / (MaxIndexCount);
        int index = (int)((normalY + step / 2) / step);
        index = Mathf.Clamp(index, 0, IndexCount);
        return index;
    }

    float IdxToNormalY(int idx)
    {
        float step = (float)1 / (MaxIndexCount);
        idx = Mathf.Clamp(idx, 0, IndexCount);
        return step * idx;
    }

    override protected void OnChoose()
    {
        if (callback == null) return;
        callback();
    }

    // ICardDialog
    Card card;
    public void SetCard(Card c)
    {
        this.card = c;
    }
}
