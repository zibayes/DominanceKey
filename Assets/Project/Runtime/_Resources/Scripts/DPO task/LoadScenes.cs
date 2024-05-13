using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadSceneAdditive("Tanks");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadSceneAdditive("SecondScene");
        }
    }

    private void LoadSceneAdditive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
