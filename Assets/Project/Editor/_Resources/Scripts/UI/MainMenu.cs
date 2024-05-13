using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadingScreen;
    public Slider scale;
    public void StartGame()
    {
        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        yield return null;

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync("SIMPLE");
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
