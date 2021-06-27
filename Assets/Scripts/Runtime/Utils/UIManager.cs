using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    public Vector2 DesignResolution = new Vector2(750, 1334);
    public GameObject UIRoot;
    public Canvas Canvas;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateResolution();
    }
    private int lastScreenWidth;
    private int lastScreenHeight;
    // Update is called once per frame
    void Update()
    {
        UpdateResolution();
    }

    void UpdateResolution()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height) {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            OnScreenSizeChanged();
        }
    }
 
    void OnScreenSizeChanged()
    {
        var scaler = Canvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = DesignResolution;
        var designWHRatio = (float)DesignResolution.x / DesignResolution.y;
        var screenWHRatio = (float)Screen.width / Screen.height;
        if (screenWHRatio > designWHRatio) {
            scaler.matchWidthOrHeight = 1;
        } else {
            scaler.matchWidthOrHeight = 0;
        }
    }

    public void AddToRoot(GameObject go)
    {
        if (UIRoot == null) return;
        go.transform.SetParent(UIRoot.transform, false);
    }
    public void AddToRoot(Component comp)
    {
        if (UIRoot == null) return;
        AddToRoot(comp.gameObject);
    }
}
