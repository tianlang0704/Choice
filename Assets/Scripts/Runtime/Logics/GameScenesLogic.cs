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
    public Dictionary<int, SceneData> AllScenes;
    private SceneData current;
    void Awake()
    {
        AllScenes = new Dictionary<int, SceneData>() {
            {
                1,
                new SceneData(){
                    name = "路",
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0, 2),
                }
            },
            {
                2,
                new SceneData(){
                    name = "草原",
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0, 0),
                }
            },
            {
                3,
                new SceneData(){
                    name = "地狱",
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0, -5),
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

    public void Init() 
    {
        SetSceneById(1);
    }

    private void SyncData()
    {
        var sceneID = DataSystem.I.GetDataByType<int>(DataType.Scene);
        if (current != null) {
            
        }
        current = AllScenes[sceneID];
    }

    public string GetCurrentSceneName() 
    {
        return current.name;
    }

    public void SetSceneById(int id)
    {
        DataSystem.I.SetDataByType(DataType.Scene, id);
        SyncData();
    }
}
