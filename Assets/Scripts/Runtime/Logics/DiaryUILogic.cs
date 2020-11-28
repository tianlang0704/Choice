using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class DiaryUILogic : UILogicBase<DiaryUILogic>
{
    const string resStrFormat = "<sprite name=\"{0}\">";

    // Start is called before the first frame update
    void Start()
    {
        uiRoot.i<Button>("Ex_城镇").onClick.AddListener(() => {
            CommonFlowLogic.Instance.Town();
        });
        UpdateView();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateView() {

    }
}
