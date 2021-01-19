using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public enum CardType { Basic = 0, Trap, Item, Rest, Event, SceneChange }
public enum CardQuality { Red = 0, White, Green, Blue, Purple, Gold, Special }
public class Card 
{
    public int Id;
    public int DrawPriority = 0;
    public bool IsMaskable = true;
    public CardType Type = CardType.Basic;
    public CardQuality Quality = CardQuality.White;
    public Condition FillCondition;
    public Condition DrawCondition;
    public Condition SeeCondition = new Condition() { Formula = "IsHaveItem(6)==0" };
    public string content = "";
    public float baseWeight = 1;
    public List<Answer> answers;
    public Card ShallowCopy()
    {
        return (Card)this.MemberwiseClone();
    }
}

public class Answer
{
    public string content;
    public Condition condition;
    public Func<List<LogicExecution>> logicListFunc;
    public Answer ShallowCopy()
    {
        return (Answer)this.MemberwiseClone();
    }
}

public class CardLogic : SingletonBehaviour<CardLogic>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MaskCard(Card c)
    {
        c.content = "???";
        var oriAnswers = c.answers;
        c.answers = new List<Answer>();
        for (int i = 0; i < oriAnswers.Count; i++)
        {
            var a = oriAnswers[i].ShallowCopy();
            a.content = "????";
            c.answers.Add(a);
        }
    }
}
