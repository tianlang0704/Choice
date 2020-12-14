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
    Dictionary<int, SceneData> allScenes;
    private SceneData current;
    void Awake()
    {
        allScenes = new Dictionary<int, SceneData>() {
            {
                20001,
                new SceneData(){
                    name = "路",
                    influence = DataInfluenceSystem.I.GetAttrInfluences(0, 2),
                }
            },
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SyncData()
    {
        var sceneID = DataSystem.I.GetAttrDataByType<int>(AttributeType.Scene);
        current = allScenes[sceneID];
    }
    public void Init() 
    {
        DataSystem.I.SetAttrDataByType(AttributeType.Scene, 20001);
        SyncData();
    }

    public void InitIfNot()
    {
        if (current == null) {
            current = allScenes[20001];
        }
    }

    public string GetCurrentSceneName() 
    {
        return current.name;
    }
}
