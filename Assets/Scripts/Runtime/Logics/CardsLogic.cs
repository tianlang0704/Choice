using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card 
{
    public string content;
    public List<Answer> answers;
}

public class Answer
{
    public string content;
    public List<AttrInfluence> influenceList;
    public List<string> logicIdList;
}

public class CardsLogic : SingletonBehaviour<CardsLogic>
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
