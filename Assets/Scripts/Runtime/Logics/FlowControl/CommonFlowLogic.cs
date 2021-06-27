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
    public GameObject UIHost;
    private Component commonDialog = null;
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

    public void ShowStartDialog(Action cb = null)
    {
        if (commonDialog != null) {
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
        }
        var dialog = ObjectPoolManager.Instance.GetGameObject<DialogSwipe>(Constants.UIBasePath + Constants.UIStartCardPath);
        commonDialog = dialog;
        UIManager.I.AddToRoot(dialog);
        dialog.gameObject.SetActive(true);
        dialog.SetCallback(() => {
            if (cb != null) cb();
        });
    }

    public void ShowDialog(string content, Action<int> cb = null, params string[] list) {
        if (commonDialog != null) {
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
        }
        var dialog = ObjectPoolManager.Instance.GetGameObject<Dialog>(Constants.UIBasePath + Constants.UICardPath);
        commonDialog = dialog;
        UIManager.I.AddToRoot(dialog);
        dialog.gameObject.SetActive(true);
        dialog.SetContent(content);
        dialog.SetCB((b) => {
            if (cb == null) return;
            cb(b);
        });
        // 设置答案
        dialog.ResetAllAnsws();
        for (int i = 0; i < list.Length; i++) {
            var arg = list[i];
            dialog.ShowAnsw(arg);
        }
        // 设置颜色
        dialog.SetColor(Color.white);
    }

    public void ShowDialogWithColor(string content, Color color, Action<int> cb = null, params string[] list)
    {
        if (commonDialog != null) {
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
        }
        var dialog = ObjectPoolManager.Instance.GetGameObject<Dialog>(Constants.UIBasePath + Constants.UICardPath);
        commonDialog = dialog;
        UIManager.I.AddToRoot(dialog);
        dialog.gameObject.SetActive(true);
        dialog.SetContent(content);
        dialog.SetCB((b) => {
            if (cb == null) return;
            cb(b);
        });
        // 设置答案
        dialog.ResetAllAnsws();
        for (int i = 0; i < list.Length; i++) {
            var arg = list[i];
            dialog.ShowAnsw(arg);
        }
        // 设置颜色
        dialog.SetColor(color);
    }

    public void ShowCardWithColor(Card card, Action<int> cb = null) {
        // 显示卡片
        var color = GameUtil.CardQualityToColor(card.Quality);
        CommonFlowLogic.I.ShowDialogWithColor(
            card.content, 
            color,
            (ansNum) => {
                // 获取答案
                Answer answer = card.answers[ansNum];
                // 执行逻辑列表
                if (answer.logicListFuncList != null) {
                    var logicList = answer.logicListFuncList
                        .Select((func)=>func())
                        .SelectMany((logicExeutionList)=>logicExeutionList)
                        .ToList();
                    CommonLogicSystem.I.ExecuteCommonLogic(logicList);
                }
                if (cb != null) cb(ansNum);
                // 更新界面
                GameUILogic.I.UpdateView();
            }, 
            card.answers
                .Where((a)=>ConditionSystem.I.IsConditionMet(a.condition))
                .Select((a)=>a.content)
                .ToArray()
        );
    }

    public void CloseDialog()
    {
        if (commonDialog == null) return;
        commonDialog.gameObject.SetActive(false);
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

    public void ShowEndGame(Action cb = null) {
        var card = CardLogic.I.GetCardById(GameUtil.CardId(10004));
        if (card == null) return;
        // 显示卡片
        ShowCardWithColor(card, (ansNum) => { if(cb != null) cb(); });
        
    }

    // 显示事件
    public void ShowEvent() 
    {
        var card = CardPoolLogic.I.GetTurnCardInstance(); // 抽到卡就用, 没抽到卡就用通用提示卡
        if (card == null) return;
        // 显示卡片
        ShowCardWithColor(card, (a) => {
            // 下一回合
            TurnFLowLogic.I.NextTurn();
        });
    }

    // 显示打工或者跑路
    public void ShowWorkOrRun(Action<bool, int> cb = null)
    {
        if (commonDialog != null) {
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
        }
        var card = ObjectPoolManager.Instance.GetGameObject<DialogSwipeBlank>(Constants.UIBasePath + Constants.UIWhiteCardPath);
        commonDialog = card;
        UIManager.I.AddToRoot(card);
        card.gameObject.SetActive(true);
        card.SetCallback(() => {
            if (cb != null) cb(card.IsWork, card.Duration);
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
            commonDialog = null;
        });
    }

    // 显示回合提问
    public void ShowTurnDialog() {
        // 处理没有卡牌
        var cardInfo = CardPoolLogic.I.GetTurnCardRaw();
        if (cardInfo == null) {
            var card = CardLogic.I.GetCardCopyById(GameUtil.CardId(10002));
            ShowCardWithColor(card, (a) => {
                // 下一回合
                TurnFLowLogic.I.NextTurn();
            });
            return;
        }
        // 分类显示卡牌
        // if (cardInfo.Type == CardType.Event) {
            ShowEvent();
        // } else if (cardInfo.Type == CardType.Shop) {
            
        // }
    }

    // 显示选择场景
    public void ShowSelectSceneDialog()
    {
        if (commonDialog != null) {
            ObjectPoolManager.Instance.RecycleGameObject(commonDialog.gameObject);
        }
        var dialog = ObjectPoolManager.Instance.GetGameObject<Dialog>(Constants.UIBasePath + Constants.UICardPath);
        dialog.gameObject.SetActive(true);
        UIManager.I.AddToRoot(dialog);
        dialog.SetContent("选择一个新场景");
        dialog.SetCB((b) => {
            // 更新场景
            GameScenesLogic.I.SetSceneById(1 + b, true);
            SceneFlowLogic.I.NextScene();
        });
        dialog.ResetAllAnsws();
        var nameList = GameScenesLogic.I.AllScenes.Select((s)=>GameScenesLogic.I.SceneTypeToString(s.Value.sceneType)).ToList();
        for (int i = 0; i < nameList.Count; i++) {
            var arg = nameList[i];
            dialog.ShowAnsw(arg);
        }
        dialog.SetColor(Color.white);
    }

    private Shop shop = null;
    public void ShowSellShop(List<Item> itemList, Action<int> cb = null, Action closeCb = null)
    {
        // 创建
        if (shop == null) {
            shop = ObjectPoolManager.Instance.GetGameObject<Shop>(Constants.UIBasePath + Constants.UIShopPath);
        }
        // 加入场景
        UIManager.I.AddToRoot(shop);
        shop.gameObject.SetActive(true);
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
            shop = ObjectPoolManager.Instance.GetGameObject<Shop>(Constants.UIBasePath + Constants.UIShopPath);
        }
        // 加入场景
        UIManager.I.AddToRoot(shop);
        shop.gameObject.SetActive(true);
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
