// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : SingletonBehaviour<GameLogic>
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        ShowRandomDialog();
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowRandomDialog() {
        var profiles = ProfilesManager.Instance.profilesChoiceContent;
        var profilesCount = profiles.dataArray.Length;
        var randIndex = Random.Range(0, profilesCount);
        var profileEntry = profiles.dataArray[randIndex];
        UILogic.Instance.ShowDialog(profileEntry.Content, () => {
            // 改变数值
            var yesChangeArr = profileEntry.Yes_Change;
            DataLogic.Instance.ApplyChangeToData(profileEntry.Yes_Change);
            UILogic.Instance.UpdateView();
             // 显示新提问
            StartCoroutine(CoroutineUtils.DelaySeconds(() => {
                ShowRandomDialog();
            }, 1));
        }, () => {
            // 改变数值
            var yesChangeArr = profileEntry.No_Change;
            DataLogic.Instance.ApplyChangeToData(profileEntry.Yes_Change);
            UILogic.Instance.UpdateView();
            // 显示新提问
            StartCoroutine(CoroutineUtils.DelaySeconds(() => {
                ShowRandomDialog();
            }, 1));
        });
    }
}
