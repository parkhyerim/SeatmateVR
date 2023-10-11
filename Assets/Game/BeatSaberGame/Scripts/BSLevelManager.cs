using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BSLevelManager : MonoBehaviour
{
    public float sceneLoadDelay = 0f;
    int currentLevelIndex; // practice 0


    // private static BSLevelManager instance;

    private void Awake()
    {
       // studyOrder = userstudyManager.GetStudyOrder();
    }
    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
      //  Debug.Log("current scene index: " + currentLevelIndex);
      //  string currentSceneName = SceneManager.GetActiveScene().name;
       // logManager.WriteLogFile("Condition: " + currentSceneName + ", Study Order: " + (currentLevelIndex));        
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1; // 0 -> 1, 1-> 2 ... 

        if (currentLevelIndex < 6 && currentLevelIndex > 1) // trial + four conditions = 5 (index -> 0,1,2,3,4)
            //GoToScene(currentLevelIndex);
            GoToScene("RoundOver");
        else if(currentLevelIndex ==1)
            GoToScene("TrialOver");
           // LoadGameOver();
    }

    public void GoToScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void GoToScene(int sceneIndex)
    {
       // SceneManager.LoadScene(sceneIndex);
        StartCoroutine(WaitAndLoad(sceneIndex, sceneLoadDelay));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("");
    }
    public void LoadGameOver()
    {
        // SceneManager.LoadScene("GameOverScene");
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator WaitAndLoad(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
