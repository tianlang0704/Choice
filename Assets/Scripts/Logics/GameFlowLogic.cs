﻿// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowLogic : SingletonBehaviour<GameFlowLogic>
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
        var card = CardPoolLogic.Instance.GetRandomCard();
        UILogic.Instance.ShowDialog(card.content, (isOK) => {
            // 获取答案
            Answer answer = isOK ? card.answers[0] : card.answers[1];
            // 改变数值
            AttributesLogic.Instance.ApplyChangeToData(answer.influenceList);
            UILogic.Instance.UpdateView();
            // 显示新提问
            StartCoroutine(CoroutineUtils.DelaySeconds(() => {
                ShowRandomDialog();
            }, 1));
        });
    }
}
