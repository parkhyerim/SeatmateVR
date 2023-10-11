using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        StartCoroutine(ShowOverlayAndLoad(sceneName));
    }

    IEnumerator ShowOverlayAndLoad(string sceneName)
    {
        // Waiting some seconds to prevent "pop" to new scene
        yield return new WaitForSeconds(3f);

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return null;
    
    }
}
