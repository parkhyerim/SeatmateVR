using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BSBystanderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    private Transform trackerTransform;
    BSGameManager gameManager;
    BSLogManager logManager;
    UserStudyManager userstudyManager;
    TimeLog timeLog;
    private float trackerEulerYAxis;  // bystander's euler y-axis -> tracker y
    public GameObject bystanderAvatar;
    public Animator bystanderAnim;
    [SerializeField]
    private bool doInteraction = true;
    public RawImage arrowImage;

    [Header("GOs for Animoji")]
    // Variables for Animoji Setting
    public GameObject presenceAnimojiBoard;
    public RawImage yesInteractionFrontImage, noInteractionFrontImage, backsideImage;
    public GameObject arrowPositionForMixed;
    public GameObject originalArrowPos;

    [Header("GOs for Avatar")]
    // variables for Avatar Setting
    public GameObject FOVPos;
    public GameObject guidePosForMixed;
    public GameObject originalAnimojiPanelPos;
    public GameObject middlePos;
    private Transform guidingPos;
    public GameObject arrowPosAvatarStart, arrowPosAvatarEnd, arrowPosAvatarMiddle, arrowParticle, arrowParticlePos;
    public GameObject guidingPosForAV;

    [Header("GOs for Mixed")]
    private float timeElapsedForMixedTransition;

    [Header("Time Settings")]
    public float timeToReachTarget;
    public float movingTime = 0f;

    [Header("User-Study Settings")]
    public bool sitToLeft;  // Where is the bystander sitting?
    private bool isAnimojiSetting, isAvatarSetting, isMixedSetting, isBaseline, isPractice;

    [Header("Avatar Sub-Settings")]
    // public bool isSeatedAndInFOV;

    private float mainCameraYAxis;
    private bool isGuidingFOVToSeatedExceed;
    public bool isguided;

    public float angleinFOV = 45f;

    private float guidingLength;
    private float guidingSpeed = 1.0f;
    private float timeElapsedForSeatToFOV, timeElapsedForFOVToSeat, timeElapsedForGuideToSEAT;
    public float guideTimeForAvatar = 3f;
    public float fadeTime = 2f;
    float timeElapsedForAnimoji = 0f;
    bool logFlag;

    [SerializeField]
    private bool inCriticalZone, inTransitionZone, inUncriticalZone, inNoZone;
    private bool lookedOnceSeatedPosition;
    private Color animojiBacksideColor;
    Color noTransparency, lowTransparency;
    public bool askedQuestion;
    private bool firstQuestioned, secondQuestioned, thirdQuestioned;
    private int[] studyOrder = new int[4];
    int currentLevelIndex; // practice 0
    int order;

    bool inVisualization;
    bool notif_On;
    bool questionOn;
    public bool LookedOnceSeatedPosition { get => lookedOnceSeatedPosition; set => lookedOnceSeatedPosition = value; }
    public bool IsGuidingFOVToSeatedExceed { get => isGuidingFOVToSeatedExceed; set => isGuidingFOVToSeatedExceed = value; }
    public bool IsAnimojiSetting { get => isAnimojiSetting; set => isAnimojiSetting = value; }
    public bool IsAvatarSetting { get => isAvatarSetting; set => isAvatarSetting = value; }
    public bool IsMixedSetting { get => isMixedSetting; set => isMixedSetting = value; }
    public bool IsBaseline { get => isBaseline; set => isBaseline = value; }
    public bool IsPractice { get => isPractice; set => isPractice = value; }
    public bool InVisualization { get => inVisualization; set => inVisualization = value; }
    public bool Notif_On { get => notif_On; set => notif_On = value; }
    public bool QuestionOn { get => questionOn; set => questionOn = value; }

    private void Awake()
    {
        gameManager = FindObjectOfType<BSGameManager>();
        logManager = FindObjectOfType<BSLogManager>();
        userstudyManager = FindObjectOfType<UserStudyManager>();
        timeLog = FindObjectOfType<TimeLog>();
    }
    // Start is called before the first frame update
    void Start()
    {
        doInteraction = true;
        sitToLeft = true;
        guidingPos = GetComponent<Transform>(); // For Avatar Setting (FOV -> Seated)
        bystanderAnim.SetBool("isInteracting", false);
      
        // Default setting: Avatar setting
        //if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting || isBaseline || isPractice))
        //    isPractice = true;

       // Debug.Log("Animoji: " + isAnimojiSetting + " Avatar: " + isAvatarSetting + " Mixed: " + isMixedSetting);
        yesInteractionFrontImage.enabled = false;
        noInteractionFrontImage.enabled = false;
        backsideImage.enabled = false;
        bystanderAvatar.SetActive(false);
        arrowImage.enabled = false;
        presenceAnimojiBoard.SetActive(false);

        trackerTransform = bystanderTracker.transform;
        transform.position = trackerTransform.position;
        trackerEulerYAxis = trackerTransform.eulerAngles.y;
        // bystanderRotationOffset = bystanderEulerYAxis - 0f;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        guidingLength = Vector3.Distance(new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z), trackerTransform.position);

        noTransparency = backsideImage.color;
        lowTransparency = backsideImage.color;
        noTransparency.a = 1f;
        lowTransparency.a = 0f;

        Notif_On = false;
        QuestionOn = false;
    }

    void FixedUpdate()
    {
        // transform.position = trackerTrans.position;
        trackerEulerYAxis = trackerTransform.eulerAngles.y;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        // For animoji? Avatar? guiding
        middlePos.transform.position = new Vector3(
            (originalAnimojiPanelPos.transform.position.x + trackerTransform.position.x) / 2,
            (originalAnimojiPanelPos.transform.position.y + trackerTransform.position.y) / 2,
            (originalAnimojiPanelPos.transform.position.z + trackerTransform.position.z) / 2);

        // For avatar guiding to seated pos
        guidingPos.position = new Vector3(
            (FOVPos.transform.position.x + trackerTransform.position.x) / 2,
            (FOVPos.transform.position.y + trackerTransform.position.y) / 2,
            (FOVPos.transform.position.z + trackerTransform.position.z) / 2);

        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
   /**********************************************************************************************
   ***************************** ANIMOJI SETTING *************************************************                                                              
   *********************************************************************************************/
            if (isAnimojiSetting)
            {
                //  [Animoji]  CRITICAL ZONE: 30 >= [Bystander's degrees] > 0 to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100)
                {
                    InVisualization = true;
                    inCriticalZone = true;
                    presenceAnimojiBoard.SetActive(true);
                    if (inTransitionZone) // From Transition Zone (60-30 degrees)
                    {
                       // Debug.Log("Enter_CZ");
                        BystanderShiftZone("Enter_CZ");
                        inTransitionZone = false;
                    }

                    // TODO: askedQuestion 
                    if (!askedQuestion)
                    {
                        // Bigger Animoji with FE
                        backsideImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    else
                    {
                        // Bigger Animoji with FE
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                }
                // [Animoji] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                    InVisualization = true;
                    inTransitionZone = true;
                    inNoZone = false;
                    timeElapsedForAnimoji = 0;
                    presenceAnimojiBoard.SetActive(true);

                    if (inCriticalZone) // From Critical Zone : Bigger animoji with FE -> backside
                    {
                       // Debug.Log("From_CZ_to_TZ");
                        BystanderShiftZone("From_CZ_to_TZ");
                        backsideImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        inCriticalZone = false;
                        logFlag = false;
                    }
                    if (inUncriticalZone) // From Uncritical Zone: backside -> small animoji
                    {
                       // Debug.Log("From_UZ_to_TZ");
                        BystanderShiftZone("From_UCZ_to_TZ");
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                        inUncriticalZone = false;
                        logFlag = false;
                    }
                }
                // [Animoji] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    InVisualization = true;
                    inUncriticalZone = true;

                    presenceAnimojiBoard.SetActive(true);
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    if (inNoZone) //&& !inTransitionZone) // From No-Zone : Full transparency to No transparency
                    {
                        if (!logFlag)
                        {
                            //Debug.Log("FROM_NZ_to_UCZ");
                            BystanderShiftZone("From_NZ_to_UCZ");
                            Notif_On = true;
                            timeLog.TimeStampForVisualisationOnMoment();
                            logFlag = true;                         
                        }                    
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime) // fadetime: 2f (default)
                        {
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                        }
                        else // more than fadetime(2f) 
                        {
                            backsideImage.color = noTransparency;
                            inNoZone = false;
                        }
                    }

                    if (inTransitionZone)// && !inNoZone) // From Transition Zone: No transparency to Full transparency
                    {
                        if (!logFlag)
                        {
                            //  Debug.Log("FROM_TZ_to_UCZ");
                            BystanderShiftZone("FromM_TZ_to_UCZ");
                            SetRightPauseStamp();
                            logFlag = true;
                        }
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                        }
                        else  // more than fadetime(2f) 
                        {
                            backsideImage.color = lowTransparency;
                            inTransitionZone = false;
                        }
                    }
                }
                // [Animoji] NO ZONE:  Bystander's degrees > 85
                else
                {
                    // No Visualisation
                    InVisualization = false;
                    // Set flags for each zone
                    inNoZone = true;
                    if (inUncriticalZone)
                    {
                        //Debug.Log("From_UCZ_to_NZ");
                        BystanderShiftZone("From_UCZ_To_NZ");
                        timeLog.TimeStampForVisualisationOffMoment();
                        Notif_On = false;
                        inUncriticalZone = false;                      
                    }
                    inTransitionZone = false;
                    inCriticalZone = false;
                    logFlag = false;

                    // No Visualisation
                    presenceAnimojiBoard.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    // time set 0
                    timeElapsedForAnimoji = 0f;
                }
            }
   /**********************************************************************************************
    ***************************** AVATAR SETTING *************************************************                                                              
    *********************************************************************************************/
            if (isAvatarSetting)
            {
                // [AVATAR]  CRITICAL ZONE: 30-0 degrees to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100)
                {
                    presenceAnimojiBoard.SetActive(true);
                    InVisualization = true;
                    inCriticalZone = true;
                    if (inTransitionZone)
                    {
                       // Debug.Log("Avatar: From TZ to CZ");
                        BystanderShiftZone("Enter_CZ"); // enter 30 (30->0) degrees
                        inTransitionZone = false;
                    }

                    // Visualisation with FE
                    bystanderAnim.SetBool("isInteracting", true);

                    // VR user is looking at the seated position
                    // TODO: depending on the headset models
                    if (mainCameraYAxis >= 250 && mainCameraYAxis <= 300) //310 -> 300
                    {
                        // Avatar: seated position & same rotation
                        transform.position = bystanderTracker.transform.position;
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, trackerEulerYAxis, 0);
                        arrowImage.enabled = false;
                        lookedOnceSeatedPosition = true;
                    }
                    //else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315) 
                    //{
                    //    bystanderAvatar.SetActive(false);
                    //    arrowImage.enabled = false;
                    //}
                    else  // The bystander is outside the FOV of the VR user ( 310 < d < 360, ....) 
                    {
                        movingTime += Time.deltaTime;
                        if (movingTime > (guideTimeForAvatar + 1f)) // 2+ 2f
                        { 
                            isGuidingFOVToSeatedExceed = true;
                        }

                        // The VR user haven't look at the seated avatar yet
                        if (!lookedOnceSeatedPosition)
                        {
                            // 1. move to the guding pos (border of fov)
                            if (!isGuidingFOVToSeatedExceed)
                            {
                                    timeElapsedForSeatToFOV += Time.deltaTime;
                                    if (timeElapsedForSeatToFOV < guideTimeForAvatar) // lerpGuideTime:2
                                    {
                                        float t = timeElapsedForSeatToFOV / guideTimeForAvatar;
                                        t = t * t * (3f - 2f * t);
                                        // guiding from tracker position -> fov position
                                        transform.position = Vector3.Lerp(
                                            new Vector3(bystanderTracker.transform.position.x, bystanderTracker.transform.position.y, bystanderTracker.transform.position.z),
                                            new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z),
                                        //  new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                    t);

                                        // Avatar's rotation angle (manipulation)
                                        bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                                Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0)),
                                                Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0)),
                                                t);
                                    }
                                    else // more than guiding (to FOV) Time (2 sec)
                                    {
                                        // Debug.Log("guide time passed: " + timeElapsedForSEATToFOV);
                                        transform.position = new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z);
                                        bystanderAvatar.transform.rotation = Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0));
                                        arrowImage.enabled = true;
                                        arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                                // Destroy(arrowParticle);
                                    }
                            }
                            // more than the expected guiding time
                            // -> intensively guide to the seated position
                            else
                            {
                                // ARROW
                                arrowImage.enabled = true;
                                arrowImage.transform.position = arrowPosAvatarStart.transform.position;
                                    // Instantiate(arrowParticle, arrowParticlePos.transform.position, Quaternion.identity);
                              
                                timeElapsedForFOVToSeat += Time.deltaTime;

                                if (timeElapsedForFOVToSeat < guideTimeForAvatar) // gudige time: default 2
                                {
                                    float t = timeElapsedForFOVToSeat / guideTimeForAvatar;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                        new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z),
                                        bystanderTracker.transform.position,
                                        t);

                                    // Angles
                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0)),
                                        Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0)),
                                        t);
                                    
                                    // Arrow for guiding
                                    arrowImage.transform.position = Vector3.Lerp(
                                        arrowPosAvatarStart.transform.position,
                                        arrowPosAvatarMiddle.transform.position,
                                        t);
                                }
                                // the guding time passed 
                                else
                                {
                                    transform.position = bystanderTracker.transform.position;
                                    bystanderAvatar.transform.rotation = Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0));
                                    arrowImage.enabled = true;
                                    arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                                }
                            }
                        }
                        else // already quided to seated once. (looked once the seated pos) but, looked at now FOV
                        {
                            transform.position = new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0);
                            arrowImage.enabled = true;       
                            arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                        }
                    }
                }
                // [AVATAR] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                    presenceAnimojiBoard.SetActive(false);
                    InVisualization = true;
                    inTransitionZone = true;
                    // From Uncritical Zone to Transition Zone
                    if (inUncriticalZone)
                    {
                       //Debug.Log("Avatar:From UCZ to TZ");
                        BystanderShiftZone("From_UCZ_to_TZ");

                        // Position (Seat)
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis

                        // Visualisation ON (No FE)
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        // No guiding
                        arrowImage.enabled = false;

                        inUncriticalZone = false;
                    }
                    else
                    {
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    }

                    if (inCriticalZone) // From Critical Zone to Transition Zone
                    {
                        // Debug.Log("Avatar:From CZ to TZ");
                        BystanderShiftZone("From_CZ_to_TZ");
                        // avatar without FE in the situated position            
                        transform.position = bystanderTracker.transform.position;
                        // manimpulation of the y-axis (rotates with bigger y-aixs)
                        // TODO: rational degrees
                       // transform.localEulerAngles = new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0);
                        transform.localEulerAngles = new Vector3(0, 30, 0);               
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, 0, 0);

                        // Visualisation ON (No FE)
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        arrowImage.enabled = false;

                        inCriticalZone = false;
                    }
                    else
                    {
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, 30, 0);
                    }
                    // Reset timer values as 0
                    movingTime = 0f; // for guiding to the critical zone 
                    timeElapsedForSeatToFOV = 0f;
                    timeElapsedForFOVToSeat = 0f;
                }
                // [AVATAR] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    inUncriticalZone = true;
                    InVisualization = true;
                    if (inNoZone)
                    {
                       // Debug.Log("Avatar: From NZ to UCZ");
                        BystanderShiftZone("From_NZ_To_UCZ");
                        timeLog.TimeStampForVisualisationOnMoment();
                        SetRightPauseStamp();
                        Notif_On = true;
                        inNoZone = false;                     
                    }

                    if (inTransitionZone)
                    {
                       // Debug.Log("Avatar: From TZ to UCZ");
                        BystanderShiftZone("From_TZ_To_UCZ");
                        inTransitionZone = false;
                    }
                   //inCriticalZone = false;

                    // Avatar without FE in the seated position 
                    // Position (SEAT)
                    transform.position = bystanderTracker.transform.position;
                    transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    // Visualisation ON (No FE)
                    bystanderAvatar.SetActive(true);
                    bystanderAnim.SetBool("isInteracting", false);
                    // NO Guiding
                    arrowImage.enabled = false;
                }
                // [AVATAR] NO ZONE:  Bystander's degrees > 85
                else
                {
                    InVisualization = false;

                    // bool flags
                    inNoZone = true;
                    if (inUncriticalZone)
                    {
                       // Debug.Log("Avatar: From UCZ To NZ");
                        BystanderShiftZone("From_UCZ_To_NZ");
                        Notif_On = false;
                        timeLog.TimeStampForVisualisationOffMoment();
                        inUncriticalZone = false;
                    }
                     inTransitionZone = false;
                    inCriticalZone = false;

                    // Nothing is visualisized
                    transform.position = bystanderTracker.transform.position;
                    transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    bystanderAvatar.SetActive(false);
                }
            }
    /**********************************************************************************************
    ***************************** MIXED SETTING *************************************************                                                              
    *********************************************************************************************/
            if (isMixedSetting)
            {
                // [MIXED] CRITICAL ZONE: 30 >= [Bystander's degrees] > 0 to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100) // 100 <- 90
                {
                    InVisualization = true;
                    presenceAnimojiBoard.SetActive(true);
                    inCriticalZone = true;
                    if (inTransitionZone)
                    {
                        //Debug.Log("From TZ To CZ");
                        BystanderShiftZone("Enter_CZ");
                        inTransitionZone = false;
                    }

                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);

                    // Avatar is still outside the FOV of VR user
                    // 320 - 315 - 300
                    if ((mainCameraYAxis >= 320 && mainCameraYAxis<=360) || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        // Animoji Guiding
                        bystanderAvatar.SetActive(false);

                        timeElapsedForMixedTransition += Time.deltaTime;
                        if (timeElapsedForMixedTransition < guideTimeForAvatar)
                        {
                            arrowImage.enabled = true;
                            float t = timeElapsedForMixedTransition / guideTimeForAvatar;
                            t = t * t * (3f - 2f * t);
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                originalAnimojiPanelPos.transform.position,
                                guidePosForMixed.transform.position,
                                t);

                            arrowImage.transform.position = Vector3.Lerp(
                                originalArrowPos.transform.position,
                                arrowPositionForMixed.transform.position,
                                t);
                        }
                        // time passed
                        else
                        {
                            presenceAnimojiBoard.transform.position = guidePosForMixed.transform.position;
                            arrowImage.transform.position = arrowPositionForMixed.transform.position;
                        }

                        arrowImage.enabled = true;
                        backsideImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    // VR user looking at the Bystander
                    // Avatar visualisation
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 230)
                    {
                        presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                        bystanderAvatar.SetActive(true);
                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);
                        // TODO: AskedQuestion
                        if (!askedQuestion) // Avatar with FE
                            bystanderAnim.SetBool("isInteracting", true);
                        else // Avatar without FE
                            bystanderAnim.SetBool("isInteracting", false);
                    }
                }
                // [MIXED] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis < 60 && trackerEulerYAxis >= 30)
                {
                    InVisualization = true;
                    presenceAnimojiBoard.SetActive(true);
                    inTransitionZone = true;
                    inNoZone = false;
                    logFlag = false;
                    timeElapsedForMixedTransition = 0;
                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                    // VR user is looking at the game
                    // -> The bystander's avatar is outside the VR user's FOV
                    // ANIMOJI Visualisation
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);

                        if (inCriticalZone) // From Critical Zone to Transition Zone
                        {
                           // Debug.Log("From_CZ_To_TZ");
                            BystanderShiftZone("From_CZ_To_TZ");
                            // show backside
                            backsideImage.enabled = true;
                            arrowImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = false;
                            inCriticalZone = false;
                        }
                        if (inUncriticalZone) // From Uncritical Zone
                        {
                           // Debug.Log("From_UCZ_To_TZ");
                            BystanderShiftZone("From_UCZ_To_TZ");
                            backsideImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = true;
                            noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                            arrowImage.enabled = false;
                            inUncriticalZone = false;
                        }
                    }
                    // VR user is looking at the bystander
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 230)
                    {
                        if (inCriticalZone)
                        {
                            BystanderShiftZone("From_CZ_To_TZ");
                            inCriticalZone = false;
                        }
                        if (inUncriticalZone)
                        {
                            BystanderShiftZone("From_UCZ_To_TZ");
                            inUncriticalZone = false;
                        }
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // [MIXED] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    InVisualization = true;
                    presenceAnimojiBoard.SetActive(true);
                    inUncriticalZone = true;
        
                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
  
                    // VR user is looking at the game 
                    //-> The bystander's avatar is outside the VR user's FOV
                    // Animoji Visualisation
                    if ((mainCameraYAxis >= 320 && mainCameraYAxis<=360) || (mainCameraYAxis > 0 && mainCameraYAxis <= 150))
                    {
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        arrowImage.enabled = false;

                        if (inNoZone) // From No-Zone: Full-transparency to No-transparency
                        {
                            timeElapsedForMixedTransition += Time.deltaTime;
                            presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                            if (!logFlag)
                            {
                                BystanderShiftZone("From_NZ_To_UCZ");
                                Notif_On = true;
                                SetRightPauseStamp();
                                timeLog.TimeStampForVisualisationOnMoment();
                                logFlag = true;
                            }

                            if (timeElapsedForMixedTransition < fadeTime) // fadeTime: 2f (default)
                            {
                                float t = timeElapsedForMixedTransition / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                            }
                            else // more than fadetime (e.g., 2f)
                            {
                                backsideImage.color = noTransparency;
                                inNoZone = false;
                            }                          
                        }

                        if (inTransitionZone) // From Transition Zone: No-Transparency to Full-transparency
                        {
                            timeElapsedForMixedTransition += Time.deltaTime;
                            presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;

                            if (!logFlag)
                            {
                                BystanderShiftZone("From_TZ_To_UCZ");
                                logFlag = true;
                            }
                            if (timeElapsedForMixedTransition < fadeTime)
                            {
                                float t = timeElapsedForMixedTransition / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                            }
                            else // more than fadetime (e.g., 2f)
                            {
                                backsideImage.color = lowTransparency;
                                inTransitionZone = false;
                            }
                           
                        }
                    }
                    // VR user is looking at the seated position 
                    // -> avatar in the seated position
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 230)
                    {
                        
                        if (inNoZone)
                        {
                            if (!logFlag)
                            {
                                BystanderShiftZone("From_NZ_To_UCZ");
                                timeLog.TimeStampForVisualisationOnMoment();
                                Notif_On = true;
                                SetRightPauseStamp();                              
                                logFlag = true;
                            }
                           //inNoZone = false;
                        }

                        if (inTransitionZone)
                        {
                            if (!logFlag)
                            {
                                BystanderShiftZone("From_TZ_To_UCZ");
                                logFlag = true;
                            }
                           // inTransitionZone = false;
                        }

                        // Avatar (No FE)
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);

                        // noInteractionFrontImage.enabled = false;
                        // yesInteractionFrontImage.enabled = false;
                        presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // [MIXED] NO ZONE:  Bystander's degrees > 85
                else
                {
                    inNoZone = true;
                    inVisualization = false;
                    if (inUncriticalZone)
                    {
                       // Debug.Log("Mixed: From UCZ to NZ");
                        BystanderShiftZone("From_UCZ_To_NZ");
                        timeLog.TimeStampForVisualisationOffMoment();
                        Notif_On = false;
                        inUncriticalZone = false;                     
                        logFlag = false;
                    }                    
                    inTransitionZone = false;
                    inCriticalZone = false;

                    // No Visualisation
                    bystanderAvatar.SetActive(false);
                    bystanderAnim.SetBool("isInteracting", false);

                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position; // set the begin postion of Animoji Panel
                    presenceAnimojiBoard.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    arrowImage.enabled = false;
                    timeElapsedForMixedTransition = 0;
                }
            }
            else if (isBaseline)
            {
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100)
                {
                    InVisualization = true;                
                }
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                    InVisualization = true;                
                }
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    InVisualization = true;                
                }
                else
                {
                    InVisualization = false;                 
                }
            }
        }
        /**************************************************
        ********** To the right side of the VR user
        ***********************************************/
        else
        {
            //    // critical zone
            //    if (bystanderEulerYAxis <= 300 && bystanderEulerYAxis >= 260)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //            // TODO: image bigger and animations
            //        }
            //        else if (isAvatarSetting)
            //        {
            //            if (mainCameraYAxis >= 60 && mainCameraYAxis <= 100)
            //            {
            //                transform.position = bystanderTracker.transform.position;
            //            }
            //            else
            //            {
            //                transform.position = FOVPos.transform.position;
            //            }

            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            // Debug.Log(bystanderRotationEulerY);
            //            bystanderAvatar.SetActive(true);
            //        }
            //        else if (isMixedSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);

            //            yesInteractionFrontImage.enabled = false;
            //            backsideImage.enabled = false;
            //        }
            //    }
            //    // pheriperal zone
            //    else if (bystanderEulerYAxis <= 330 && bystanderEulerYAxis > 300)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //        }

            //        if (isAvatarSetting)
            //        {

            //           transform.position = FOVPos.transform.position;


            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //        }
            //    }
            //    else if (bystanderEulerYAxis <= 360 && bystanderEulerYAxis > 300)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = true;
            //            yesInteractionFrontImage.enabled = false;
            //        }

            //        if (isAvatarSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = true;
            //            yesInteractionFrontImage.enabled = false;
            //        }
            //    }
            //    else
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = false;
            //        }

            //        if (isAvatarSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(false);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = false;
            //        }
            //    }
        }
    }

    public void SetUserstudyCondition()
    {
        isPractice = gameManager.isTrialGame;
        isBaseline = gameManager.isBaseline;
        isAnimojiSetting = gameManager.isAnimojiSetting;
        isAvatarSetting = gameManager.isAvatarSetting;
        isMixedSetting = gameManager.isMixedSetting;
      // Debug.Log("set condition is called: " + isPractice + isBaseline + isAnimojiSetting + isAvatarSetting + isMixedSetting);
    }
    private void BystanderShiftZone(string state)
    {
        gameManager.SetTimeStampForAvatarInCriticalZoneWithMessage(state);
    }

    private void SetRightPauseStamp()
    {
        gameManager.DoVisualising = true;
    }

    private void SetRightResumeStamp()
    {
        gameManager.DoVisualising = false;
    }
    public void TurnBackwards()
    {
        transform.localEulerAngles = new Vector3(0, 180, 0);
        //transform.Rotate(new Vector3(0, 180, 0) * Time.deltaTime);
    }
    Vector3 EulerAngles(Quaternion trs)
    {
        return trs.eulerAngles;
    }

    private void SetAnimoji(string rotationDegrees)
    {

        yesInteractionFrontImage.enabled = true;
        backsideImage.enabled = false;
        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
        // frontImage.GetComponent<RectTransform>().rect.Set(0, 0, 100, 300);
    }

    //public void GuideToBystander()
    //{
    //    transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
    //    //transform.position = new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z);
    //    isGuidingToSeated = true;
    //    if (isguided)
    //    {
    //        Invoke("SetGuided", 2f);
    //    }
    //}

    //public void SetGuided()
    //{
    //    isguided = true;
    //    transform.position = Vector3.Lerp(transform.position, new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z), Time.deltaTime * 2);
    //    Invoke("GuideToBystander", 2f);
    //}


    IEnumerator FadeImage(bool fadeOut)
    {
        if (fadeOut)
        {
            for (float f = 0.1f; f <= 1; f += 0.1f)
            {
                Debug.Log("Fade called");
                Color c = backsideImage.color;
                c.a = f;
                backsideImage.color = c;
                yield return null;
            }
        }
        else
        {
            for (float f = 0.1f; f <= 1; f -= 0.1f)
            {
                Color c = backsideImage.color;
                c.a = f;
                backsideImage.color = c;
                yield return null;
            }
        }
    }
}
