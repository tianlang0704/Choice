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
}

public class DurFreSystem : SingletonBehaviour<DurFreSystem>
{
    List<AttrInfluence> influTrackingList = new List<AttrInfluence>();
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
        influTrackingList.ForEach((influ) => {
            influ.durFre.curTurn += 1;
        });
        CheckAndRemove();
    }

    public void ResetTurn()
    {
        influTrackingList.ForEach((influ) => {
            influ.durFre.curTurn = 0;
        });
    }

    public void UpdateDay()
    {
        influTrackingList.ForEach((influ) => {
            influ.durFre.curDay += 1;
        });
        CheckAndRemove();
    }

    public void ResetDay()
    {
        influTrackingList.ForEach((influ) => {
            influ.durFre.curDay = 0;
        });
    }

    void CheckAndRemove()
    {
        var influenceToRemove = new List<AttrInfluence>();
        influTrackingList.ForEach((influ) => {
            var durFre = influ.durFre;
            if (durFre.curTurn >= durFre.turn && durFre.curDay >= durFre.day) {
                influenceToRemove.Add(influ);
            }
        });
        influenceToRemove.ForEach((influ) => {
            influTrackingList.Remove(influ);
        });
        DataInfluenceSystem.I.RemoveInfluence(influenceToRemove);
    }

    public void AddInfluenceDurFreControl(AttrInfluence influence)
    {
        var durFre = influence.durFre;
        if (durFre == null || (durFre.turn == 0 && durFre.day == 0)) return;
        ResetDurFre(influence.durFre);
        influTrackingList.Add(influence);
    }

    public void RemoveInfluenceDurFreControl(AttrInfluence influence)
    {
        influTrackingList.Remove(influence);
    }
}
