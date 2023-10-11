using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyManager : MonoBehaviour
{
    public ToggleGroupManager[] toggleGroupM;
    private bool allTrue;
    public Button warningBtn;
    public Button SubmitBtn;
    public LogManager logManager;
    public GameManager gameManager;

    private void Awake()
    {
        warningBtn.gameObject.SetActive(false);
        
      //  warningBtn.enabled = false;
    }

    public void CheckSurveyResult()
    {
        foreach(ToggleGroupManager toggleG in toggleGroupM)
        {
           // Debug.Log(toggleG.Question + "  "  + toggleG.CheckAnswered());
            // toggleG.SendAnswer();
            allTrue = toggleG.CheckAnswered();
            if (!toggleG.CheckAnswered())
            {
                warningBtn.gameObject.SetActive(true);
                SubmitBtn.gameObject.SetActive(false);
                Invoke(nameof(ControlWarning), 2f);
            }
        }

        if (allTrue)
        {
            Debug.Log("is is all true");
            SendResult();

           // Debug.Log(toggleG.Question + "  " + toggleG.CheckAnswered());
        }
        //else
        //{
        //    warningBtn.gameObject.SetActive(true);
        //    SubmitBtn.gameObject.SetActive(false);
        //    Invoke(nameof(ControlWarning), 2f);
        //}
    }

    public void SendResult()
    {
        logManager.WriteToLogFile("Survey Result:");
        foreach(ToggleGroupManager t in toggleGroupM)
        {
            logManager.WriteToLogFile(t.Question + ": " + t.SelectedAnswer);
        }

        gameManager.GoToNextLevel();
    }
    public void ControlWarning()
    {
        warningBtn.gameObject.SetActive(false);
        SubmitBtn.gameObject.SetActive(true);
    }
}
