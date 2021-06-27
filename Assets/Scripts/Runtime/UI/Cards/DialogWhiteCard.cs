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
        var work = gameObject.i<Button>("Ex_打工");
        work.onClick.RemoveAllListeners();
        var walk = gameObject.i<Button>("Ex_走路");
        walk.onClick.RemoveAllListeners();
    }
    
    public override void SetContent(string content)
    {

    }

    public override void ShowAnsw(string content)
    {

    }

    public void ShowCard(Card c, Action<int> cb = null)
    {
        var work = gameObject.i<Button>("Ex_打工");
        work.onClick.AddListener(() => {
            if (cb != null) {
                cb(0);
            }
        });
        var walk = gameObject.i<Button>("Ex_走路");
        walk.onClick.AddListener(() => {
            if (cb != null) {
                cb(1);
            }
        });
    }
}
