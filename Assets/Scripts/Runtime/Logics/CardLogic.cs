using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Card 
{
    public enum CardType { Basic = 0, Trap, Item, Rest, Event, SceneChange }
    public int Id;
    public CardType type = CardType.Basic;
    public Condition condition;
    public string content = "";
    public float baseWeight = 1;
    public List<Answer> answers;
}

public class Answer
{
    public string content;
    public Condition condition;
    public List<AttrInfluence> influenceList;
    public List<LogicExecution> logicList;
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
