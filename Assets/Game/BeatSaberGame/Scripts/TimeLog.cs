using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLog : MonoBehaviour
{
    HeadMovement headMovement;
    BSLogManager logManager;
    BSGameManager gameManager;
    BSBystanderAvatar bystanderAvatar;

    private float period = 0.09f;
    private float checkTimer = 0.0f;
    private int count;

    bool gameStart;

    float headUpMaxDegrees, headDownDegrees, headLeftDegrees, headRightDegrees; 
    float curEulerY, curEulerX, curEulerZ;
    float curPosX, curPosY, curPosZ;
    float gazeXAni, gazeYAni, gazeZAni, gazeXAva, gazeYAva, gazeZAva;
    bool paused;
    bool gazeAnimoji, gazeAvatar, gazeUI, gazeScore, gazeTimer, gazeCubearea;
    bool notif_On, notif_zero_On, notif_one_On, notif_two_On, notif_three_On;
    bool question_one_On, question_two_On, question_three_On;
    bool isAvatarSetting, isAnimojiSetting, isMixedSetting;
    public bool GameStart { get => gameStart; set => gameStart = value; }

    private void Awake()
    {
        headMovement = FindObjectOfType<HeadMovement>();
        logManager = FindObjectOfType<BSLogManager>();
        gameManager = FindObjectOfType<BSGameManager>();
        bystanderAvatar = FindObjectOfType<BSBystanderAvatar>();
    }

    private void Start()
    {
        //   + "HeadUpMax(-(deg.))," + " HeadDownMax(deg.)," + " HeadLeft(-(deg.))," + " HeadRight(deg.),"
        logManager.WriteLogForExcel(
            "Timestamp(s:ms),"
            + "RotationX(deg.)," + "RotationY(deg.)," + "RotationZ(deg.),"
            + "PositionX," + " PosítionY," + " PositionZ,"
            + "GazePosX_Animoji," + "GazePosY_Animoji," + "GazePosZ_Animoji,"
            + "GazePosX_Avatar," + "GazePosY_Avatar," + "GazePosZ_Avatar,"
            + "GazeAnimoji(bool)," + "GazeAvatar(bool)," + "GazeCubeArea(bool)," + "GazeUI(bool),"
            + "Pause(bool),"
            + "Notification,"
            + "Notif_Start,"
            + "Notif_End,"
            + "Question_Start,"
            //+ "Notif_Start0," + "Notif_End0,"
            //+ "Notif_Start1," + "Question1," + "Notif_End1,"
            //+ "Notif_Start2," + "Question2," + "Notif_End2,"
            //+ "Notif_Start3," + "Question3," + "Notif_End3,"
            , true);
        gazeXAva = 0f;
        gazeYAva = 0f;
        gazeZAva = 0f;
        gazeXAni = 0f;
        gazeYAni = 0f;
        gazeZAni = 0f;
        isAvatarSetting = gameManager.isAvatarSetting;
        isAnimojiSetting = gameManager.isAnimojiSetting;
        isMixedSetting = gameManager.isMixedSetting;
    }

    private void FixedUpdate()
    {
        checkTimer += Time.fixedDeltaTime;

        if (!GameStart)
        {
            headUpMaxDegrees = gameManager.MaxUpAxis;
            headDownDegrees = gameManager.MaxDownAxis;
            headLeftDegrees = gameManager.MaxLeftAxis;
            headRightDegrees = gameManager.MaxRightAxis;
            curEulerY = headMovement.Conv_curEulerY;
            curEulerX = headMovement.Conv_curEulerX;
            curEulerZ = headMovement.Conv_curEulerZ;
            curPosX = headMovement.HeadsetPosX;
            curPosY = headMovement.HeadsetPosY;
            curPosZ = headMovement.HeadsetPosZ;
            paused = gameManager.GamePaused;
            gazeAnimoji = gameManager.gazeAnimoji;
            gazeAvatar = gameManager.gazeAvatar;
            gazeCubearea = gameManager.gazeCubes;
          //  gazeScore = gameManager.gazeScore;
          //  gazeTimer = gameManager.gazeTimer;
            gazeUI = gameManager.gazeUI;
            if (isAnimojiSetting)
            {
                gazeXAni = gameManager.AnimojiGazeTransform.position.x;
                gazeYAni = gameManager.AnimojiGazeTransform.position.y;
                gazeZAni = gameManager.AnimojiGazeTransform.position.z;
 
            }
            else if (isAvatarSetting)
            {
            
                gazeXAva = gameManager.AvatarGazeTransform.position.x;
                gazeYAva = gameManager.AvatarGazeTransform.position.y;
                gazeZAva = gameManager.AvatarGazeTransform.position.z;
            }
            else if (isMixedSetting)
            {
                gazeXAni = gameManager.AnimojiGazeTransform.position.x; // check
                gazeYAni = gameManager.AnimojiGazeTransform.position.y;
                gazeZAni = gameManager.AnimojiGazeTransform.position.z;
                gazeXAva = gameManager.AvatarGazeTransform.position.x;
                gazeYAva = gameManager.AvatarGazeTransform.position.y;
                gazeZAva = gameManager.AvatarGazeTransform.position.z;
            }

           
            // notification
            notif_On = bystanderAvatar.Notif_On;
            
        }
        else
        {
            checkTimer += Time.fixedDeltaTime;
            if (checkTimer >= period)
            {
               // Debug.Log("chek timer: " + checkTimer);
                headUpMaxDegrees = gameManager.MaxUpAxis;
                headDownDegrees = gameManager.MaxDownAxis;
                headLeftDegrees = gameManager.MaxLeftAxis;
                headRightDegrees = gameManager.MaxRightAxis;
                curEulerY = headMovement.Conv_curEulerY;
                curEulerX = headMovement.Conv_curEulerX;
                curEulerZ = headMovement.Conv_curEulerZ;
                curPosX = headMovement.HeadsetPosX;
                curPosY = headMovement.HeadsetPosY;
                curPosZ = headMovement.HeadsetPosZ;
                paused = gameManager.GamePaused;
                gazeAnimoji = gameManager.gazeAnimoji;
                gazeAvatar = gameManager.gazeAvatar;
                gazeCubearea = gameManager.gazeCubes;
              //  gazeScore = gameManager.gazeScore;
              //  gazeTimer = gameManager.gazeTimer;
                gazeUI = gameManager.gazeUI;
                if (isAnimojiSetting)
                {
                    gazeXAni = gameManager.AnimojiGazeTransform.position.x;
                    gazeYAni = gameManager.AnimojiGazeTransform.position.y;
                    gazeZAni = gameManager.AnimojiGazeTransform.position.z;
                    
                }
                else if (isAvatarSetting)
                {
                    
                    gazeXAva = gameManager.AvatarGazeTransform.position.x;
                    gazeYAva = gameManager.AvatarGazeTransform.position.y;
                    gazeZAva = gameManager.AvatarGazeTransform.position.z;
                }
                else if (isMixedSetting)
                {
                    gazeXAni = gameManager.AnimojiGazeTransform.position.x;
                    gazeYAni = gameManager.AnimojiGazeTransform.position.y;
                    gazeZAni = gameManager.AnimojiGazeTransform.position.z;
                    gazeXAva = gameManager.AvatarGazeTransform.position.x;
                    gazeYAva = gameManager.AvatarGazeTransform.position.y;
                    gazeZAva = gameManager.AvatarGazeTransform.position.z;
                }
             
                notif_On = bystanderAvatar.Notif_On;
                
                logManager.WriteLogForExcel(
                     curEulerX + "," + curEulerY + "," + curEulerZ + ","
                    + curPosX + "," + curPosY + "," + curPosZ + ","
                    + gazeXAni + "," + gazeYAni + "," + gazeZAni + ","
                     + gazeXAva + "," + gazeYAva + "," + gazeZAva + ","
                    + gazeAnimoji +","+ gazeAvatar +"," + gazeCubearea +"," + gazeUI+","
                    + paused + ", "
                    + notif_On + ","
                    + "-" + ","
                    + "-" + ","
                    + "-" + ","
                    , false);


                checkTimer = 0f;
            }
        }
    }


    public void TimeStampForQuestionMoment(string num)
    {
        logManager.WriteLogForExcel(
                    curEulerX + "," + curEulerY + "," + curEulerZ + ","
                   + curPosX + "," + curPosY + "," + curPosZ + ","
                   + gazeXAni + "," + gazeYAni + "," + gazeZAni + ","
                    + gazeXAva + "," + gazeYAva + "," + gazeZAva + ","
                   + gazeAnimoji + "," + gazeAvatar + "," + gazeCubearea + "," + gazeUI + ","
                   + paused + ", "
                   + notif_On + ","
                   + "-" + ","
                   + "-" + ","
                   + "Yes " + num + ","
                   , false);
    }

    public void TimeStampForVisualisationOnMoment()
    {
        logManager.WriteLogForExcel(
                   curEulerX + "," + curEulerY + "," + curEulerZ + ","
                  + curPosX + "," + curPosY + "," + curPosZ + ","
                  + gazeXAni + "," + gazeYAni + "," + gazeZAni + ","
                   + gazeXAva + "," + gazeYAva + "," + gazeZAva + ","
                  + gazeAnimoji + "," + gazeAvatar + "," + gazeCubearea + "," + gazeUI + ","
                  + paused + ", "
                  + notif_On + ","
                  + "Yes" + ","
                  + "-" + ","
                  + "-" + ","
                  , false);
    }

    public void TimeStampForVisualisationOffMoment()
    {
        logManager.WriteLogForExcel(
                   curEulerX + "," + curEulerY + "," + curEulerZ + ","
                  + curPosX + "," + curPosY + "," + curPosZ + ","
                  + gazeXAni + "," + gazeYAni + "," + gazeZAni + ","
                   + gazeXAva + "," + gazeYAva + "," + gazeZAva + ","
                  + gazeAnimoji + "," + gazeAvatar + "," + gazeCubearea + "," + gazeUI + ","
                  + paused + ", "
                  + notif_On + ","
                  + "-" + ","
                  + "Yes" + ","
                  + "-" + ","
                  , false);
    }
}
