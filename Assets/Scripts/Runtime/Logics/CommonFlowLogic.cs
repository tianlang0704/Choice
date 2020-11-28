// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFlowLogic : SingletonBehaviour<CommonFlowLogic>
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void Town()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TownScene");
    }

    public void Diary()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DiaryScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
