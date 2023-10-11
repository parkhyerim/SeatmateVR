using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HideAfterDelay : MonoBehaviour
{
    public float delayInSeconds = 5f;
    public float fadeRate = 0.25f;

    private CanvasGroup canvasGroup;
    private float startTimer; // determines when its time to start hiding the canvas
    private float fadeoutTimer; // calculates the change in the canvas' alpha value over time

    // OnEnable is called any time that the object is enabled, 
    // so for example, if you were to reactive the canvas later on, it will again wait
    // 5 seconds and then fade out again. 
    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        startTimer = Time.time + delayInSeconds;  
        fadeoutTimer = fadeRate;
    }

    private void Update()
    {
        //check the current time to see whether it's time for the fade out.
        if(Time.time >= startTimer)
        {
            fadeoutTimer -= Time.deltaTime;

            // fade out complete?
            if(fadeoutTimer <= 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                // reduce the alpha value
                canvasGroup.alpha = fadeoutTimer / fadeRate;
            }
        }
    }
}
