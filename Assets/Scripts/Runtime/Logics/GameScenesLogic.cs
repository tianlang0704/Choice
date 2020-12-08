using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData {
    // public string id;
    public string name;
    public List<AttrInfluence> influence;
}

public class GameScenesLogic : SingletonBehaviour<GameScenesLogic>
{
    private SceneData current;
    GameScenesLogic()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init() 
    {
        current = new SceneData(){
            name = "路",
            influence = new List<AttrInfluence>(){
                new AttrInfluence(){
                    attributeType = AttributeType.Luck,
                    attr = new Attr() {floatValue = 2f}
                }
            }
        };
    }

    public string GetCurrentSceneName() 
    {
        return current.name;
    }
}
