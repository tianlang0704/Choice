// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Linq;

public class CommonFlowLogic : SingletonBehaviour<CommonFlowLogic>
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void Town()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TownScene");
    }

    public void Diary()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DiaryScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private Dialog dialog = null;
    public void ShowDialog(string content, Action<int> cb = null, params string[] list) {
        if (dialog == null) {
            dialog = ObjectPoolManager.Instance.GetGameObject<Dialog>("Prefabs/弹窗");
        }
        var root = FindObjectOfType<Canvas>();
        dialog.gameObject.SetActive(true);
        dialog.transform.SetParent(root.transform, false);
        dialog.SetContent(content);
        dialog.SetCB((b) => {
            if (cb == null) return;
            cb(b);
        });
        dialog.ResetAllAnsws();
        for (int i = 0; i < list.Length; i++) {
            var arg = list[i];
            dialog.ShowAnsw(arg);
        }
    }

    public void CloseDialog()
    {
        if (dialog == null) return;
        dialog.gameObject.SetActive(false);

    }

    public bool CheckAndNotifyDead()
    {
        // 如果死亡, 显示死亡提示
        var isDead = AttributesLogic.Instance.IsDead();
        if (isDead) {
            CommonFlowLogic.Instance.ShowDialog("哦豁! 死求了啊. 完了撒. <color=#FF0000FF>回城</color>!!!", (answIdx) => {
                if (answIdx == 0) {
                    CommonFlowLogic.Instance.Town();
                }
            }, "YES", "NO");
        }
        return isDead;
    }

    public void ShowSelectSceneDialog()
    {
        if (dialog == null) {
            dialog = ObjectPoolManager.Instance.GetGameObject<Dialog>("Prefabs/弹窗");
        }
        var root = FindObjectOfType<Canvas>();
        dialog.gameObject.SetActive(true);
        dialog.transform.SetParent(root.transform, false);
        dialog.SetContent("选择一个新场景");
        dialog.SetCB((b) => {
            // 更新场景
            GameScenesLogic.I.SetSceneById(1+b);
            // 重新刷新卡牌
            CardPoolLogic.I.ShuffleDayCards();
            TurnFLowLogic.I.NextTurn();
        });
        dialog.ResetAllAnsws();
        var nameList = GameScenesLogic.I.AllScenes.Select((s)=>s.Value.name).ToList();
        for (int i = 0; i < nameList.Count; i++) {
            var arg = nameList[i];
            dialog.ShowAnsw(arg);
        }
    }

    private Shop shop = null;
    public void ShowSellShop(List<Item> itemList, Action<int> cb = null, Action closeCb = null)
    {
        // 创建
        if (shop == null) {
            shop = ObjectPoolManager.Instance.GetGameObject<Shop>("Prefabs/商店");
        }
        // 加入场景
        var root = FindObjectOfType<Canvas>();
        shop.gameObject.SetActive(true);
        shop.transform.SetParent(root.transform, false);
        // 设置回调
        shop.SetCB((idx) => {
            var item = itemList[idx];
            // 更新道具
            ItemLogic.I.SellItem(item.Id, 1);
            item.Num--;
            // 更新显示
            GameUILogic.I.UpdateView();
            shop.UpdateItem();
            if (cb != null) cb(idx);
        });
        shop.SetQuitCB(() => {
            shop.gameObject.SetActive(false);
            TurnFLowLogic.I.NextTurn();
            if (closeCb != null) closeCb();
        });
        // 展示物品
        shop.ShowItem(itemList);
    }
    public void ShowBuyShop(List<Item> itemList, Action<int> cb = null, Action closeCb = null)
    {
        // 创建
        if (shop == null) {
            shop = ObjectPoolManager.Instance.GetGameObject<Shop>("Prefabs/商店");
        }
        // 加入场景
        var root = FindObjectOfType<Canvas>();
        shop.gameObject.SetActive(true);
        shop.transform.SetParent(root.transform, false);
        // 设置回调
        shop.SetCB((idx) => {
            var item = itemList[idx];
            // 更新道具
            if (ItemLogic.I.IsCanBuy(item.Id, 1)) {
                ItemLogic.I.BuyItem(item.Id, 1);
                item.Num--;
            } else {
                // TODO: 显示买不起
            }
            // 更新显示
            GameUILogic.I.UpdateView();
            shop.UpdateItem();
            if (cb != null) cb(idx);
        });
        shop.SetQuitCB(() => {
            shop.gameObject.SetActive(false);
            TurnFLowLogic.I.NextTurn();
            if (closeCb != null) closeCb();
        });
        // 展示物品
        shop.ShowItem(itemList);
    }

    public void ShowShop(List<ItemType> typeList = null, bool isBuy = true)
    {
        // 检查参数
        if (typeList == null) {
            typeList = new List<ItemType>(){ItemType.Goods};
        }
        // 获取列表
        List<Item> itemList = null;
        if (isBuy) {
            itemList = ItemLogic.I.GetAllItemListByType(typeList);
        } else {
            itemList = ItemLogic.I.GetHaveItemListByType(typeList);
        }
        // 实例化列表数据
        itemList = itemList.Select((i)=>{
            i = ItemLogic.I.InstantiateItem(i);
            i.Num = i.Num <= 0 ? 1 : i.Num;
            return i;
        }).ToList();
        // 展示商店
        if (isBuy) {
            ShowBuyShop(itemList);
        } else {
            ShowSellShop(itemList);
        }
    }
}
