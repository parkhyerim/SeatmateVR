using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int currentLevelIndex; // practice 0

    public List<int> studyOrder;

    private static LevelManager instance;

    public int[] userStudyOrder = new int[6];
    private string[] randomedOrder = new string[] {"AnimojiY", "AnimojiN", "AvatarY", "AvatarN", "MixedY", "MixedN"};

    public GameManager gameManager;
    public LogManager logManager;
    
    //public int animoji_Y;
    //public int animoji_N;
    //public int avatar_Y;
    //public int avatar_N;
    //public int mixed_Y;
    //public int mixed_N;
   // private int[] newOrder;
    /**
     * index 0 - practice
     * index 1 - aimoji y
     * index 2 - animoji n
     * index 3 - avatar y
     * index 4 - avatar n
     * index 5 - mixed y
     * index 6 - mixed n
     */
    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Debug.Log(currentSceneName);
        // gameManager.WriteToLogFile("Index: " + currentLevelIndex + currentSceneName);
      //  logManager.WriteToLogFile("Study Order: " + currentLevelIndex + " , name: " + currentSceneName);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1; // 0 -> 1, 1-> 2, 
       // int nextIndex = userStudyOrder[currentLevelIndex-1];
       // Debug.Log("next index: " + nextIndex);
        //  Debug.Log("Level" + currentLevelIndex + "is called");
        // Debug.Log(newOrder[currentLevelIndex]);
        if (currentLevelIndex <= 6)
            GoToScene(currentLevelIndex);
        else
            GoToScene("EndScene");
    }

    public void GoToScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void GoToScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
