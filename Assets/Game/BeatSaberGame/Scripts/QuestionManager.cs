using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    int gameTimeToZero;
    public int totalGameTime;
    public float getReadyTime;
    public float BystanderStartTime;
    public float bystanderInterval;
    private float BystanderStartTime2, BystanderStartTime3, BystanderStartTime4;
    private float audioPlayTime1, audioPlayTime2, audioPlayTime3;
    private float timeFromSceneLoading, startTimeForSpawningCubes;
    float beforeGameTimer = 0f;
    public bool canStartGame;
    float gameCountTimer;
    bool startGameIsCalled;
    bool q1Played, q2Played, q3Played;
    public AudioSource questionAudioSource;
    public AudioClip[] questionAudios;

    // Start is called before the first frame update
    void Start()
    {
        gameTimeToZero = totalGameTime;
        BystanderStartTime2 = BystanderStartTime + bystanderInterval;
        BystanderStartTime3 = BystanderStartTime2 + bystanderInterval;
        BystanderStartTime4 = BystanderStartTime3 + bystanderInterval;
        audioPlayTime1 = BystanderStartTime2 + 12f;
        audioPlayTime2 = BystanderStartTime3 + 12f;
        audioPlayTime3 = BystanderStartTime4 + 12f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!startGameIsCalled)
            {
                StartGame();
            }
        }

        if (canStartGame)
        {
            if(Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawningCubes)
            {
                beforeGameTimer += Time.fixedDeltaTime;
            }
            else if(Time.time > startTimeForSpawningCubes && gameCountTimer <= totalGameTime)
            {
                gameCountTimer += Time.fixedDeltaTime;

                if(Math.Round(gameCountTimer) == audioPlayTime1)
                {
                    if (!q1Played)
                    {
                        PlayQuestionAudio(0);
                        q1Played = true;
                    }
                   

                }

                if(Math.Round(gameCountTimer) == audioPlayTime2)
                {
                    if (!q2Played)
                    {
                        PlayQuestionAudio(1);
                        q2Played = true;
                    }
                }
                   

                if (Math.Round(gameCountTimer) == audioPlayTime3)
                {
                    if (!q3Played)
                    {
                        PlayQuestionAudio(1);
                        q3Played = true;
                    }
                }
                    

                if (Math.Round(gameCountTimer) == totalGameTime)
                {
                    EndGame();
                }
            }
        }
    }

    private void StartGame()
    {
        canStartGame = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTime;
    }
    private void EndGame()
    {
        
    }

    private void PlayQuestionAudio(int index)
    {
        questionAudioSource.PlayOneShot(questionAudios[index]);
        
        Debug.Log("question is called");
    }
}
