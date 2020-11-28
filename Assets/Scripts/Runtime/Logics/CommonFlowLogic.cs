// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

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
        AttributesLogic.Instance.InitData();
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
    public void ShowDialog(string content, Action<bool> cb = null) {
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
    }
}
