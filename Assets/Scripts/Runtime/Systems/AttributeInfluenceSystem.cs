using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttributeInfluenceSystem : SingletonBehaviour<AttributeInfluenceSystem>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float GetRandomAttr(int [] randArr)
    {
        return GetRandomAttr(randArr.Select((e)=>(float)e).ToList());    
    }

    float GetRandomAttr(float [] randArr)
    {
        return GetRandomAttr(randArr.ToList());
    }

    float GetRandomAttr(List<float> randList)
    {
        float min = randList.Count > 0 ? randList[0] : 0f;
        float max = randList.Count > 1 ? randList[1] : min;
        return Random.Range(min, max);
    }

    public List<AttrInfluence> GetInfluencesFromID(int id)
    {
        var infProfile = ProfilesManager.Instance.GetProfileByID<ProfileInfluenceData>(id);
        if (infProfile == null) return null;
        var infList = new List<AttrInfluence>() {
            new AttrInfluence() {attributeType = AttributeType.Oil, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Oil)}},
            new AttrInfluence() {attributeType = AttributeType.Hp, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Hp)}},
            new AttrInfluence() {attributeType = AttributeType.Water, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Water)}},
            new AttrInfluence() {attributeType = AttributeType.Knowledge, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Knowledge)}},
            new AttrInfluence() {attributeType = AttributeType.Bag, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Bag)}},
            new AttrInfluence() {attributeType = AttributeType.Day, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Day)}},
            new AttrInfluence() {attributeType = AttributeType.Luck, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Luck)}},
            new AttrInfluence() {attributeType = AttributeType.Distance, attr = new Attr() {floatValue = GetRandomAttr(infProfile.Distance)}},
        };
        return infList;
    } 
}
