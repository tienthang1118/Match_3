using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{   
    public void LoadGameplayScene()
    {
        StartCoroutine(LoadSceneAsync(SceneList.GAMEPLAY));
    }
    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone)
        {
            yield return null;
        }
    }
}
public static class SceneList
{
    public const string MAINMENU = "Start";
    public const string GAMEPLAY = "Match3";
}