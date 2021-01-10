using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DurationAndFrequency{
    public int turn = 0;
    public int day = 0;
    public int turnInterval = 0;
    public int dayInterval = 0;
    public int curTurn = -1;
    public int curDay = 0;
    public int curTurnInterval = 0;
    public int curDayInterval = 0;
    public DurationAndFrequency ShallowCopy()
    {
        return (DurationAndFrequency)this.MemberwiseClone();
    }
}

public class DurFreSystem : SingletonBehaviour<DurFreSystem>
{
    // List<DurationAndFrequency> trackingList = new List<DurationAndFrequency>();
    Dictionary<DurationAndFrequency, Action> tracker = new Dictionary<DurationAndFrequency, Action>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ResetDurFre(DurationAndFrequency durFre)
    {
        durFre.curDay = 0;
        durFre.curTurn = -1;
        durFre.curDayInterval = 0;
        durFre.curTurnInterval = 0;
    }

    public void UpdateTurn()
    {   
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.curTurn += 1;
        });
        CheckAndRemove();
    }

    public void ResetTurn()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.curTurn = 0;
        });
    }

    public void UpdateDay()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.curDay += 1;
        });
        CheckAndRemove();
    }

    public void ResetDay()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.curDay = 0;
        });
    }

    void CheckAndRemove()
    {
        var durFreToRemove = new List<DurationAndFrequency>();
        tracker.ToList().ForEach((kvp) => {
            var durFre = kvp.Key;
            if (durFre.curTurn >= durFre.turn && durFre.curDay >= durFre.day) {
                // 添加去除
                durFreToRemove.Add(durFre);
                // 调用回调
                if (kvp.Value != null) {
                    kvp.Value();
                }
            }
        });
        durFreToRemove.ForEach((durFre) => {
            tracker.Remove(durFre);
        });
    }

    public void AddDurFreControl(DurationAndFrequency durFre, Action cb)
    {
        if (durFre == null || (durFre.turn == 0 && durFre.day == 0)) return;
        ResetDurFre(durFre);
        tracker[durFre] = cb;
    }

    public void RemoveDurFreControl(DurationAndFrequency durFre)
    {
        tracker.Remove(durFre);
    }
}
