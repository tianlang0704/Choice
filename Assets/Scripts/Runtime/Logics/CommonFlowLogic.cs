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
}
