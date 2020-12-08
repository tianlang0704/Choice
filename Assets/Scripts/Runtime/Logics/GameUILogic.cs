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
            CommonFlowLogic.Instance.ShowDialog("这是导游哦? 他问你要不要<color=#FF0000FF>继续</color>旅程?", (isYes) => {
                if (isYes) {
                    GameFlowLogic.Instance.ShowRandomDialog();
                }
            });
        });
        uiRoot.i<Button>("Ex_小僧").onClick.AddListener(()=>{
            CommonFlowLogic.Instance.ShowDialog("这是一个小僧呢! 他想回<color=#FF0000FF>城镇</color>休息一下, 回去吗?", (isYes) => {
                if (isYes) {
                    CommonFlowLogic.Instance.Town();
                }
            });
            
        });
        UpdateView(); 
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateView() {
        // 更新现在值
        var curDataArr = AttributesLogic.Instance.DisplayAttrData;
        foreach (int type in System.Enum.GetValues(typeof(AttributeType))) {
            if (type >= curDataArr.Count) break;
            var curValue = curDataArr[type];
            UpdateResource((AttributeType)type, curValue);
        }
        // 更新改变值
        var changeDataArr = AttributesLogic.Instance.DisplayAttrDataChange;
        foreach (int type in System.Enum.GetValues(typeof(AttributeType))) {
            if (type >= curDataArr.Count) break;
            var changeValue = changeDataArr[type];
            UpdateChangeResource((AttributeType)type, changeValue);
        }
        // 更新场景
        var sceneName = GameScenesLogic.Instance.GetCurrentSceneName();
        uiRoot.i<TextMeshProUGUI>("Ex_场景").text = "场景:" + sceneName;
    }

    public void UpdateChangeResource(AttributeType type, float amount) 
    {
        // 获取UI
        TextMeshProUGUI text = null;
        if (type == AttributeType.Oil) {
            text = uiRoot.gameObject.i("Ex_资源油").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == AttributeType.Water) {
            text = uiRoot.gameObject.i("Ex_资源水").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == AttributeType.Hp) {
            text = uiRoot.gameObject.i("Ex_资源血").i<TextMeshProUGUI>("Ex_增减");
        }else if (type == AttributeType.Knowledge) {
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
        var amountText = amount.ToString();
        foreach (char c in amountText) {
            outText += string.Format(resStrFormat, c);
        }
        text.text = outText;
    }

    public void UpdateResource(AttributeType type, float amount) {
        // 获取UI
        TextMeshProUGUI text = null;
        if (type == AttributeType.Oil) {
            text = uiRoot.gameObject.i("Ex_资源油").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == AttributeType.Water) {
            text = uiRoot.gameObject.i("Ex_资源水").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == AttributeType.Hp) {
            text = uiRoot.gameObject.i("Ex_资源血").i<TextMeshProUGUI>("Ex_现在");
        }else if (type == AttributeType.Knowledge) {
            text = uiRoot.gameObject.i("Ex_资源知识").i<TextMeshProUGUI>("Ex_现在");
        }

        // 数量
        var outText = "";
        var amountText = amount.ToString();
        foreach (var c in amountText) {
            outText += string.Format(resStrFormat, c);
        }
        text.text = outText;
    }
}
