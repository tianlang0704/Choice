using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class ProfileChoiceContentData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string content;
  public string Content { get {return content; } set { this.content = value;} }
  
  [SerializeField]
  int[] yes_change = new int[0];
  public int[] Yes_Change { get {return yes_change; } set { this.yes_change = value;} }
  
  [SerializeField]
  int[] no_change = new int[0];
  public int[] No_Change { get {return no_change; } set { this.no_change = value;} }
  
}