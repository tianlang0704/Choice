using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScenesLogic : SingletonBehaviour<GameScenesLogic>
{
    public class SceneData {
        // public string id;
        public string name;
        public List<Influence> influence;
    }
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
            influence = new List<Influence>(){
                new Influence(){
                    abstraction = Abstraction.Card,
                    influenceType = InfluenceType.Possibility,
                    amount = 2
                }
            }
        };
    }

    public string GetCurrentSceneName() 
    {
        return current.name;
    }
}
