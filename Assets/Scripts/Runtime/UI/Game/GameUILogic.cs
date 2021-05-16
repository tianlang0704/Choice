using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class GameUILogic : UILogicBase<GameUILogic>
{
    const string resStrFormat = "<sprite name=\"{0}\">";

    // Start is called before the first frame update
    void Start()
    {
        uiRoot.i<Button>("Ex_导游").onClick.AddListener(()=>{
            CommonFlowLogic.I.ShowDialog("这是导游哦? 他问你要不要<color=#FF0000FF>继续</color>旅程?", (ans) => {
                if (ans == 0) {
                    var isDead = CommonFlowLogic.I.CheckAndNotifyDead();
                    if(!isDead) {
                        TurnFLowLogic.I.ShowTurnDialog();
                    }
                }
            }, "YES", "NO");
        });
        uiRoot.i<Button>("Ex_小僧").onClick.AddListener(()=>{
            CommonFlowLogic.I.ShowDialog("这是一个小僧呢! 他想回<color=#FF0000FF>城镇</color>休息一下, 回去吗?", (ans) => {
                if (ans == 0) {
                    CommonFlowLogic.I.Town();
                }
            }, "YES", "NO");
        });
        // UpdateView(); 
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateView() {
        // 更新现在值
        var curDataArr = AttributesLogic.I.DisplayAttrData;
        foreach (int type in System.Enum.GetValues(typeof(DataType))) {
            if (type >= curDataArr.Count) break;
            var curValue = curDataArr[type];
            UpdateResource((DataType)type, curValue);
        }
        // 更新改变值
        var changeDataArr = AttributesLogic.I.DisplayAttrDataChange;
        foreach (int type in System.Enum.GetValues(typeof(DataType))) {
            if (type >= curDataArr.Count) break;
            var changeValue = changeDataArr[type];
            UpdateChangeResource((DataType)type, changeValue);
        }
        // 更新日程
        var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        var maxTurn = DataSystem.I.GetDataByType<int>(DataType.MaxTurn);
        uiRoot.i<Slider>("Ex_进度条").value = (float)(curTurn) / (float)maxTurn;
        // 更新场景
        string outStr = "";
        var sceneName = GameScenesLogic.I.GetCurrentSceneName();
        outStr += "场景:" + sceneName;
        // 更新天数
        outStr += ", 天: " + DataSystem.I.GetDataByType<int>(DataType.CurrentDay);
        // 更新路程
        outStr += ", 路程: " + DataSystem.I.GetDataByType<int>(DataType.Distance);
        // 更新天气
        outStr += ", 天气: " + WeatherLogic.I.GetCurrentWeather(true).Name;
        // 更新卡牌质量
        var quality = DataSystem.I.CopyAttrDataWithInfluenceByType<CardQuality>(DataType.TurnCardQuality);
        outStr += "\n质量: " + Enum.GetName(typeof(CardQuality), quality);
        // 输出信息
        uiRoot.i<TextMeshProUGUI>("Ex_场景").text = outStr;
        // 更新道具栏
        UpdateItems();
    }

    public void UpdateChangeResource(DataType type, float amount) 
    {
        // 获取UI
        TextMeshProUGUI text = null;
        if (type == DataType.HP) {
            text = uiRoot.gameObject.i("Ex_资源油").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == DataType.Stamina) {
            text = uiRoot.gameObject.i("Ex_资源水").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == DataType.Mood) {
            text = uiRoot.gameObject.i("Ex_资源血").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == DataType.Gold) {
            text = uiRoot.gameObject.i("Ex_资源知识").i<TextMeshProUGUI>("Ex_增减");
        }

        // 检测空
        if (amount == 0) {
            text.text = " ";
            return;
        }

        // 正负号
        var outText = "";
        if (amount > 0) {
            outText += string.Format(resStrFormat, "+");
        }

        // 数量
        var amountText = Mathf.Ceil(amount).ToString();
        foreach (char c in amountText) {
            outText += string.Format(resStrFormat, c);
        }
        text.text = outText;
    }

    public void UpdateResource(DataType type, float amount) {
        // 获取UI
        TextMeshProUGUI text = null;
        if (type == DataType.HP) {
            text = uiRoot.gameObject.i("Ex_资源油").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Stamina) {
            text = uiRoot.gameObject.i("Ex_资源水").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Mood) {
            text = uiRoot.gameObject.i("Ex_资源血").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Gold) {
            text = uiRoot.gameObject.i("Ex_资源知识").i<TextMeshProUGUI>("Ex_现在");
        }

        // 数量
        var outText = "";
        var amountText = Mathf.Ceil(amount).ToString();
        foreach (var c in amountText) {
            outText += string.Format(resStrFormat, c);
        }
        text.text = outText;
    }

    public void UpdateItems()
    {
        var goods = ItemLogic.I.GetHaveItemListByType();
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Goods);
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Buff);
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Equips);
        var itemBox = uiRoot.i<RectTransform>("Ex_道具栏");
        for (int i = itemBox.childCount - 1; i >= 0; i--) {
            var child = itemBox.GetChild(i);
            ObjectPoolManager.I.RecycleGameObject(child.gameObject);
        }
        // foreach (Transform item in itemBox) {
        //     ObjectPoolManager.I.RecycleGameObject(item.gameObject);
        // }
        foreach (var item in goods) {
            var itemButton = ObjectPoolManager.I.GetGameObject<ItemButton>("Prefabs/道具");
            itemButton.transform.SetParent(itemBox, false);
            itemButton.SetText($"{item.Name}\n{item.Desc}*{item.Num}");
            itemButton.SetTextColor(GameUtil.ItemQualityToColor(item.Quality));
            itemButton.SetColor(ItemLogic.I.IsItemConsumable(item.Id) ? Color.green : Color.black);
            itemButton.SetCB(() => {
                ItemLogic.I.ConsumeItem(item.Id, 1, true);
                UpdateView();
            });
        }
    }
}
