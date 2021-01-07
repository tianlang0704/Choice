using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Card 
{
    public enum CardType { Basic = 0, Trap, Item, Rest, Event, SceneChange }
    public int Id;
    public int DrawPriority = 0;
    public CardType type = CardType.Basic;
    public Condition FillCondition;
    public Condition DrawCondition;
    public string content = "";
    public float baseWeight = 1;
    public List<Answer> answers;
}

public class Answer
{
    public string content;
    public Condition condition;
    public Func<List<LogicExecution>> logicListFunc;
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
}
