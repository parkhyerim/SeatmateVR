using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioClip clipCardForward;
    public AudioClip clipCardBackward;
    public AudioClip clipCardMatch;
    public AudioClip clipCardDismatch;

    [Header("EFFECT")]
    public GameObject matchEffectPrefab;
    public GameObject matchEffectPrefab2;
    public GameObject tunnelEffectPrefab1;
    public GameObject tunnelEffectPrefab2;
    public GameObject starEffectPrefab;

    [Header("GAME UI")]
    public GameObject menuUICanvas;
    public TMP_Text gameScoreText;
    public TMP_Text gameTimeText;
    // public GameObject interactionUI;
    public TMP_Text instructionText;
    public GameObject notificationCanvas;
    public TMP_Text notificationText;
    public Image notificationBGImage;
    public List<Image> notificationCheerImages;
    public GameObject surveryUICanvas;

    [Header("CARDs")]
    public MemoryCard[] allCards;
    private List<Vector3> allPositionsOfCards = new List<Vector3>();
    private Vector3 AngleOfCards = new Vector3();
    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;
    private bool canClickCard = true;
    private bool isFrontCard = false;

    [Header("TIME MANAGEMENT")]
    public float memorizingTime;
    public float bufferBeforeStartingGame = 2f;
    public int totalGameTime;
    //[SerializeField]
    float gameCountTimer;
    public float gameCountTimerIgnoringPause;
   // [SerializeField]
    int gameTimer;
    private float startShowingCardsInSec, startHidingCardAgainInSec; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    public float BystanderStartTime = 25f;

    // public Button pauseBtn;
    [SerializeField]
    private float pausedTime, identificationTime, eyeFocusedTime;
    bool gameIsPaused;

    [Header("SCORE")]
    //[SerializeField]
    private int score;
    private bool canStartGame;
    private bool canPauseGame;

    [Header("TRACKER")]
    public RotateTracker bysTracker;
    public XRInteractorLineVisual lineVisual;
    public string participantID;

    private int randomNumForEffect;
    private bool bystanderInteract;
    public LogManager logManager;
    public bool isPracticeGame;
    public bool isEndScene;
    private bool recordScore;
    int currentLevelIndex;

    //public string participantID = null;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }

    private void Awake()
    {       
        //Get all card positions and save in the list
        foreach(MemoryCard card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);

            //To make all cards uninteractable
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
        }
        
        AngleOfCards = allCards[0].transform.localEulerAngles;

        //Randomize the positions of the cards
        System.Random randomNumber = new System.Random();
        allPositionsOfCards = allPositionsOfCards.OrderBy(position => randomNumber.Next()).ToList();
    
        //Assign a new position to all cards
        for(int i = 0; i < allCards.Length; i++)
        {
            allCards[i].transform.position = allPositionsOfCards[i];
        }

        //Time Management
        gameTimer = totalGameTime;

        //Score
        score = 0;
        gameScoreText.text = "";
        gameTimeText.text = "";

        //UI Elements deactivated
        notificationCanvas.gameObject.SetActive(false);
        surveryUICanvas.gameObject.SetActive(false);

        foreach(Image img in notificationCheerImages)
        {
            img.enabled = false;
        }

        // Pause/Resume the game by clicking the panel
        //  interactionUI.SetActive(false);
        // pauseBtn.gameObject.SetActive(false);

        if (participantID == "" || participantID == null)
        {
            DateTime localDate = DateTime.Now;
            string cultureName = "de-DE"; // de-DE  en-GB en-US
            var culture = new CultureInfo(cultureName);
            string name = localDate.ToString(culture);
            participantID = "not assigned";
           // Debug.Log("participant name: " + participantID);
        }      
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {
            if (Time.time >= startShowingCardsInSec 
                && Time.time <= startHidingCardAgainInSec) // Showing Time
            {         
                //if(Time.time == hideCardAgainInSec)
                //{
                //    gameProcessBackground.enabled = true;
                //    gameProcessText.text = "Match a pair of cards!";
                //}

                //gameProcessBackground.enabled = false;
                //gameProcessText.text = "";

                beforeGameTimer += Time.fixedDeltaTime;
                gameTimeText.text = Math.Round(memorizingTime - beforeGameTimer).ToString();
            
                if (isFrontCard == false)
                {
                    ShowCards();
                    isFrontCard = true;
                }
            }
            else if (Time.time > startHidingCardAgainInSec && GameCountTimer <= totalGameTime) // During the Game
            {
                gameCountTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gameIsPaused)
                {
                    GameCountTimer += Time.fixedDeltaTime; 
                    gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    if (Math.Round(GameCountTimer) == totalGameTime) //The time is up
                    {
                        StopRayInteractoin();
                        EndGame();
                    }
                }
                else
                {
                    gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                }
            }
        }
    }

    public void StartGame()
    {
        CanStartGame = true;
        startShowingCardsInSec = Time.time + bufferBeforeStartingGame;
        startHidingCardAgainInSec = startShowingCardsInSec + memorizingTime;
        Destroy(menuUICanvas);

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Study Order: " + currentLevelIndex + " , name: " + currentSceneName);

        Instantiate(tunnelEffectPrefab1);
        Instantiate(tunnelEffectPrefab2);
    }

    public void SetAvatarTimeStamp()
    {
        string curDateTime = GetCurrentTime();
        logManager.WriteToLogFile("Bystander wants to interact: " + (float)Math.Round(gameCountTimerIgnoringPause) + " [" + curDateTime + "]");
    }

    public void ShowCards()
    {             
        Vector3 frontLocalEulerAngles = new Vector3(0, 0, 0);

        foreach(MemoryCard card in allCards) {
            card.transform.localEulerAngles = frontLocalEulerAngles;
        }

        instructionText.text = "Match Pairs by Clicking Two Cards!";

        Invoke("HideCards", time: memorizingTime);
        Invoke(nameof(showNotification), time: memorizingTime - 0.8f);
    }

    public void HideCards() {
        notificationBGImage.enabled = false; // TODO: notificationCanvas is parent
        notificationText.enabled = false;
        instructionText.text = "";
    
        gameScoreText.text = "0/20";
        Vector3 backsidelocEulerAngles = new Vector3(0, 180, 0);

        foreach(MemoryCard card in allCards) {
            card.IsGameStart = true;
            card.transform.localEulerAngles = backsidelocEulerAngles;
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        }

        CanPauseGame = true;
        //isFront = false;
        Invoke(nameof(BystanderStart), time: BystanderStartTime);
       // interactionUI.SetActive(true);
      //  pauseBtn.gameObject.SetActive(true);
    }
    public void showNotification()
    {
        notificationCanvas.SetActive(true);
        notificationBGImage.enabled = true;
        notificationText.text = "GAME START!";
    }
    public void BystanderStart()
    {
        gameTimeText.text = "";
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        // logManager
    }
 
    public void CardClicked(MemoryCard card)
    {
        if (canClickCard == false || card == firstSelectedCard)
        {
            return;
        }
            
        // Always rotate card forwards to show its image
       // card.transform.localEulerAngles = new Vector3(0,0,0);
        card.targetHeight = 0.05f;
        card.targetRotation = 0;

        audioSource.PlayOneShot(clipCardForward);

        if(firstSelectedCard == null)
        {
            firstSelectedCard = card;
        }
        else
        {
            // Second card selected;
            secondSelectedCard = card;
            canClickCard = false;
            // 1 second later
            Invoke(nameof(CheckMatch), time: 1f); ;         
        }
    }

    public void CheckMatch()
    {      
        // RESULT
        if (firstSelectedCard.identifier == secondSelectedCard.identifier)
        {
            Instantiate(matchEffectPrefab, firstSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab, secondSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab2, firstSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab2, secondSelectedCard.gameObject.transform.position, Quaternion.identity);
            Destroy(firstSelectedCard.gameObject);
            Destroy(secondSelectedCard.gameObject);
            score += 2;
            gameScoreText.text = score.ToString() + "/20";

            //if ((score % 4 == 0) && score!= 20)
            //{
                // notificationCheerImage.enabled = true;
                StartCoroutine("ShowRandomImage");
                randomNumForEffect = UnityEngine.Random.Range(0, notificationCheerImages.Count);
                notificationCheerImages[randomNumForEffect].enabled = true;
                notificationCheerImages[randomNumForEffect].transform.position = firstSelectedCard.gameObject.transform.position;
            //}

            audioSource.PlayOneShot(clipCardMatch);
            if(score == 20)
            {
                StopRayInteractoin();
                Invoke(nameof(EndGame), 2);
            }
        }
        else
        {
            firstSelectedCard.targetRotation = 180;
            secondSelectedCard.targetRotation = 180;
            audioSource.PlayOneShot(clipCardDismatch);
        }

        // RESET
        firstSelectedCard = null;
        secondSelectedCard = null;

        audioSource.PlayOneShot(clipCardBackward);

        canClickCard = true;
    }

    public void PauseGame()
    {
        if (!gameIsPaused)
        {
            gameIsPaused = true;
            pausedTime = (float)Math.Round(GameCountTimer);
            identificationTime = (float)Math.Round(gameCountTimerIgnoringPause);
            logManager.WriteToLogFile("Identification (Paused) Time: " + identificationTime);
            StopRayInteractoin();
        }
        else
        {
            gameIsPaused = false;
            logManager.WriteToLogFile("Resume Time: " + (float)Math.Round(gameCountTimerIgnoringPause));
            StartRayInteraction();
        }
    }

    public void EndGame()
    {
        notificationCanvas.SetActive(true);
        notificationBGImage.enabled = true;
        notificationText.enabled = true;
        bystanderInteract = false;
        CanPauseGame = false;
        //foreach (MemoryCard card in allCards)
        //{
           
        //        Destroy(card);
          
        //}

        if (score == 20)
            notificationText.text = "BRAVO!\nYOU WIN!";
        else
            notificationText.text = "GAME OVER!";

        if (!recordScore)
        {
            logManager.WriteToLogFile("Score: " + score);
            logManager.WriteToLogFile("==============================");
            recordScore = true;
        }

       // tunnelEffectPrefab2.SetActive(false);
       // tunnelEffectPrefab1.SetActive(false);

       // Invoke(nameof(GoToNextLevel), 2f);
        Invoke(nameof(DoSurvey), 1f);
    }

    public void GoSurvey()
    {
        surveryUICanvas.SetActive(true);
    }
    public void DoSurvey()
    { 
        surveryUICanvas.SetActive(true);
        lineVisual.enabled = true;
        
        //lineVisual.gameObject.SetActive(true);
        notificationCanvas.SetActive(false);
        notificationBGImage.enabled = false;
        notificationText.enabled = false;
        // menuUICanvas.SetActive(false);

        foreach (MemoryCard card in allCards)
        {
            if (card != null)
                card.gameObject.SetActive(false);
        }
    }

    public void GoToNextLevel() {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LoadNextLevel();
    }

    public IEnumerator ShowRandomImage()
    {
        while (true)
        {   
            yield return new WaitForSeconds(1);
            notificationCheerImages[randomNumForEffect].enabled = false;
        }
    }


    //public void WriteToLogFile(string message)
    //{
    //    using (System.IO.StreamWriter logFile = 
    //        new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID +".txt", append:true))
    //    {
    //        logFile.WriteLine(DateTime.Now + message);       
    //    }  
    //}

    void StopRayInteractoin()
    {
        foreach (MemoryCard card in allCards)
        {
            if (card != null)
            {
                card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
              //  card.gameObject.SetActive(false);
            }
        }

       // Destroy(lineVisual);
         lineVisual.enabled = false;
      //  lineVisual.gameObject.SetActive(false);
    }

    void StartRayInteraction()
    {
        foreach (MemoryCard card in allCards)
        {
            if (card != null)
                card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        }
        lineVisual.enabled = true;
    }

    public void SubmitSurvey()
    {
        Debug.Log("submit survey");
    }

    public void EyeFocused()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        Debug.Log("Eye focused: " + name);
        eyeFocusedTime = (float)Math.Round(gameCountTimerIgnoringPause);
        logManager.WriteToLogFile("Eye Focused Time: " + eyeFocusedTime);
    }

    public string GetCurrentTime()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        return name;
    }
}
