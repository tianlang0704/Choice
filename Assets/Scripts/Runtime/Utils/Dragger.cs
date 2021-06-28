using UnityEngine;
 using System;
 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Dragger {
    public delegate void DragEndHandler();
    public DragEndHandler dragEndHandler;
    public Vector3 startPos = Vector3.zero;
    public bool isDragging = false;
    public Vector3 dragPosDelta = Vector3.zero;
    public Vector3 dragPosDeltaFrame = Vector3.zero;
    private Vector3 dragLastFramePos = Vector3.zero;
    public void UpdateDrag()
    {
        dragPosDeltaFrame = Vector3.zero;
        var isDown = Input.GetMouseButtonDown(0);
        if (isDown) {
            isDragging = true;
            startPos = Input.mousePosition * GameUtil.ScreenToDesignFactor().x;
            dragLastFramePos = startPos;
        }
        var isMouseHold = Input.GetMouseButton(0);
        if (isMouseHold) {
            var curMousePosDesign = Input.mousePosition * GameUtil.ScreenToDesignFactor().x;
            dragPosDeltaFrame = curMousePosDesign - dragLastFramePos;
            dragPosDelta = curMousePosDesign - startPos;
            dragLastFramePos = curMousePosDesign;
        }
        var isMouseUp = Input.GetMouseButtonUp(0);
        if (isMouseUp) {
            isDragging = false;
            dragEndHandler.Invoke();
            dragPosDelta = Vector3.zero;
            startPos = Vector3.zero;
            dragLastFramePos = Vector3.zero;
        }
    }
}