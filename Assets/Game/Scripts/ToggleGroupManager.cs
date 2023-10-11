using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ToggleGroupManager : MonoBehaviour
{
    public ToggleGroup toggleGroupInstance;

    public TMP_Text questionText;

    private string question;

    private string selectedAnswer;


    public Toggle currentSelection
    {
        get { return toggleGroupInstance.ActiveToggles().FirstOrDefault(); }
    }

    public string Question { get => question; set => question = value; }
    public string SelectedAnswer { get => selectedAnswer; set => selectedAnswer = value; }

    // Start is called before the first frame update
    void Start()
    {
        // toggleGroupInstance = GetComponent<ToggleGroup>();
        //  Debug.Log("First selected: " + currentSelection.name);

        // SelectToggle(2);
        Question = questionText.text;
    }

    public void SelectToggle(int id)
    {
        var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();
        //  toggles[id].isOn = true;
        //Debug.Log("toggle " + toggles[id].name + " is on");
        //Debug.Log("current: " + currentSelection.name);
        // Debug.Log("question: " + questionText.text + " Answer: toggle " + toggles[id].name + ", name: " + toggles[id].GetComponentInChildren<TMP_Text>().text);
        SelectedAnswer = "toggle index: " + toggles[id].name + ", name: " + toggles[id].GetComponentInChildren<TMP_Text>().text;    
    }

    public void SelectToggle(string answer)
    {
        var toggles = toggleGroupInstance.GetComponentsInChildren<Toggle>();

    }

    public void SendAnswer()
    {
        if(currentSelection!= null)
        {
            Debug.Log(Question + ": " + SelectedAnswer + " current: " + currentSelection.name);
        }
        else
        {
            Debug.Log("Please select an answer");
        }
      
    }

    public bool CheckAnswered()
    {
        return currentSelection != null;
    }
}

