// using System;
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

    private bool CheckDead()
    {
        var isDead = AttributesLogic.Instance.IsDead();
        if (isDead) {
            CommonFlowLogic.Instance.ShowDialog("哦豁! 死求了啊. 完了撒. <color=#FF0000FF>回城</color>!!!", (isOK) => {
                if (isOK) {
                    CommonFlowLogic.Instance.Town();
                }
            });
        }
        return isDead;
    }

    public void ShowRandomDialog() {
         // 检查死亡
        var isDead = CheckDead();
        if (isDead) return;
        // 抽卡
        var card = CardPoolLogic.Instance.GetRandomCard();
        CommonFlowLogic.Instance.ShowDialog(card.content, (isOK) => {
            // 获取答案
            Answer answer = isOK ? card.answers[0] : card.answers[1];
            // 改变数值
            AttributesLogic.Instance.ApplyChangeToData(answer.influenceList);
            // 更新界面
            GameUILogic.Instance.UpdateView();
            // 显示新提问
            StartCoroutine(CoroutineUtils.DelaySeconds(() => {
                ShowRandomDialog(); 
            }, 0.1f));
        });
    }
}
