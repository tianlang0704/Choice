using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ProfilesManager : SingletonBehaviour<ProfilesManager>
{
    Dictionary<Type, (Type, string)> profileConfig = new Dictionary<Type, (Type, string)>() {
        {typeof(ProfileChoiceContent), (ProfileChoiceContent.dataType, "Excels/ProfileChoiceContent")},
        {typeof(ProfileInfluence), (ProfileInfluence.dataType, "Excels/ProfileInfluence")},
    };
    public Dictionary<Type, IIDAbleProfileContent> profiles = new Dictionary<Type, IIDAbleProfileContent>();
    public Dictionary<Type, Dictionary<int, IIDAble>> profilesById = new Dictionary<Type, Dictionary<int, IIDAble>>();
    public Dictionary<Type, Type> profileToDataType;
    public Dictionary<Type, Type> dataToProfileType;


    void Awake()
    {
        profileToDataType = profileConfig.ToDictionary((k)=>k.Key, (k)=>k.Value.Item1);
        dataToProfileType = profileConfig.ToDictionary((k)=>k.Value.Item1, (k)=>k.Key);
        foreach (var kvp in profileConfig) {
            var profileContent = ResourceManager.Instance.LoadObject<IIDAbleProfileContent>(kvp.Value.Item2);
            profiles[kvp.Key] = profileContent;
            profilesById[profileContent.DataType] = profileContent.DataArray.ToDictionary(
                (dataKvp) => dataKvp.Id, 
                (dataKvp) => dataKvp
            );
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

    public (T, bool) GetProfileAndCheckByID<T>(int id) where T : IIDAble
    {
        var profile = GetProfileIDAble<T>();
        if (profile == null || !profile.ContainsKey(id)) return (default(T), false);
        return (profile[id], true);
    }

    public T GetProfileByID<T>(int id) where T : IIDAble
    {
        (T res, bool isSuccess) = GetProfileAndCheckByID<T>(id);
        return res;
    }

    public Dictionary<int, T> GetProfileIDAble<T>() where T : IIDAble
    {
        if (!profilesById.ContainsKey(typeof(T))) return null;
        return profilesById[typeof(T)].ToDictionary((k)=>k.Key, (k)=>(T)k.Value);
    }

    public List<T> GetProfileDataList<T>() where T : IIDAble
    {
        if (!dataToProfileType.ContainsKey(typeof(T))) return null;
        var profileType = dataToProfileType[typeof(T)];
        if (!profiles.ContainsKey(profileType)) return null;
        return profiles[profileType].DataArray.Select((entry)=>(T)entry).ToList();
    }

    public T GetProfile<T>() where T : ScriptableObject
    {
        if (!profiles.ContainsKey(typeof(T))) return null;
        return profiles[typeof(T)] as T;
    }
}
