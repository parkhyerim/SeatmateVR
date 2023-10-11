using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStudyManager : MonoBehaviour
{
    static UserStudyManager instance;

    public string participantID; 

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public string GetParticipantID()
    {
        return participantID;
    }
}
