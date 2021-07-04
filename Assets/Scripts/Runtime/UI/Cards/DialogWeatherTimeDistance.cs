using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using UnityEngine.Playables;

public class DialogWeatherTimeDistance : UILogicBase<DialogWeatherTimeDistance>
{
    Image weather;
    Image icon;
    TextMeshProUGUI time;
    TextMeshProUGUI distance;
    TextMeshProUGUI content;
    override protected void Awake()
    {
        icon = gameObject.i<Image>("Ex_图标");
        weather = gameObject.i<Image>("Ex_天气");
        time = gameObject.i<TextMeshProUGUI>("Ex_时间");
        distance = gameObject.i<TextMeshProUGUI>("Ex_路程");
        content = gameObject.i<TextMeshProUGUI>("Ex_弹窗内容");
        base.Awake();
    }
    
    void Start()
    {

    }

    // Update is called once per frame
    virtual protected void Update()
    {

    }

    virtual protected void OnEnable()
    {
        // 天气
        weather.enabled = false;

        // 距离
        var distanceInt = DataSystem.I.GetDataByType<int>(DataType.DistanceTotal);
        distance.text = $"{distanceInt} KM";

        // 时间
        var turn = DataSystem.I.GetDataByType<int>(DataType.CurrentTurn);
        var dayMaxTurn = DataSystem.I.GetDataByType<int>(DataType.DayMaxTurn);
        var dayTurn = turn % dayMaxTurn;
        var hourInt = dayTurn + 8;
        var isPM = false;
        if (hourInt >= 12) {
            hourInt -= 12;
            isPM = true;
        }
        var minuteInt = UnityEngine.Random.Range(0, 60);
        var prefix = isPM ? "PM" : "AM"; 
        var timeStr = $"{prefix} {string.Format("{0:D2}", hourInt)}:{string.Format("{0:D2}", minuteInt)}";
        time.text = timeStr;
    }

    virtual protected void OnDisable()
    {

    }

}
