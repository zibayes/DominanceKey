using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AsyncOperation = UnityEngine.AsyncOperation;

public class MainMenuDemo : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadingScreen;
    public Slider scale;

    public void StartGame()
    {
        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(LoadGame());
    }

    public void StartDemo()
    {
        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(LoadDemo());
    }

    public void StartSimulation()
    {
        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(LoadSimulation());
    }

    IEnumerator LoadGame()
    {
        yield return null;

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("DEMO-LEVEL");
        loadAsync.allowSceneActivation = false;

        while (!loadAsync.isDone)
        {
            scale.value = loadAsync.progress / 0.9f * 100;

            if (loadAsync.progress >= 0.9f && !loadAsync.allowSceneActivation)
            {
                yield return new WaitForSeconds(2.2f);
                loadAsync.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator LoadDemo()
    {
        yield return null;

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("TanksDemo");
        loadAsync.allowSceneActivation = false;

        while (!loadAsync.isDone)
        {
            scale.value = loadAsync.progress / 0.9f * 100;

            if (loadAsync.progress >= 0.9f && !loadAsync.allowSceneActivation)
            {
                yield return new WaitForSeconds(2.2f);
                loadAsync.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    IEnumerator LoadSimulation()
    {
        yield return null;

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("TanksBattle");
        loadAsync.allowSceneActivation = false;

        while (!loadAsync.isDone)
        {
            scale.value = loadAsync.progress / 0.9f * 100;

            if (loadAsync.progress >= 0.9f && !loadAsync.allowSceneActivation)
            {
                yield return new WaitForSeconds(2.2f);
                loadAsync.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
