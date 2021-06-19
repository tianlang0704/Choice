using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class DialogWhiteCard : Dialog
{
    void Start()
    {
        
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        var mask = gameObject.i<Button>("Ex_底板按钮");
        mask.onClick.RemoveAllListeners();
    }
    
    public override void SetContent(string content)
    {

    }

    public override void ShowAnsw(string content)
    {

    }

    public void ShowCard(Card c, Action<int> cb = null)
    {
        var mask = gameObject.i<Button>("Ex_底板按钮");
        mask.onClick.AddListener(() => {
            if (cb != null) {
                cb(0);
            }
        });
    }
}
