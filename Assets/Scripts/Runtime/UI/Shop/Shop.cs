using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class Shop : UILogicBase<Shop>
{
    Action<int> cb = null;
    Action quitCb = null;
    // Start is called before the first frame update
    override protected void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        var quitBtn = uiRoot.i<Button>("Ex_离开");
        quitBtn.onClick.AddListener(() => {
            if (quitCb != null) 
                quitCb();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        quitCb = null;
        cb = null;
    }

    // 设置选择回调
    public void SetCB(Action<int> cb)
    {
        this.cb = cb;
    }

    // 设置离开回调
    public void SetQuitCB(Action cb)
    {
        quitCb = cb;
    }

    // 清除内容
    public void ClearContent()
    {
        var parent = uiRoot.i<RectTransform>("Ex_商品内容");
        for (int i = parent.childCount - 1; i >= 0; i--) {
            var child = parent.GetChild(i);
            ObjectPoolManager.I.RecycleGameObject(child.gameObject);
        }
    }

    // 添加道具
    List<Item> itemList = null;
    public void ShowItem(List<Item> itemList)
    {
        this.itemList = itemList;
        ClearContent();
        if (itemList == null || itemList.Count <= 0) return;
        // 添加
        var parent = uiRoot.i<RectTransform>("Ex_商品内容");
        for (int i = 0; i < itemList.Count; i++) {
            var itemData = itemList[i];
            if (itemData.Num <= 0) continue;
            // 实例化
            var itemView = ObjectPoolManager.Instance.GetGameObject<ItemButton>(Constants.UIBasePath + Constants.UIItemPath);
            // 添加进框框    
            itemView.transform.SetParent(parent, false);
            // 添加内容
            itemView.SetText($"{itemData.Name}\n{itemData.Desc}*{itemData.Num}");
            itemView.SetTextColor(Color.black);
            itemView.SetColor(Color.white);
            var copyI = i;
            itemView.SetCB(() => {
                cb(copyI);
            });
        }
    }
    public void UpdateItem()
    {
        ShowItem(itemList);
    }
}
