﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Weather {
    public int Id;
    public string Name;
    public string Desc;
    public List<AttrInfluence> baseConsumption;
}

public class WeatherLogic : SingletonBehaviour<WeatherLogic>
{
    List<Weather> allWeather;
    Dictionary<int, Weather> allWeatherIdIndex = new Dictionary<int, Weather>();
    Weather curWeather;
    void Awake()
    {
        allWeather = new List<Weather>() {
            new Weather() {
                Id = 1,
                Name = "晴天",
                baseConsumption = DataInfluenceSystem.I.GetAttrInfluences(-2),
            }
        };
        foreach (var weather in allWeather) {
            allWeatherIdIndex[weather.Id] = weather;
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

    public void Init()
    {
        var weatherIndex = Random.Range(0, allWeather.Count());
        DataSystem.I.SetAttrDataByType(DataType.Weather, allWeather[weatherIndex].Id);
    }

    public void SyncWeatherData()
    {
        var weatherId = DataSystem.I.GetAttrDataByType<int>(DataType.Weather);
        curWeather = allWeatherIdIndex[weatherId];
    }

    public Weather GetCurrentWeather(bool isSync = false)
    {
        if (isSync) {
            SyncWeatherData();
        }
        return curWeather;
    }
}
