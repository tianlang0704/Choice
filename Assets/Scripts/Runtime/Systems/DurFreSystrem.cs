using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DruFreUpdateType {
    Turn = 1,
    Day
}

public class DurationAndFrequency {
    public int Turn = 0;
    public int Day = 0;
    public int TurnInterval = 0;
    public int DayInterval = 0;
    public int CurTurn = -1;
    public int CurDay = 0;
    public int CurTurnInterval = 0;
    public int CurDayInterval = 0;
    public Action<DruFreUpdateType> UpdateCallback;
    public Func<bool> Predicate;
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
        durFre.CurDay = 0;
        durFre.CurTurn = -1;
        durFre.CurDayInterval = 0;
        durFre.CurTurnInterval = 0;
    }

    public void UpdateTurn()
    {   
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.CurTurn += 1;
            if (kvp.Key.UpdateCallback != null) {
                kvp.Key.UpdateCallback(DruFreUpdateType.Turn);
            }
        });
        CheckAndRemove();
    }

    public void ResetTurn()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.CurTurn = 0;
        });
    }

    public void UpdateDay()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.CurDay += 1;
            if (kvp.Key.UpdateCallback != null) {
                kvp.Key.UpdateCallback(DruFreUpdateType.Day);
            }
        });
        CheckAndRemove();
    }

    public void ResetDay()
    {
        tracker.ToList().ForEach((kvp) => {
            kvp.Key.CurDay = 0;
        });
    }

    void CheckAndRemove()
    {
        // 检查时间
        var durFreToRemove = new List<DurationAndFrequency>();
        tracker.ToList().ForEach((kvp) => {
            var durFre = kvp.Key;
            if (durFre.CurTurn >= durFre.Turn && 
                durFre.CurDay >= durFre.Day &&
                (durFre.Predicate == null || durFre.Predicate())
            ) {
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
        if (durFre == null || (durFre.Turn == 0 && durFre.Day == 0 && durFre.Predicate == null)) {
            cb();
            return;
        }
        ResetDurFre(durFre);
        tracker[durFre] = cb;
    }

    public void RemoveDurFreControl(DurationAndFrequency durFre)
    {
        tracker.Remove(durFre);
    }
}
