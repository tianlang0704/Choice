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
                20001,
                new SceneData(){
                    name = "路",
                    influence = DataInfluenceSystem.I.GetAttrInfluences(0, 2),
                }
            },
            {
                20002,
                new SceneData(){
                    name = "草原",
                    influence = DataInfluenceSystem.I.GetAttrInfluences(0, 0),
                }
            },
            {
                20003,
                new SceneData(){
                    name = "地狱",
                    influence = DataInfluenceSystem.I.GetAttrInfluences(0, -5),
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
        SetSceneById(20001);
    }

    public void InitIfNot()
    {
        if (current == null) {
            current = AllScenes[20001];
        }
    }

    private void SyncData()
    {
        var sceneID = DataSystem.I.GetAttrDataByType<int>(DataType.Scene);
        current = AllScenes[sceneID];
    }

    public string GetCurrentSceneName() 
    {
        return current.name;
    }

    public void SetSceneById(int id)
    {
        DataSystem.I.SetAttrDataByType(DataType.Scene, id);
        SyncData();
    }
}
