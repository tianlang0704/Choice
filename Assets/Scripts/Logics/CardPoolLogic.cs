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

    public Card GetRandomCard()
    {
        var profiles = ProfilesManager.Instance.profilesChoiceContent;
        var profilesCount = profiles.dataArray.Length;
        var randIndex = Random.Range(0, profilesCount);
        var profileEntry = profiles.dataArray[randIndex];
        var yesInfluenceList = profileEntry.Yes_Change.Select((num, idx) => {
            AttributeType aType = (AttributeType)idx;
            return new Influence() {
                abstraction = Abstraction.Attribute,
                attributeType = aType,
                amount = num,
            };
        }).ToList();
        var noInfluenceList = profileEntry.No_Change.Select((num, idx) => {
            AttributeType aType = (AttributeType)idx;
            return new Influence() {
                abstraction = Abstraction.Attribute,
                attributeType = aType,
                amount = num,
            };
        }).ToList();
        return new Card() {
            content = profileEntry.Content,
            answers = new List<Answer>() {
                new Answer() {
                    content = "测试卡回答Yes",
                    influenceList = yesInfluenceList,
                },
                new Answer() {
                    content = "测试卡回答No",
                    influenceList = noInfluenceList,
                }
            }
        };
    }
}
