using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPoolLogic : SingletonBehaviour<CardPoolLogic>
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        
    }

    private List<AttrInfluence> ChangeToInfluence(int[] change)
    {
        return change.Select((num, idx) => {
            AttributeType aType = (AttributeType)idx;
            return new AttrInfluence() {
                attributeType = aType,
                attr = new Attr() {floatValue = num}
            };
        }).ToList();
    }

    public Card GetRandomCard()
    {
        var profiles = ProfilesManager.Instance.GetProfile<ProfileChoiceContent>();
        var profilesCount = profiles.dataArray.Length;
        var randIndex = Random.Range(0, profilesCount);
        var profileEntry = profiles.dataArray[randIndex];
        var answers = new List<Answer>(){
            new Answer() {
                content = profileEntry.Answer1_Content,
                influenceList = AttributeInfluenceSystem.Instance.GetInfluencesFromID(profileEntry.Answer1_Influence),
            },
            new Answer() {
                content = profileEntry.Answer2_Content,
                influenceList = AttributeInfluenceSystem.Instance.GetInfluencesFromID(profileEntry.Answer2_Influence),
            },
            new Answer() {
                content = profileEntry.Answer3_Content,
                influenceList = AttributeInfluenceSystem.Instance.GetInfluencesFromID(profileEntry.Answer3_Influence),
            },
            new Answer() {
                content = profileEntry.Answer4_Content,
                influenceList = AttributeInfluenceSystem.Instance.GetInfluencesFromID(profileEntry.Answer4_Influence),
            },
        };
        return new Card() {
            content = profileEntry.Content,
            answers = answers,
        };
    }
}
