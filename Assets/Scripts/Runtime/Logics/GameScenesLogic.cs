using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SceneType {
    Road = 1,
    Grassland,
    Mountain,
    Island,
    Desert,
    End,
}

public class SceneData {
    // public string
    public List<AttrInfluence> influence;
    public int maxDistance;
    public Dictionary <CardType, int> sceneTurnNum = new Dictionary<CardType, int>();
    public SceneType sceneType;
}

public class GameScenesLogic : SingletonBehaviour<GameScenesLogic>
{
    public Dictionary<int, SceneData> AllScenes = new Dictionary<int, SceneData>();
    private SceneData current;
    void Awake()
    {
        AllScenes = new Dictionary<int, SceneData>() {
            {
                (int)SceneType.Road,
                new SceneData(){
                    sceneType = SceneType.Road,
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0, 2),
                    maxDistance = 48,
                    sceneTurnNum = {
                        {CardType.Blank, 10},
                        {CardType.Event, 33},
                        {CardType.Shop, 5},
                    }
                }
            },
            {
                (int)SceneType.Grassland,
                new SceneData(){
                    sceneType = SceneType.Grassland,
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0),
                    maxDistance = 49,
                    sceneTurnNum = {
                        {CardType.Blank, 10},
                        {CardType.Event, 34},
                        {CardType.Shop, 5},
                    }
                }
            },
            {
                (int)SceneType.Mountain,
                new SceneData(){
                    sceneType = SceneType.Mountain,
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0),
                    maxDistance = 39,
                    sceneTurnNum = {
                        {CardType.Blank, 8},
                        {CardType.Event, 27},
                        {CardType.Shop, 4},
                    }
                }
            },
            {
                (int)SceneType.Island,
                new SceneData(){
                    sceneType = SceneType.Island,
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0),
                    maxDistance = 39,
                    sceneTurnNum = {
                        {CardType.Blank, 5},
                        {CardType.Event, 27},
                        {CardType.Shop, 7},
                    }
                }
            },
            {
                (int)SceneType.Desert,
                new SceneData(){
                    sceneType = SceneType.Desert,
                    influence = DataInfluenceSystem.I.GetAttrInfluenceList(0),
                    maxDistance = 39,
                    sceneTurnNum = {
                        {CardType.Blank, 8},
                        {CardType.Event, 27},
                        {CardType.Shop, 4},
                    }
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

    public string SceneTypeToString(SceneType sceneType)
    {
        if (sceneType == SceneType.Road) {
            return "道路";
        } else if (sceneType == SceneType.Grassland) {
            return "草原";
        } else if (sceneType == SceneType.Island) {
            return "岛屿";
        } else if (sceneType == SceneType.Mountain) {
            return "山脉";
        } else if (sceneType == SceneType.Desert) {
            return "沙漠";
        }
        var sceneId = ((int)sceneType).ToString();
        return $"场景{sceneId}";
    }

    public void Init() 
    {
        
    }

    private void SyncData()
    {
        var sceneID = DataSystem.I.GetDataByType<int>(DataType.Scene);
        current = AllScenes[sceneID];
    }

    public SceneData GetCurrentSceneData()
    {
        return current;
    }

    public void SetSceneById(int id, bool isForce = false)
    {
        var sceneID = DataSystem.I.GetDataByType<int>(DataType.Scene);
        if (!isForce && sceneID == id) return;
        DataSystem.I.SetDataByType(DataType.Scene, id);
        SyncData();
        ResetDataForScene();
    }

    public void RegenSceneMap()
    {
        List<CardType> sceneMap = new List<CardType>();
        var sceneTurnNum = current.sceneTurnNum.ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
        for (int i = 0; i < current.maxDistance; i++) {
            var typeList = sceneTurnNum.Keys.Where((key) => sceneTurnNum[key] > 0).ToList();
            if (typeList.Count <= 0) break;
            var cardType = GameUtil.RandomRemoveFromList(typeList);
            sceneMap.Add(cardType);
            sceneTurnNum[cardType] -= 1;
        }
        DataSystem.I.SetDataByType(DataType.SceneMap, sceneMap);
    }

    public void ResetDataForScene()
    {
        // 重新设置数据
        DataSystem.I.SetDataByType(DataType.CurrentDay, 0);
        DataSystem.I.SetDataByType(DataType.CurrentTurn, 0);
        DataSystem.I.SetDataByType(DataType.Distance, 0);
        DataSystem.I.SetDataByType(DataType.SceneMaxDistance, current.maxDistance);
        // 重新随机所有回合 
        RegenSceneMap();
    }
}
