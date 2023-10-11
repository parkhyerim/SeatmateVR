using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IdentifyInteraction : MonoBehaviour
{
    // public InputActionReference pauseReference = null;
    
    public AudioSource audioSource;
    public AudioClip quesitionAudio;
    public Button button;
    public TMP_Text buttonText;
    public AudioSource bgMusicAS;
    bool pauseClicked;
    public GameManager gameManager;
    bool onceClicked;
    public Image interactionImage;
    public BystanderAvatar bAvatar;

    //private void Awake()
    //{
    //    // interactionImage.enabled = false;
    //    pauseReference.action.started += PauseGame;
    //}


    //private void OnDestroy()
    //{
    //    pauseReference.action.started -= PauseGame;
    //}

    //private void PauseGame(InputAction.CallbackContext context)
    //{
    //    bool isActive = !gameObject.activeSelf;
    //    gameObject.SetActive(isActive);

    //}

    public void OnPauseButtonClicked()
    { 
        if (!pauseClicked)
        {
           // Debug.Log("Pause Button is clicked");
            bgMusicAS.Pause();
            if (!onceClicked)
            {
                if (bAvatar.doInteraction)
                    Invoke(nameof(PlayQuestionAudio), 1f);
                onceClicked = true;
                interactionImage.enabled = false;
            }
            button.GetComponentInChildren<TMP_Text>().text = "RESUME";
            button.GetComponent<Image>().color = new Color32(255, 209, 139, 255);
            // buttonText.text = "RESUME";
            pauseClicked = true;
            gameManager.PauseGame();
            
        }
        else 
        {
            button.GetComponentInChildren<TMP_Text>().text = "PAUSE";
            button.GetComponent<Image>().color = new Color32(0, 144, 255, 255);
            bgMusicAS.UnPause();
            gameManager.PauseGame();
            pauseClicked = false;
        }


    }

    public void PlayQuestionAudio()
    {
        audioSource.PlayOneShot(quesitionAudio);
    }

}



