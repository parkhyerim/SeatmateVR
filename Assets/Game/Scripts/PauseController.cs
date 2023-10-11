using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PauseController : MonoBehaviour
{
    public InputActionReference pauseReference = null;
    public AudioSource audioSource;
    public AudioClip quesitionAudio;
    public AudioSource bgMusicAS;
    bool pauseClicked;
    bool oncePaused;
    public GameManager gameManager;
    public BystanderAvatar bAvatar;
    public LogManager logManager;

    private void Awake()
    {
        pauseReference.action.started += PauseGame;
    }

    private void OnDestroy()
    {
        pauseReference.action.started -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        // bool isActive = !gameObject.activeSelf;
        // gameObject.SetActive(isActive);
        if (gameManager.CanPauseGame)
        {
            if (!pauseClicked)
            {
                bgMusicAS.Pause();
             //   Debug.Log("pause clicked");
                if (!oncePaused && gameManager.BystanderInteract)
                {
                    // gameManager.WriteToLogFile("Test1");
                  //  logManager.WriteToLogFile("Identify time");
                    if (bAvatar.doInteraction)
                        Invoke(nameof(PlayQuestionAudio), 1f);
                    oncePaused = true;
                }
                pauseClicked = true;
                gameManager.PauseGame();
            }
            else
            {
               // Debug.Log("resume clciked");
                bgMusicAS.UnPause();
                gameManager.PauseGame();
                pauseClicked = false;
                //  gameManager.WriteToLogFile("Test2");
               // logManager.WriteToLogFile("Resume the game again (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }      
    }

    public void PlayQuestionAudio()
    {
        audioSource.PlayOneShot(quesitionAudio);
    }
}
