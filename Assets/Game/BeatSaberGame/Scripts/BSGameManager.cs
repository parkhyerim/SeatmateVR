using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class BSGameManager : MonoBehaviour
{
    public GameObject panelForAnimoji;
    public GameObject panelForTime;
    public GameObject panelForScore;
    public GameObject panelForAvatar;
    public GameObject panelForCubeArea;

    private Transform animojiGazeTransform;
    private Transform avatarGazeTransform;
    private Transform scoreGazeTransform, timerGazeTransform, cubeAreaGazeTransform;

    [Header("User-Study Settings")]
    public bool isTrialGame;
    public bool isBaseline;
    public bool isAnimojiSetting;
    public bool isAvatarSetting;
    public bool isMixedSetting;
    private bool[] settingArray;
    public int[] audioOrder = { 1, 2, 3, 4 };

    [Header("AUDIO")]
    public AudioSource gameEffectAudioSource;
    public AudioSource bgMusicAudioSource;
    public AudioSource lobbyMusicAudioSource;
    public AudioSource quesitionAudioSource;
    public AudioClip rightSound;
    public AudioClip missCubeSound;
    public AudioClip sliceSound;
    public AudioClip cheerSound;
    // public AudioClip[] questionAudios;

    [Header("TIME INPUT")]
    public int totalGameTime = 180;  // default
    public float getReadyTime = 1f;  // default
    public float BystanderStartTime = 25f; // default
    public float bystanderInterval = 40f;  // default
    public float getReadyTimeForTrial = 9f; // default

    [Header("TIME INFO.")]
    private float gameTimerIgnoringPause, gameCountTimer;
    int gameTimerToZero; // for timer UI
    private float timeFromSceneLoading, startTimeForSpawningCubes; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    [SerializeField]
    private float BystanderStartTime2, BystanderStartTime3, BystanderStartTime4;
    private float pausedTime, identificationTimeRound, eyeFocusTime, resumedTime, pauseDuration;
    private int pauseCounter, pauseInErrorCounter, pauseInInteractionCounter;

    [Header("GAME EFFECT")]
    public GameObject cheerEffect;
    public GameObject blueEffect;
    public GameObject greenEffect;
   // public GameObject yellowEffect;

    [Header("GAME UI")]
    public GameObject lobbyMenuUI;
    public GameObject instructionUI;
    public GameObject timeUI;
    public GameObject scoreUI;
    private TMP_Text gameScoreText;
    private TMP_Text gameTimeText;
    private TMP_Text instructionText;

    [Header("TRIAL_UI")]
    public GameObject trialLobbyMenuUI;
    public GameObject trialInstructionUI;
    public GameObject trialProcessUI;
    private TMP_Text trialInstructionText;
    private TMP_Text trialLobbyText;
    public GameObject trialStartButton;

    [Header("GAME COMPONENTS")]
    public GameObject saberObject;

    [Header("SCORE")]
    [SerializeField]
    private int score;

    [Header("BOOLEADN FOR GAME")]
    [SerializeField]
    private bool canStartGame;
    [SerializeField]
    private bool canPauseGame;
    [SerializeField]
    private bool gamePaused;
    [SerializeField]
    private bool gameResumed;
    [SerializeField]
    private bool canStartTrial;
    [SerializeField]
    private bool canPauseTrial;
        
    public XRInteractorLineVisual lineVisual;

    private string participantID;
    public bool isEndScene;
    private bool recordScoreAndTime, recordMaxMin, recordStartAxis;
    int currentSceneIndex;
    private bool askSpawnCubes;
    //Trial
    private bool pauseInstructed;
    private bool trialOncePaused, trialOnceResumed;

    [Header("CUBES")]
    public CubeSpawner cubeSpawner;
    public CubeSpawner trialCubeSpawner;
    private GameObject[] cubes;
    private GameObject[] trialCubes;
    private bool askSpawnCubesForTrial;

    private string instructionMsg;

    BSRotateTracker bysTracker;
    private bool bystanderInteract;
    BSBystanderAvatar bystanderAvatar;
    BSLevelManager levelManager;
    UserStudyManager userstudyManager;
    BSLogManager logManager;
    BSPauseController pauseController;
    HeadMovement headMovement;
    TimeLog timeLog;
    SocketManager socketManager;

    private float currentYAxis, diffYAxis, previousYAxis;
    private float checkPointTime = 0.0f;
    public float period = 0.2f;
    private Vector3 cameraAxis;
    private Vector3 maincameraAxisVector, maxLeftVectorAxis, maxRightVecotorAxis, maxUpVectorAxis, maxDownVectorAxis;
    private float mainCameraYAxis, mainCameraXAxis, mainCameraZAxis, maxRightAxis, maxLeftAxis, maxDownAxis, maxUpAxis;
    private bool oneInteruption;
    private bool bystanderCanHearAnswer;
    bool questionStart;

    int questionCounter;
    bool allQuestionAsked, reduceGameTime, calledPushEnd;
    bool firstPauseCalled, secondPauseCalled, thirdPauseCalled, fourthPauseCalled, doVisualizing;

    // Gaze
    public bool gazeAnimoji, gazeAvatar, gazeUI, gazeScore, gazeTimer, gazeCubes;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public bool CanPauseTrial { get => canPauseTrial; set => canPauseTrial = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }
    public bool BystanderCanHearAnswer { get => bystanderCanHearAnswer; set => bystanderCanHearAnswer = value; }
    public bool CanStartTrial { get => canStartTrial; set => canStartTrial = value; }
    public int Score { get => score; set => score = value; }
    public float MaxRightAxis { get => maxRightAxis; set => maxRightAxis = value; }
    public float MaxLeftAxis { get => maxLeftAxis; set => maxLeftAxis = value; }
    public float MaxUpAxis { get => maxUpAxis; set => maxUpAxis = value; }
    public float MaxDownAxis { get => maxDownAxis; set => maxDownAxis = value; }
    public bool GamePaused { get => gamePaused; set => gamePaused = value; }
    public bool DoVisualising { get => doVisualizing; set => doVisualizing = value; }
    public Transform AnimojiGazeTransform { get => animojiGazeTransform; set => animojiGazeTransform = value; }
    public Transform AvatarGazeTransform { get => avatarGazeTransform; set => avatarGazeTransform = value; }
    public bool QuestionStart { get => questionStart; set => questionStart = value; }

    /**************************************************************
     * socket code
     **************************************************************/

    private void Awake()
    {
        // Create references to other game objects and components
        levelManager = FindObjectOfType<BSLevelManager>();
        userstudyManager = FindObjectOfType<UserStudyManager>();
        logManager = FindObjectOfType<BSLogManager>();
        pauseController = FindObjectOfType<BSPauseController>();
        bystanderAvatar = FindObjectOfType<BSBystanderAvatar>();
        bysTracker = FindObjectOfType<BSRotateTracker>();
        headMovement = FindObjectOfType<HeadMovement>();
        timeLog = FindObjectOfType<TimeLog>();
        socketManager = FindObjectOfType<SocketManager>();

        // GAME
        instructionText = instructionUI.GetComponentInChildren<TMP_Text>();
        gameScoreText = scoreUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        gameTimeText = timeUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        //TRIAL
        trialLobbyText = trialLobbyMenuUI.GetComponentInChildren<TMP_Text>();
        trialInstructionText = trialInstructionUI.GetComponentInChildren<TMP_Text>();

        // Game Notification
        instructionUI.SetActive(false);
        trialInstructionUI.SetActive(false);
        trialProcessUI.SetActive(false);
        // surveryUI.gameObject.SetActive(false);
        timeUI.SetActive(false);
        scoreUI.SetActive(false);

        // Default setting: Avatar setting
        if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting || isBaseline || isTrialGame))
            isTrialGame = true;

        settingArray = new bool[] { isTrialGame, isBaseline, isAnimojiSetting, isAvatarSetting, isMixedSetting };

        bool isOneSettingselected = CheckOneConditionSelected();
        if (!isOneSettingselected)
        {
            isTrialGame = true;
            isBaseline = false;
            isAnimojiSetting = false;
            isAvatarSetting = false;
            isMixedSetting = false;
        }
     
        if (!isTrialGame)
        {
            trialLobbyMenuUI.SetActive(false);         
        }
        else // Practice game
        {
            lobbyMenuUI.SetActive(false);
            // trialStartButton.SetActive(false);
        }

        if (isAnimojiSetting)
        {
            panelForAvatar.SetActive(false);
            animojiGazeTransform = panelForAnimoji.gameObject.transform;


        }
        else if (isAvatarSetting)
        {
            panelForAnimoji.SetActive(false);
            avatarGazeTransform = panelForAnimoji.gameObject.transform;
        }
        else if (isMixedSetting)
        {
            animojiGazeTransform = panelForAnimoji.gameObject.transform;
            avatarGazeTransform = panelForAnimoji.gameObject.transform;
        }
        else if (isBaseline)
        {
            panelForAvatar.SetActive(false);
            panelForAnimoji.SetActive(false);
        }

        saberObject.SetActive(false);

        participantID = userstudyManager.GetParticipantID();

        socketManager.setupSocket();
       
    }

    private void Start()
    {
        bystanderAvatar.SetUserstudyCondition();
        gameTimerToZero = totalGameTime; // set time for the game e.g., 150
        score = 0;

        // Interruption Time of Bystander
        BystanderStartTime2 = BystanderStartTime + bystanderInterval;
        BystanderStartTime3 = BystanderStartTime2 + bystanderInterval;
        BystanderStartTime4 = BystanderStartTime3 + bystanderInterval;

        // X, Y Y-Axis of VR User
        maincameraAxisVector = Camera.main.transform.eulerAngles;
        mainCameraYAxis = maincameraAxisVector.y;
        mainCameraXAxis = maincameraAxisVector.x;
        mainCameraZAxis = maincameraAxisVector.z;

        // basic values
        MaxUpAxis = mainCameraXAxis;
        MaxDownAxis = mainCameraXAxis;
        MaxLeftAxis = mainCameraYAxis;
        MaxRightAxis = mainCameraYAxis;

        if (participantID == "" || participantID == null)
            participantID = "IDNotAssigned";

        // TRIAL_GAME
        if (isTrialGame)
        {
            trialLobbyText.text = "";
            StartCoroutine(WelcomeInstruction());
        }
        //else
        //{
        //    setupSocket();
        //}

        if (isAnimojiSetting)
        {
            panelForAvatar.SetActive(false);
            animojiGazeTransform = panelForAnimoji.gameObject.transform;
           
            
        }
        else if (isAvatarSetting)
        {
            panelForAnimoji.SetActive(false);
            avatarGazeTransform = panelForAnimoji.gameObject.transform;
        }
        else if (isMixedSetting)
        {
            animojiGazeTransform = panelForAnimoji.gameObject.transform;
            avatarGazeTransform = panelForAnimoji.gameObject.transform;
        }
        else if (isBaseline)
        {
            panelForAvatar.SetActive(false);
            panelForAnimoji.SetActive(false);
        }
             
        scoreGazeTransform = panelForScore.gameObject.transform;
        timerGazeTransform = panelForTime.gameObject.transform;
        cubeAreaGazeTransform = panelForCubeArea.gameObject.transform;

        //Debug.Log("gaze for animoji position: " + animojiGazeTransform.position);
        //Debug.Log("gaze for avatar position: " + avatarGazeTransform.position);
        //Debug.Log("gaze for score: " + scoreGazeTransform.position);
        //Debug.Log("gaze for timer: " + timerGazeTransform.position);
        //Debug.Log("gaze for cube area: " + cubeAreaGazeTransform.position);
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {

            if (isAnimojiSetting)
            {
                animojiGazeTransform = panelForAnimoji.gameObject.transform;
            }
            else if (isAvatarSetting)
            {
                avatarGazeTransform = panelForAnimoji.gameObject.transform;
            }
            else if (isMixedSetting)
            {
                animojiGazeTransform = panelForAnimoji.gameObject.transform;
                avatarGazeTransform = panelForAnimoji.gameObject.transform;
            }
          
            maincameraAxisVector = Camera.main.transform.eulerAngles;

            if (maincameraAxisVector.y > 180 && maincameraAxisVector.y <= 360) // 360-> 270-> 179 => 0-> -90 -> -179
            {
                mainCameraYAxis = maincameraAxisVector.y - 360f;
            }
            if (maincameraAxisVector.y > 0 && maincameraAxisVector.y <= 180) // 1-> 90-> 180 => 1 -> 90 -> 180
            {
                mainCameraYAxis = maincameraAxisVector.y;
            }
            if (maincameraAxisVector.x > 180 && maincameraAxisVector.x <= 360)
            {
                mainCameraXAxis = maincameraAxisVector.x - 360f;
            }
            if (maincameraAxisVector.x > 0 && maincameraAxisVector.x <= 180)
            {
                mainCameraXAxis = maincameraAxisVector.x;
            }
            if (maincameraAxisVector.z> 180 && maincameraAxisVector.z <= 360)
            {
                mainCameraZAxis = maincameraAxisVector.z - 360f;
            }
            if (maincameraAxisVector.z > 0 && maincameraAxisVector.z <= 180)
            {
                mainCameraZAxis = maincameraAxisVector.z;
            }
            // Set Max. & Min. Value
            if (MaxRightAxis < mainCameraYAxis) // against bystander: 0 <-> 90
            {
                MaxRightAxis = mainCameraYAxis;
                maxRightVecotorAxis = maincameraAxisVector;
            }

            if (MaxLeftAxis > mainCameraYAxis) // towards bystander: -90 <-> 0
            {
                MaxLeftAxis = mainCameraYAxis;
                maxLeftVectorAxis = maincameraAxisVector;
            }

            if (MaxDownAxis < mainCameraXAxis) // head down: 0 <-> 90
            {
                MaxDownAxis = mainCameraXAxis;
                maxDownVectorAxis = maincameraAxisVector;
            }

            if (MaxUpAxis > mainCameraXAxis) // head up: 0 <-> -90
            {
                MaxUpAxis = mainCameraXAxis;
                maxUpVectorAxis = maincameraAxisVector;
            }
            // Head Movement
            if (!recordStartAxis)
            {
                LogCameraAxisAtStart();
                recordStartAxis = true;
            }

            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawningCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                 gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
               // gameTimeText.text = ConvertToMinAndSeconds(getReadyTime - beforeGameTimer);

            }
            // GAME TIME
            else if (Time.time > startTimeForSpawningCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                //// Set Max. & Min. Value
                //if (MaxRightAxis < mainCameraYAxis) // against bystander: 0 <-> 90
                //{
                //    MaxRightAxis = mainCameraYAxis;
                //    maxRightVecotorAxis = maincameraAxisVector;
                //}
                   
                //if (MaxLeftAxis > mainCameraYAxis) // towards bystander: -90 <-> 0
                //{
                //    MaxLeftAxis = mainCameraYAxis;
                //    maxLeftVectorAxis = maincameraAxisVector;
                //}                

                //if (MaxDownAxis < mainCameraXAxis) // head down: 0 <-> 90
                //{
                //    MaxDownAxis = mainCameraXAxis;
                //    maxDownVectorAxis = maincameraAxisVector;               
                //}
                   
                //if (MaxUpAxis > mainCameraXAxis) // head up: 0 <-> -90
                //{
                //    MaxUpAxis = mainCameraXAxis;
                //    maxUpVectorAxis = maincameraAxisVector;
                //}
            
                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                     gameTimeText.text = Math.Round(gameTimerToZero - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                   // gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                   // gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);

                    if (!askSpawnCubes)
                    {
                        SpawnCubes();
                        askSpawnCubes = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                        //cubeSpawner.CanSpawn = false;
                       // StopRayInteractoin();
                        EndGame();
                    }

                    //if (reduceGameTime && !calledPushEnd)
                    //{
                    //    cubeSpawner.CanSpawn = false;
                    //    StopRayInteractoin();
                    //    EndGame();
                    //    calledPushEnd = true;
                    //}
                }
                else
                {
                     gameTimeText.text = Math.Round(gameTimerToZero - GameCountTimer).ToString();
                  //  gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                   // gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);
                }
            }
        }

        if (isTrialGame && canStartTrial)
        {
            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawningCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
              //  gameTimeText.text = ConvertToMinAndSeconds(getReadyTimeForTrial - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawningCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                    gameTimeText.text = Math.Round(gameTimerToZero - GameCountTimer).ToString();
                   // gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                  //  gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);

                    if (askSpawnCubesForTrial == false)
                    {
                        SpawnCubesForTrial();
                        askSpawnCubesForTrial = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                       // trialCubeSpawner.CanSpawn = false;
                        //StopRayInteractoin();
                        //EndGame();
                        EndTrial();
                    }
                }
                else
                {
                     gameTimeText.text = Math.Round(gameTimerToZero - GameCountTimer).ToString();
                   //  gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                    //gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);
                }
            }       
        }
    }


    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time; // Time.time returns the amount of time in seconds since the project started playing
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTime;

        headMovement.GameStart = true;
        timeLog.GameStart = true;
        
       // headMovement.InGame = true;

        // Music 
        lobbyMusicAudioSource.Stop();
        bgMusicAudioSource.Play();

        // UI GameObject 
        saberObject.SetActive(true);
        scoreUI.SetActive(true);
        timeUI.SetActive(true);
        Destroy(lobbyMenuUI);

        // Scene Management
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Logging Game-Start
        logManager.WriteLogForOverview("GAME START");
        logManager.WriteLogForYawHeadMovement("GAME START");
        logManager.WriteLogForPitchHeadMovement("GAME START");
        logManager.WriteLogForRollHeadMovement("GAME START");
        logManager.WriteLogForEyeGaze("GAME START");
        logManager.WriteLogForHeadPosition("GAME START");

        logManager.WriteLogForOverview("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForYawHeadMovement("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));        
        logManager.WriteLogForPitchHeadMovement("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForRollHeadMovement("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForEyeGaze("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForHeadPosition("CONDITION: " + currentSceneName);// + ", ORDER: " + (currentSceneIndex));
    }

    public void SetTimeStampForAvatarInCriticalZoneWithMessage(string state)
    {
        Debug.Log("BYSTANDER: " + state + " " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForOverview("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForYawHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) +" (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForPitchHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForRollHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForEyeGaze("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForHeadPosition("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) +" (" + gameTimerIgnoringPause + ")");
    }

    private void LogCameraAxisAtStart()
    {
        Vector3 cameraLocalPos = Camera.main.transform.localPosition;
        logManager.WriteLogForOverview("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis+ ", Vector3: (" + maincameraAxisVector.x + " ," + maincameraAxisVector.y +" ," + maincameraAxisVector.z +")");
        logManager.WriteLogForYawHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3: (" + maincameraAxisVector.x + " ," + maincameraAxisVector.y + " ," + maincameraAxisVector.z + ")");
        logManager.WriteLogForPitchHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3: (" + maincameraAxisVector.x + " ," + maincameraAxisVector.y + " ," + maincameraAxisVector.z + ")");
        logManager.WriteLogForRollHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3: (" + maincameraAxisVector.x + " ," + maincameraAxisVector.y + " ," + maincameraAxisVector.z + ")");
        logManager.WriteLogForHeadPosition("Head Movement [ORIGIN]: " + "X: " + cameraLocalPos.x + ", Y: " + cameraLocalPos.y + ", Z: " + cameraLocalPos.z + ", Vector3:" + cameraLocalPos);
    }
 
    public void SpawnCubes()
    {
        cubeSpawner.CanSpawn = true;
        CanPauseGame = true;

        instructionText.enabled = false;
        gameScoreText.text = "0";

        Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime);

        if (!oneInteruption)
        {
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime2);
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime3);
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime4);
        }
        // interactionUI.SetActive(true);
    }


    public void BystanderStartTurningToVRPlayer()
    {
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        pauseController.OncePausedInSession = false;
        
        // logManager
        logManager.WriteLogForOverview("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForEyeGaze("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForYawHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForPitchHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForRollHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForHeadPosition("BYSTANDER FROM starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        bystanderCanHearAnswer = true;
        bystanderAvatar.LookedOnceSeatedPosition = false;
        bystanderAvatar.IsGuidingFOVToSeatedExceed = false;
    }

    public void BystanderEnd()
    {
        BystanderInteract = false;
        if (questionCounter == 4)
        {
            Debug.Log("all question is asked");
            allQuestionAsked = true;
            Invoke(nameof(EndGame), 25f);
            //ChangeTotalTime();
        }
    }

    public void SliceCube(GameObject cube)
    {
        //  Debug.Log(cube.name + " called the Method");
        if (score % 8 == 0 && score > 0)
        {
            //ToDo: Short Effect
            gameEffectAudioSource.PlayOneShot(cheerSound);
            Instantiate(cheerEffect, cube.transform.position, Quaternion.identity);
        }
        else
        {
            gameEffectAudioSource.PlayOneShot(rightSound);
        }

        if (cube.name.Contains("Blue"))
        {
            gameEffectAudioSource.PlayOneShot(sliceSound);

            Instantiate(blueEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }
        else if (cube.name.Contains("Green"))
        {
            gameEffectAudioSource.PlayOneShot(sliceSound);

            Instantiate(greenEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }
        //else if (cube.name.Contains("Yellow"))
        //{
        //    gameEffectAudioSource.PlayOneShot(sliceSound);
        //    Instantiate(yellowEffect, cube.transform.position, Quaternion.identity);
        //    score += 1;
        //    gameScoreText.text = score.ToString();
        //}

        if (score % 10 == 0 && score > 0 && score < 30)
        {
            if(isTrialGame)
                ShowPauseHint();
        }

        
        //if(score % 5 == 0 && score > 30)
        //{
        //    Debug.Log("speed is higher");
        //    trialCubeSpawner.beat = 0.8f;
        //}
        //if(score > 20 && score % 25 == 0 && score < 30)
        //{
        //    ShowFinishHint();
        //}

    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            if (!isTrialGame)
            {
                gamePaused = true;
                pauseCounter++;
                if (!bystanderAvatar.InVisualization)
                    pauseInErrorCounter++;
                else
                    pauseInInteractionCounter++;
                //Debug.Log(pauseCounter + " " + pauseInErrorCounter + " " + pauseInInteractionCounter);
                pausedTime = gameTimerIgnoringPause;
                identificationTimeRound = (float)Math.Round(gameTimerIgnoringPause);
                logManager.WriteLogForOverview("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForYawHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("IDENTIFICATION (Paused) TIME: " + identificationTimeRound + " (" + gameTimerIgnoringPause + ")");
                              
                cubeSpawner.CanSpawn = false;
                cubeSpawner.StopMoving = true;
                cubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in cubes)
                {
                    cube.GetComponent<Cube>().StopMove();
                }
                StopRayInteractoin();
            }
            // Practice Game
            else
            {
                gamePaused = true;
                trialCubeSpawner.CanSpawn = false;
                trialCubeSpawner.StopMoving = true;
                trialCubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in trialCubes)
                {
                    cube.GetComponent<Cube>().StopMove();
                }
                StopRayInteractoin();
                if (!trialOncePaused)
                {
                    instructionMsg = "You paused the game.\n Try to resume the game by pressing the trackpad again!";
                    //trialInstructionText.text = instructionMsg;
                    trialInstructionText.text = instructionMsg;
                    trialOncePaused = true;
                }
                else
                {
                    instructionMsg = "You paused the game.\n Try to resume the game!";
                    trialInstructionText.text = instructionMsg;
                }      
            }
        }
        else
        {
            if (!isTrialGame)
            {
                gamePaused = false;
                resumedTime = gameTimerIgnoringPause;
                pauseDuration = resumedTime - pausedTime;
                logManager.WriteLogForOverview("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause +")");
                logManager.WriteLogForYawHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForOverview("DURATION: " + pauseDuration);
                logManager.WriteLogForPitchHeadMovement("DURATION: " + pauseDuration);
                logManager.WriteLogForRollHeadMovement("DURATION: " + pauseDuration);
                logManager.WriteLogForYawHeadMovement("DURATION: " + pauseDuration);
                logManager.WriteLogForHeadPosition("DURATION: " + pauseDuration);
                logManager.WriteLogForEyeGaze("DURATION: " + pauseDuration);

                cubeSpawner.CanSpawn = true;
                cubeSpawner.StopMoving = false;
                cubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in cubes)
                {
                    cube.GetComponent<Cube>().StartMove();
                }
                StartRayInteraction();
            }
            else
            {
                gamePaused = false;
               // logManager.WriteLogFile("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
                trialCubeSpawner.CanSpawn = true;
                trialCubeSpawner.StopMoving = false;
                trialCubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in trialCubes)
                {
                    //  Debug.Log(cube.name);
                    cube.GetComponent<Cube>().StartMove();
                }
                if (!trialOnceResumed)
                {
                    instructionMsg = "You resumed the game! \n You can play the game again!";
                    trialInstructionText.text = instructionMsg;
                    trialOnceResumed = true;
                    StartCoroutine(CleanInstructionBoard());
                }
                else
                {
                    instructionMsg = "You resumed the game.";
                    trialInstructionText.text = instructionMsg;
                    StartCoroutine(CleanInstructionBoard());
                }

               // trialInstructionText.text = instructionMsg;
                trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
                // notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
               // StartCoroutine(InstructionForPause());
                StartRayInteraction();
            }
        }
    }

    public void EndGame()
    {
        trialInstructionUI.SetActive(false);
        StopRayInteractoin();
        cubeSpawner.CanSpawn = false;
        float totalPlayTime = gameTimerIgnoringPause;
       // Debug.Log("END gametimeingnoringpause: " + totalPlayTime);

       // logManager.WriteLogFile("Total Game Time: " + totalPlayTime);
        if (!isTrialGame)
        {
            //writeSocket("endscript");
            //closeSocket();

            instructionUI.SetActive(true);
            instructionText.enabled = true;
            bystanderInteract = false;
            CanPauseGame = false;
            CanPauseTrial = false;
            headMovement.GameStart = false;
            headMovement.GameEnd = true;
            timeLog.GameStart = false;
            // headMovement.InGame = false;
            

            float avgYawHMValue = headMovement.GetYawHeadMovement();
            float avgPitchHMValue = headMovement.GetPitchHeadMovement();
            float avgRollValue = headMovement.GetRollHeadMovement();

            cubeSpawner.CanSpawn = false;
            cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject cube in cubes)
            {
                // Debug.Log(cube.name + " Destrpy");
                Destroy(cube);
            }
            saberObject.SetActive(false);
            trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
            //
            scoreUI.SetActive(false);
            timeUI.SetActive(false);

            instructionText.text = "BRAVO!\nYOUR SCORE IS " + score + "!\n Please take off your heaset  and fill out a questionnaire.";


            gameTimeText.text = 0.ToString();
            // gameTimeText.text = ConvertToMinAndSeconds(0);

            int totalScore = cubeSpawner.GetCountCubes();

            if (!recordScoreAndTime)
            {
                int missedPause = 4 - pauseInInteractionCounter;
                logManager.WriteLogForOverview("Score: " + score + " /" + totalScore);
                logManager.WriteLogForOverview("Total Game Time: " + totalPlayTime);
                logManager.WriteLogForOverview("Pause In Total: " + pauseCounter  + ", Pause during Notification: " + pauseInInteractionCounter + ", Error: " + pauseInErrorCounter + ", Missed: " + missedPause);
                recordScoreAndTime = true;
            }

            if (!recordMaxMin)
            {
                //logManager.WriteLogFile("Max Y Axis (Toward Bystander): " + maxYAxis + " Vector: " + maxYVectorAxis);
                //logManager.WriteLogFile("Min Y Axis (Against Bystander): " + minYAxis + " Vector: " + minYVecotorAxis);
                //logManager.WriteLogFile("Max X Axis: " + maxXAxis + " Vector: " + maxXVectorAxis);
                //logManager.WriteLogFile("Min X Axis: " + minXAxis + " Vector: " + minXVectorAxis);
                //logManager.WriteLogFile("================================\n=================");
                LogVRHeadsetAxis();
                logManager.WriteLogForOverview("Yaw(Y) Head Movement Avg. (every 0.20s): " + avgYawHMValue);
                logManager.WriteLogForOverview("Pitch(X) Head Movement Avg. (every 0.20s): " + avgPitchHMValue);
                logManager.WriteLogForOverview("Roll(Z) Head Movement Avg. (every 0.20s): " + avgRollValue);
                logManager.WriteLogForYawHeadMovement("Yaw(Y) Head Movement Avg. (every 0.20s): " + avgYawHMValue);
                logManager.WriteLogForPitchHeadMovement("Pitch(X) Head Movement Avg. (every 0.20s): " + avgPitchHMValue);
                logManager.WriteLogForRollHeadMovement("Roll(Z) Head Movement Avg. (every 0.20s): " + avgRollValue);

                logManager.WriteLogForOverview("END GAME");
                logManager.WriteLogForYawHeadMovement("END GAME");
                logManager.WriteLogForPitchHeadMovement("END GAME");
                logManager.WriteLogForRollHeadMovement("END GAME");
                logManager.WriteLogForHeadPosition("END GAME");
                logManager.WriteLogForEyeGaze("END GAME");
                logManager.WriteLogForOverview("============================================\n");
                logManager.WriteLogForYawHeadMovement("====================================================\n");
                logManager.WriteLogForPitchHeadMovement("===========================================\n");
                logManager.WriteLogForRollHeadMovement("=============================================\n");
                logManager.WriteLogForHeadPosition("============================================\n");
                logManager.WriteLogForEyeGaze("=============================================\n");
                recordMaxMin = true;
            }
            //GoToNextLevel();
            Invoke(nameof(GoToNextLevel), 4f);
            // Invoke(nameof(DoSurvey), 1f);

            //TODO:
            // levelManager.LoadGameOver();
        }
        //else
        //{
        //    CanPauseGame = false;
        //    CanPauseTrial = false;
        //    trialCubeSpawner.CanSpawn = false;
        //    saberObject.SetActive(false);
        //    instructionText.text = "Your Trial Game is finised!";
        //    gameTimeText.text = ConvertToMinAndSeconds(0);
        //    GoToNextLevel();
        //   // Invoke(nameof(GoToNextLevel), 5f);
        //}

        Debug.Log("EndGame");
        Debug.Log("Closing socket connection to python");
        socketManager.writeSocket("endscript");
        socketManager.closeSocket();
    }

    private void LogVRHeadsetAxis()
    {
        logManager.WriteLogForOverview("Head Movement [END]: Max Left(Y) (Toward Bystander): " + MaxLeftAxis + " Vector: (" + maxLeftVectorAxis.x +", " + maxLeftVectorAxis.y+ ", " + maxLeftVectorAxis.z+")");
        logManager.WriteLogForOverview("Head Movement [END]: Max Right(Y) (Against Bystander): " + MaxRightAxis + " Vector: (" + maxRightVecotorAxis.x + ", " + maxRightVecotorAxis.y + ", " + maxRightVecotorAxis.z + ")");
        logManager.WriteLogForOverview("Head Movement [END]: Max Up(X): " + MaxUpAxis + " Vector: (" + maxUpVectorAxis.x + ", " + maxUpVectorAxis.y + ", " + maxUpVectorAxis.z + ")");
        logManager.WriteLogForOverview("Head Movement [END]: Max Down(X): " + MaxDownAxis + " Vector: (" + maxDownVectorAxis.x + ", " + maxDownVectorAxis.y + ", " + maxDownVectorAxis.z + ")");
       // logManager.WriteLogFile("==========================================================");
    }


    private bool CheckOneConditionSelected()
    {
        int boolCount = 0;
        bool resultBool;
        foreach(bool element in settingArray)
        {
            if (element)
                boolCount++;
        }

        if (boolCount > 1)
            resultBool = false;
        else
            resultBool = true;

        return resultBool;
    }
  
    public void GoToNextLevel()
    {
        levelManager.LoadNextLevel();
    }

    void StopRayInteractoin()
    {
        lineVisual.enabled = false;
    }

    void StartRayInteraction()
    {
        lineVisual.enabled = true;
    }

    public void EyeFocused(bool focus, string visType)
    {
        if (BystanderInteract)
        {
            eyeFocusTime = (float)Math.Round(gameTimerIgnoringPause);
            if (focus)
            {
                if (visType.Contains("Animoji"))
                {
                    gazeAnimoji = true;
                    Debug.Log(visType + " " + gazeAnimoji);
                }
                else if (visType.Contains("Avatar"))
                {
                    gazeAvatar = true;
                    Debug.Log(visType + " " + gazeAvatar);
                }
                else if (visType.Contains("Cube"))
                {
                    gazeCubes = true;
                    Debug.Log(visType + " " + gazeCubes);
                }
                else if (visType.Contains("Timer"))
                {
                   // gazeTimer = true;
                    gazeUI = true;
                    Debug.Log(visType + " " + gazeTimer);
                }
                else if (visType.Contains("Score"))
                {
                   // gazeScore = true;
                    gazeUI = true;
                    Debug.Log(visType + " " + gazeScore);
                }
                // logManager.WriteLogFile("[" + visType +"] Receive FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("[" + visType + "] Receive FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
                
            }
            else
            {
                if (visType.Contains("Animoji"))
                {
                    gazeAnimoji = false;
                    Debug.Log(visType + " " + gazeAnimoji);
                }
                else if (visType.Contains("Avatar"))
                {
                    gazeAvatar = false;
                    Debug.Log(visType + " " + gazeAvatar);
                }
                else if (visType.Contains("Cube"))
                {
                    gazeCubes = false;
                    Debug.Log(visType + " " + gazeCubes);
                }
                else if (visType.Contains("Timer"))
                {
                   // gazeTimer = false;
                    gazeUI = false;
                    Debug.Log(visType + " " + gazeTimer);
                }
                else if (visType.Contains("Score"))
                {
                   // gazeScore = false;
                    gazeUI = false;
                    Debug.Log(visType + " " + gazeScore);
                }
                // logManager.WriteLogFile("[" + visType + "] LOST FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("[" + visType + "] LOST FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
            }
        }     
    }

    public string GetCurrentTime()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        return name;
    }

    private string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    public void MissCube()
    {
        // Debug.Log("missed ball");
        gameEffectAudioSource.PlayOneShot(missCubeSound);
    }

    public void AskQuestion()
    {
        Invoke(nameof(PlayQuestionAudio), 2f);
    }

    public void PlayQuestionAudio()
    {
        // default. counter = 0
        if (questionCounter < 5 )
        {
            questionCounter++; // 0-> 1,/ 1-> 2 /2-> 3/ 3->4   end


            //if (questionCounter == 0)
            //{
            //    // no question(1st visualisation)
            //}
            //else
            //{
            int index = audioOrder[questionCounter - 1] - 1;  // counter1,2,3 -> 0,1,2
                //quesitionAudioSource.PlayOneShot(questionAudios[index]);

                // socket
                Debug.Log(index+ " question is called");
                socketManager.writeSocket("question" + index);
            // Debug.Log(index + "question is called: " + (float)Math.Round(gameTimerIgnoringPause));
            // QuestionStart = true;
           
                timeLog.TimeStampForQuestionMoment((index+1).ToString());
                logManager.WriteLogForOverview("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForYawHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
            //}
          
        }
    }

    private void ShowPauseHint()
    {
        trialInstructionUI.SetActive(true);
        trialInstructionText.text = "Try to Pause the game.\n You can press the trackpad on the controller";
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        Debug.Log("pause instruction is called");
    }

    private void ShowFinishHint()
    {
        Debug.Log("Called Finish hint");
        trialInstructionUI.SetActive(true);
        trialInstructionText.text = "You can practice this game until you feel confident! \n You can click the Finish button below whenever you want to stop this tutorial.";
        StartCoroutine(CleanInstructionBoard());
    }


    /****************************************************************************************************
     * *********************************** TRIAL *******************************************************
     ****************************************************************************************************/
    //  TRIAL_GAME:2
    public void StartTrialIntroduction()
    {
        trialLobbyMenuUI.SetActive(false);

        trialInstructionUI.SetActive(true);
       
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;

        saberObject.SetActive(true);

        //foreach (GameObject cube in stopCubes)
        //{
        //    cube.SetActive(true);
        //}
        trialProcessUI.SetActive(true);

        CanStartTrial = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTimeForTrial; 


        StartCoroutine(InstructionsForCubeSlice());
    }

    public void StartTrialGame()
    {
        CanStartTrial = true;
        //score = 0;
        //foreach (GameObject cube in stopCubes)
        //{
        //    if (cube != null)
        //    {
        //        Destroy(cube);
        //    }
        //}

        timeFromSceneLoading = Time.time; 
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTimeForTrial; // getReadyTimeForTrial: longer than the main game
     

        //timeFromSceneLoading = Time.time;
        //startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial;
        trialInstructionText.text = "";


  
        //trialLobbyMenuUI.SetActive(false);
        //trialInstructionUI.SetActive(true);

        //saberObject.SetActive(true);
        //StartCoroutine(InstructionsForCubeSlice());
    }

    public void SpawnCubesForTrial()
    {
        trialCubeSpawner.CanSpawn = true;

        gameScoreText.text = "0";

      //  CanPauseGame = true;
        CanPauseTrial = true;
    }

    public void EndTrial()
    {
        StopRayInteractoin();
        trialCubeSpawner.CanSpawn = false;
        CanPauseGame = false;
        CanPauseTrial = false;
        trialCubeSpawner.CanSpawn = false;
        saberObject.SetActive(false);
        trialInstructionText.text = "Your Trial Game is finised!";
        // instructionText.text = "Your Trial Game is finised!";
        gameTimeText.text = 0.ToString();
       // gameTimeText.text = ConvertToMinAndSeconds(0);
        GoToNextLevel();
       // Invoke(nameof(GoToNextLevel), 5f);
    }

    // TRIAL_GAME: 1
    IEnumerator WelcomeInstruction()
    {
        instructionMsg = "Welcome to our user study!";
        trialLobbyText.text = instructionMsg;
        trialLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = false; // trackpad image
        yield return new WaitForSeconds(6);

        instructionMsg = "Please click the Start button below when you're ready." +
            "\n\n You can press the trigger button of the controller.";
        trialLobbyText.text = instructionMsg;
        trialLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = true; // trackpad image
        trialStartButton.SetActive(true);
        yield return new WaitForSeconds(0);
    }

    IEnumerator InstructionsForCubeSlice()
    {
        //instructionMsg = "You now hold a lightsaber in your right hand! \n\n " +
        //    "Slash the colored parts of the blocks in front of you with your saber! \n\n You don't need to press any button on the controller";
        //trialInstructionText.text = instructionMsg;
        //trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        instructionMsg = "You now hold a lightsaber in your right hand! \n\n " +
           "Slash the colored parts of the blocks with your saber! \n\n You don't need to press any button on the controller";
        trialInstructionText.text = instructionMsg;
        yield return new WaitForSeconds(10);
        timeUI.SetActive(true);
        scoreUI.SetActive(true);
       // trialInstructionText.text = "";
        trialInstructionUI.SetActive(false);
 
        //instructionMsg = "If you slash all boxes, you can press the NEXT button below.";
        //trialInstructionText.text = instructionMsg;


        yield return new WaitForSeconds(0); //20
    }

    IEnumerator InstructionForPause()
    {
        instructionMsg = ""; 
        trialInstructionText.text = instructionMsg;
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        yield return new WaitForSeconds(10);
        trialInstructionText.text = "Try to Pause the game.\n You can press the touchpad on the controller";
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        yield return new WaitForSeconds(3);
    }

    IEnumerator CleanInstructionBoard()
    {
        trialInstructionUI.SetActive(false);
        yield return new WaitForSeconds(3);
    }






}