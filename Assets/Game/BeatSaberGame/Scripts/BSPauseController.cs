using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class BSPauseController : MonoBehaviour
{
    public InputActionReference pauseReference = null;
    public AudioSource audioSource;
    public AudioClip[] quesitionAudios;
    public AudioSource bgMusicAS;
    BSGameManager gameManager;
    BSBystanderAvatar bAvatar;
    BSLogManager logManager;
    [SerializeField]
    private bool[] audioPlayed;
    bool pauseClicked;
    private bool oncePausedInSession;
    public int[] audioOrder = { 1, 2, 3 };
    private int counter;
    public bool OncePausedInSession { get => oncePausedInSession; set => oncePausedInSession = value; }

    private void Awake()
    {
        gameManager = FindObjectOfType<BSGameManager>();
        bAvatar = FindObjectOfType<BSBystanderAvatar>();
        logManager = FindObjectOfType<BSLogManager>();

        pauseReference.action.started += PauseGame;
        pauseReference.action.started += PauseTrial;
        audioPlayed = new bool[3] { false, false, false };
    }

    private void OnDestroy()
    {
        pauseReference.action.started -= PauseGame;
        pauseReference.action.started -= PauseTrial;
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

                if (gameManager.BystanderInteract)
                {
                    if (!oncePausedInSession && gameManager.BystanderCanHearAnswer)
                    {
                       // Invoke(nameof(PlayQuestionAudio), 1f);
                        oncePausedInSession = true;
                    }                   
                }
            
                pauseClicked = true;
                gameManager.PauseGame();
            }
            else
            {
                bgMusicAS.UnPause();
                gameManager.PauseGame();
                pauseClicked = false;
                // logManager.WriteToLogFile("Resume the game again (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
    }

    private void PauseTrial(InputAction.CallbackContext context)
    {
        // bool isActive = !gameObject.activeSelf;
        // gameObject.SetActive(isActive);
        if (gameManager.CanPauseTrial)
        {
            if (!pauseClicked)
            {
                bgMusicAS.Pause();

                if (gameManager.BystanderInteract)
                {
                    if (!oncePausedInSession && gameManager.BystanderCanHearAnswer)
                    {
                        // Invoke(nameof(PlayQuestionAudio), 1f);
                        oncePausedInSession = true;
                    }
                }

                pauseClicked = true;
                gameManager.PauseGame();
            }
            else
            {
                bgMusicAS.UnPause();
                gameManager.PauseGame();
                pauseClicked = false;
                // logManager.WriteToLogFile("Resume the game again (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
    }

    public void PlayQuestionAudio()
    {
        //if (counter < 3)
        //{
        //    int index = audioOrder[counter] - 1;
        //    audioSource.PlayOneShot(quesitionAudios[index]);
        //    counter++;

        //}
    }

    private int GetRandomNumber(int length)
    {
        var rnd = new System.Random();
        int index = rnd.Next(0, length);
        return index;
    }
}
