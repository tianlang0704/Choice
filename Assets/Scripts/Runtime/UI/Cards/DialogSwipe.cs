using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;

public class DialogSwipe : UILogicBase<DialogSwipe>
{
    protected Dragger dragger = new Dragger();
    protected Action callback = null;
    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
        left = gameObject.i<PlayableDirector>("Ex_左滑");
        right = gameObject.i<PlayableDirector>("Ex_右滑");
    }
    void Start()
    {

    }

    // Update is called once per frame
    virtual protected void Update()
    {
        dragger.UpdateDrag();
        UpdateDragChoose();
        UpdateReset();
    }

    virtual protected void OnEnable()
    {
        dragger.dragEndHandler += DragEndCallback;
        ResetDialog();
    }

    virtual protected void OnDisable()
    {
        dragger.dragEndHandler -= DragEndCallback;
    }

    virtual public void SetCallback(Action a)
    {
        callback = a;
    }

    virtual protected void DragEndCallback()
    {
        if (swipeAniState == SwipeAniState.Choosen) {
            return;
        }
        if (swipeAniState == SwipeAniState.Resettting || MarkReset()) {
            swipeAniState = SwipeAniState.Resettting;
            return;
        }
        swipeAniState = SwipeAniState.Choosen;
        StartCoroutine(CoroutineUtils.DelaySeconds(() => {
            OnChoose();
        }, 0.3f));
    }

    virtual protected void OnChoose()
    {
        if (callback == null) return;
        callback();
    }

    enum SwipeAniState {
        Idle = 0,
        Resettting = 1,
        Choosen = 2,
        Dragging = 3,
    }

    PlayableDirector left;
    PlayableDirector right;
    bool isReset = false;
    SwipeAniState swipeAniState = SwipeAniState.Idle;
    float swipeFullRange = 320f;
    float swipeLimiter = 0.8f;
    float swipeGoodLimiter = 0.79f;
    PlayableDirector swipeAni = null;
    void UpdateDragChoose()
    {
        if (dragger.dragPosDelta.x == 0) return;
        if (
            swipeAniState != SwipeAniState.Idle && 
            swipeAniState != SwipeAniState.Dragging && 
            swipeAniState != SwipeAniState.Resettting
        ) return;
        var rawXDelta = dragger.dragPosDelta.x / swipeFullRange;
        var normalizedXDragDelta = Mathf.Clamp(rawXDelta, -swipeLimiter, swipeLimiter);
        if (normalizedXDragDelta < 0) {
            swipeAni = left;
        } else if (normalizedXDragDelta > 0) {
            swipeAni = right;
        }
        if (swipeAni == null) return;
        left.Stop();
        right.Stop();
        if (swipeAni.state != PlayState.Playing) {
            swipeAni.Play();
        }
        swipeAni.time = swipeAni.duration * Mathf.Abs(normalizedXDragDelta);
        swipeAniState = SwipeAniState.Dragging;
    }

    void UpdateReset()
    {
        if (!isReset) return;
        if (swipeAni == null) return;
        if (swipeAniState != SwipeAniState.Resettting) return;
        swipeAni.time -= Time.deltaTime;
        if (swipeAni.time <= 0){
            isReset = false;
            swipeAniState = SwipeAniState.Idle;
            StopSwipeAni();
        }
    }

    void StopSwipeAni()
    {
        if (swipeAni == null) return;
        swipeAni.Stop();
        swipeAni.time = 0;
        swipeAni.Evaluate();
        swipeAni = null;
    }

    bool MarkReset()
    {
        isReset = false;
        var absDragDesignX = Mathf.Abs(dragger.dragPosDelta.x);
        if (absDragDesignX < swipeFullRange * swipeGoodLimiter) {
            isReset = true;
        }
        return isReset;
    }

    virtual protected void ResetDialog()
    {
        StopSwipeAni();
        isReset = false;
        swipeAniState = SwipeAniState.Idle;
    }

}
