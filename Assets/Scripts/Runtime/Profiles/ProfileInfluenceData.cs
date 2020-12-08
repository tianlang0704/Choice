using UnityEngine;
using System;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class ProfileInfluenceData : IIDAble
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string comment;
  public string Comment { get {return comment; } set { this.comment = value;} }
  
  [SerializeField]
  int[] oil = new int[0];
  public int[] Oil { get {return oil; } set { this.oil = value;} }
  
  [SerializeField]
  int[] water = new int[0];
  public int[] Water { get {return water; } set { this.water = value;} }
  
  [SerializeField]
  int[] hp = new int[0];
  public int[] Hp { get {return hp; } set { this.hp = value;} }
  
  [SerializeField]
  int[] knowledge = new int[0];
  public int[] Knowledge { get {return knowledge; } set { this.knowledge = value;} }
  
  [SerializeField]
  int[] luck = new int[0];
  public int[] Luck { get {return luck; } set { this.luck = value;} }
  
  [SerializeField]
  int[] bag = new int[0];
  public int[] Bag { get {return bag; } set { this.bag = value;} }
  
  [SerializeField]
  int[] distance = new int[0];
  public int[] Distance { get {return distance; } set { this.distance = value;} }
  
  [SerializeField]
  int[] day = new int[0];
  public int[] Day { get {return day; } set { this.day = value;} }
  
}