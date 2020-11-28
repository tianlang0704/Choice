using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilesManager : SingletonBehaviour<ProfilesManager>
{
    public ProfileChoiceContent profilesChoiceContent = null;

    void Awake()
    {
        if (profilesChoiceContent == null) {
            profilesChoiceContent = ResourceManager.Instance.LoadObject<ProfileChoiceContent>("Excels/ProfileChoiceContent");
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
