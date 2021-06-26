using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class GameUILogic : UILogicBase<GameUILogic>
{
    // Start is called before the first frame update
    void Start()
    {
        // uiRoot.i<Button>("Ex_Back").onClick.AddListener(() => {
        //     CommonFlowLogic.Instance.Town();
        // });
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 更新显示
    public void UpdateView() {
        // 更新属性显示
        UpdateAttrDisplay();
        // 更新日程
        UpdateProgress();
        // 更新调试
        UpdateDebug();
        // 更新道具栏
        UpdateItems();
    }

    // 更新显示数值
    public void UpdateAttrDisplay()
    {
        // 更新最大值
        var curDataDic = AttributesLogic.I.DisplayAttrData;
        var maxValueDic = DataSystem.I.CopyAttrDataWithInfluenceByType<Dictionary<DataType, float>>(DataType.AttrMaxTable);
        foreach (KeyValuePair<DataType, float> kvp in curDataDic) {
            DataType type = kvp.Key;
            if (maxValueDic.ContainsKey(type)) {
                float maxValue = maxValueDic[type];
                UpdateResourceMax((DataType)type, maxValue);
            } else {
                UpdateResourceMax((DataType)type, 0);
            }
        }
        // 更新现在值
        foreach (KeyValuePair<DataType, float> kvp in curDataDic) {
            UpdateResource(kvp.Key, kvp.Value);
        }
        // 更新图标
        foreach (KeyValuePair<DataType, float> kvp in curDataDic) {
            var type = kvp.Key;
            if (!maxValueDic.ContainsKey(type)) continue;
            float maxValue = maxValueDic[type];
            UpdateResourceIcon(type, kvp.Value, maxValue);
        }
    }

    // 更新进度
    public void UpdateProgress()
    {
        var distance = DataSystem.I.GetDataByType<int>(DataType.Distance);
        var maxDistance = DataSystem.I.GetDataByType<int>(DataType.SceneMaxDistance);
        uiRoot.i<UIViewBase>("Ex_UI").i<Slider>("Ex_现在进度条").value = (float)(distance) / (float)maxDistance;
    }

    // 更新最大数值
    public void UpdateResourceMax(DataType type, float amount) 
    {
        // 获取UI
        TextMeshProUGUI text = null;
        var uppperUI = uiRoot.i<UIViewBase>("Ex_UI");
        if (type == DataType.HP) {
            text = uppperUI.i("Ex_资源生命").i<TextMeshProUGUI>("Ex_最多");
        }else if (type == DataType.Stamina) {
            text = uppperUI.i("Ex_资源体力").i<TextMeshProUGUI>("Ex_最多");
        }else if (type == DataType.Mood) {
            text = uppperUI.i("Ex_资源心情").i<TextMeshProUGUI>("Ex_最多");
        }else if (type == DataType.Gold) {
            text = uppperUI.i("Ex_资源金币").i<TextMeshProUGUI>("Ex_最多");
        }
        // 检测空
        if (amount == 0) {
            text.text = " ";
            return;
        }
        // 数量
        text.text = Mathf.Ceil(amount).ToString();
    }

    // 更新数值
    public void UpdateResource(DataType type, float amount) {
        // 获取UI
        var uppperUI = uiRoot.i<UIViewBase>("Ex_UI");
        TextMeshProUGUI text = null;
        if (type == DataType.HP) {
            text = uppperUI.i("Ex_资源生命").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Stamina) {
            text = uppperUI.i("Ex_资源体力").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Mood) {
            text = uppperUI.i("Ex_资源心情").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == DataType.Gold) {
            text = uppperUI.i("Ex_资源金币").i<TextMeshProUGUI>("Ex_现在");
        }
        // 数量
        text.text = Mathf.Ceil(amount).ToString();;
    }

    // 更新图标
    public void UpdateResourceIcon(DataType type, float amount, float max) {
        // 获取UI
        var uppperUI = uiRoot.i<UIViewBase>("Ex_UI");
        Image image = null;
        if (type == DataType.HP) {
            image = uppperUI.i("Ex_资源生命").i<Image>("Ex_图标");
        }else if (type == DataType.Stamina) {
            image = uppperUI.i("Ex_资源体力").i<Image>("Ex_图标");
        }else if (type == DataType.Mood) {
            image = uppperUI.i("Ex_资源心情").i<Image>("Ex_图标");
        }else if (type == DataType.Gold) {
            image = uppperUI.i("Ex_资源金币").i<Image>("Ex_图标");
        }
        if (image == null) return;
        image.fillAmount = amount / max;
    }

    // 更新道具栏
    public void UpdateItems()
    {
        var goods = ItemLogic.I.GetHaveItemListByType();
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Goods);
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Buff);
        // var goods = ItemLogic.I.GetHaveItemListByType(ItemType.Equips);
        var itemBox = uiRoot.i<UIViewBase>("Ex_UI").i<RectTransform>("Ex_道具栏");
        for (int i = itemBox.childCount - 1; i >= 0; i--) {
            var child = itemBox.GetChild(i);
            ObjectPoolManager.I.RecycleGameObject(child.gameObject);
        }
        foreach (var item in goods) {
            var itemButton = ObjectPoolManager.I.GetGameObject<ItemButton>(Constants.UIBasePath + Constants.UIItemPath);
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

    // 更新调试文字
    public void UpdateDebug()
    {
        string outStr = "";
        var sceneData = GameScenesLogic.I.GetCurrentSceneData();
        var sceneName = GameScenesLogic.I.SceneTypeToString(sceneData.sceneType);
        outStr += "场景:" + sceneName;
        // 更新天数
        outStr += ", 天: " + DataSystem.I.GetDataByType<int>(DataType.CurrentDay);
        // 回合
        outStr += ", 回合: " + DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        // 更新路程
        outStr += ", 路程: " + DataSystem.I.GetDataByType<int>(DataType.Distance) + "," + DataSystem.I.GetDataByType<int>(DataType.DistanceTotal);
        // 更新天气
        outStr += ", 天气: " + WeatherLogic.I.GetCurrentWeather(true).Name;
        // 更新卡牌质量
        var quality = DataSystem.I.CopyAttrDataWithInfluenceByType<CardQuality>(DataType.TurnCardQuality);
        outStr += "\n质量: " + Enum.GetName(typeof(CardQuality), quality);
        // 卡牌类型
        var cardType = DataSystem.I.CopyAttrDataWithInfluenceByType<int>(DataType.TurnCardType);
        var cardTypeEnum = (CardType)cardType;
        outStr += "\n类型: " + Enum.GetName(typeof(CardType), cardTypeEnum);
        // 地图路径
        var sceneMap = DataSystem.I.GetDataByType<List<CardType>>(DataType.SceneMap);
        if (sceneMap != null) {
            var curTurn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
            outStr += "\n地图: ";
            for (int i = 0; i < sceneMap.Count; i++) {
                var mapNodeCardType = sceneMap[i];
                var curTypeStr = Enum.GetName(typeof(CardType), mapNodeCardType);
                if (i == curTurn) {
                    curTypeStr = $"<b>{curTypeStr}</b>";
                }
                outStr += curTypeStr;
                if (i < sceneMap.Count - 1) {
                    outStr += ",";
                }
            }
        }
        // 输出信息
        uiRoot.i<UIViewBase>("Ex_UI").i<TextMeshProUGUI>("Ex_调试").text = outStr;
    }
}
