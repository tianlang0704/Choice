using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Abstraction 
{
    Attribute = 1,
    Scene,
    Weather,
    Card,
    Item,
    Equipment,
    Teamate,
}

public enum AttributeType {
    Oil = 0,
    Water,
    HP,
    Knowledge,
}

public enum InfluenceType 
{
    Possibility = 1,
}

public class Influence 
{
    public Abstraction abstraction;
    public AttributeType attributeType;
    public InfluenceType influenceType;
    public float amount;
}

public class Answer
{
    public string content;
    public List<Influence> influenceList;
    public List<string> logicIdList;
}

public class Card 
{
    public string content;
    public List<Answer> answers;
}