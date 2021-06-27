using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;

public class DialogSwipeBlank : DialogSwipe
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
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        UpdateTimePos();
        UpdateTimeLabelVisual();
    }

    protected override void DragEndCallback()
    {
        SnapToWholeNumber();
        UpdateResult();
        base.DragEndCallback();
    }

    void UpdateResult()
    {
        if (dragPosDelta.x < 0) {
            IsWork = false;
        } else {
            IsWork = true;
        }
        Duration = curIndex + 1;
    }

    public int maxIndexCount = 6;
    public int indexCount = 6;
    public int curIndex = 0;
    void UpdateTimePos()
    {
        if (!isDragging) return;
        if (Mathf.Abs(dragPosDelta.x) > 40) {
            SnapToWholeNumber();
            return;
        }
        // 更新滚动位置
        var scroll = gameObject.i<ScrollRect>("Ex_时长滚动");
        var normalizedDeltaY = -dragPosDeltaFrame.y / scroll.content.rect.height;
        scroll.verticalNormalizedPosition += normalizedDeltaY;
        curIndex = NormalYToIdx(scroll.verticalNormalizedPosition);
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
        var curChildIndex = maxIndexCount - curIndex + 1;
        var curHourItem = hourContent.GetChild(curChildIndex);
        UpdateItemText(curHourItem);
        if (curIndex < maxIndexCount) {
            var lessChild = hourContent.GetChild(maxIndexCount - curIndex);
            UpdateItemText(lessChild);
        }
        if (curIndex > 0) {
            var lessChild = hourContent.GetChild(maxIndexCount - curIndex + 2);
            UpdateItemText(lessChild);
        }
    }

    void SnapToWholeNumber()
    {
        var scroll = gameObject.i<ScrollRect>("Ex_时长滚动");
        curIndex = NormalYToIdx(scroll.verticalNormalizedPosition);
        var normVPos = IdxToNormalY(curIndex);
        scroll.verticalNormalizedPosition = normVPos;
    }

    int NormalYToIdx(float normalY)
    {
        float step = (float)1 / (maxIndexCount);
        int index = (int)((normalY + step / 2) / step);
        index = Mathf.Clamp(index, 0, indexCount);
        return index;
    }

    float IdxToNormalY(int idx)
    {
        float step = (float)1 / (maxIndexCount);
        idx = Mathf.Clamp(idx, 0, indexCount);
        return step * idx;
    }

    override protected void OnChoose()
    {
        if (callback == null) return;
        callback();
    }
}
