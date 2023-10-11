using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    BSLogManager logManager;

    private float period = 0.2f; // timestamp period
    private float checkTimer = 0.0f;
    private int count;

    private Vector3 headsetEulerAngles, headsetLocalPosition, headsetOriginPos;
    private Quaternion headsetRotationAngles;
    private float headsetPosX, headsetPosY, headsetPosZ;

    // Yaw (Y roatation)
    private float curEulerY, prevEulerY, diffEulerY, avgEulerY, sumEulerY;
    private float conv_curEulerY, conv_prevEulerY, conv_diffEulerY, conv_avgEulerY, conv_sumEulerY;
    private float curRotY, prevRotY, diffRotY, avgRotY, sumRotY;

    // Pitch (X rotation)
    private float curEulerX, prevEulerX, diffEulerX, avgEulerX, sumEulerX;
    private float conv_curEulerX, conv_prevEulerX, conv_diffEulerX, conv_avgEulerX, conv_sumEulerX;
    private float curRotX, prevRotX, diffRotX, avgRotX, sumRotx;

    // Roll (Z rotation)
    private float curEulerZ, prevEulerZ, diffEulerZ, avgEulerZ, sumEulerZ;
    private float conv_curEulerZ, conv_prevEulerZ, conv_diffEulerZ, conv_avgEulerZ, conv_sumEulerZ;
    private float curRotZ, prevRotZ, diffRotZ, avgRotZ, sumRotZ;

    bool gameStart, gameEnd;
    private bool startInfoRecored;

    public bool GameStart { get => gameStart; set => gameStart = value; }
    public bool GameEnd { get => gameEnd; set => gameEnd = value; }
    public float Conv_curEulerY { get => conv_curEulerY; set => conv_curEulerY = value; }
    public float Conv_curEulerX { get => conv_curEulerX; set => conv_curEulerX = value; }
    public float Conv_curEulerZ { get => conv_curEulerZ; set => conv_curEulerZ = value; }
    public float HeadsetPosX { get => headsetPosX; set => headsetPosX = value; }
    public float HeadsetPosY { get => headsetPosY; set => headsetPosY = value; }
    public float HeadsetPosZ { get => headsetPosZ; set => headsetPosZ = value; }

    private void Awake()
    {
        logManager = FindObjectOfType<BSLogManager>();
    }

    void Start()
    {
        count = -1;
    }

    void FixedUpdate()
    {
        /***********************************
        *******  Rotation of Heaset
        ****************************/
        // 1.Euler
        headsetEulerAngles = Camera.main.transform.eulerAngles;
        curEulerY = headsetEulerAngles.y;
        curEulerX = headsetEulerAngles.x;
        curEulerZ = headsetEulerAngles.z;

        // 2. Rotation
        headsetRotationAngles = Camera.main.transform.rotation;
        curRotY = headsetRotationAngles.y;
        curRotX = headsetRotationAngles.x;
        curRotZ = headsetRotationAngles.z;
      
        // 3. converted euler
        // Yaw(Y)
        if (curEulerY > 180 && curEulerY <= 360) // 360-> 270-> 179 => 0-> -90 -> -179
        {
            conv_curEulerY = curEulerY - 360f;
        }
        else if (curEulerY > 0 && curEulerY <= 180) // 1-> 90-> 180 => 1 -> 90 -> 180
        {
            conv_curEulerY = curEulerY;
        }

        // Pitch(X)
        if (curEulerX > 180 && curEulerX <= 360) // 360-> 270-> 179 => 0-> -90 -> -179
        {
            conv_curEulerX = curEulerX - 360f;
        }
        else if (curEulerX > 0 && curEulerX <= 180) // 1-> 90-> 179 => -1 -90 -179
        {
            conv_curEulerX = curEulerX;
        }
      
        // Roll(Z)
        if (curEulerZ > 180 && curEulerZ <= 360) // 360-> 270-> 179 => 0-> -90 -> -179
        {
            conv_curEulerZ = curEulerZ - 360f;
        }
        else if (curEulerZ > 0 && curEulerZ <= 180) // 1-> 90-> 179 => -1 - 90 - 179
        {
            conv_curEulerZ = curEulerZ;
        }

        /***********************************
        *******  Position of Heaset
        ****************************/
        headsetLocalPosition = Camera.main.transform.localPosition;
        headsetPosX = headsetLocalPosition.x;
        headsetPosY = headsetLocalPosition.y;
        headsetPosZ = headsetLocalPosition.z;

        if (!GameStart) // Before game Start
        {
            if (!GameEnd) // Until game End
            {
                prevEulerY = curEulerY;
                prevEulerX = curEulerX;
                prevEulerZ = curEulerZ;
                conv_prevEulerX = conv_curEulerX;
                conv_prevEulerY = conv_curEulerY;
                conv_prevEulerZ = conv_curEulerZ;
                prevRotY = curRotY;
            }          
        }
        else 
        {
            if (!startInfoRecored)
            {
                headsetOriginPos = headsetLocalPosition;
                logManager.WriteLogForPitchHeadMovement("GAME START: " + conv_curEulerX + " (" + curEulerX + ")");
                logManager.WriteLogForYawHeadMovement("GAME START: " + conv_curEulerY + " (" + curEulerY + ")");
                logManager.WriteLogForRollHeadMovement("GAME START: " + conv_curEulerZ + " (" + curEulerZ + ")");
                logManager.WriteLogForHeadPosition("GAME START: x:" + headsetOriginPos.x + ", y:" + headsetOriginPos.y + ", z:" + headsetOriginPos.z + " " + headsetOriginPos);
                startInfoRecored = true;
            }
            // Time
            checkTimer += Time.fixedDeltaTime; // 0f - 0.2f
             
            if (checkTimer >= period) // every 0.2 secs
            {
                count += 1;
                if (count > 0)
                {
                    
                    diffEulerY = Mathf.Abs(prevEulerY - curEulerY);
                    diffEulerX = Mathf.Abs(prevEulerX - curEulerX);
                    diffEulerZ = Mathf.Abs(prevEulerZ - curEulerZ);
                    // Converted
                    conv_diffEulerY = Mathf.Abs(conv_prevEulerY - conv_curEulerY);
                    conv_diffEulerX = Mathf.Abs(conv_prevEulerX - conv_curEulerX);
                    conv_diffEulerZ = Mathf.Abs(conv_prevEulerZ - conv_curEulerZ);

                    sumEulerY += diffEulerY;
                    sumEulerX += diffEulerX;
                    sumEulerZ += diffEulerZ;
                    conv_sumEulerY += conv_diffEulerY;
                    conv_sumEulerX += conv_diffEulerX;
                    conv_sumEulerZ += conv_diffEulerZ;
            
                    avgEulerY = sumEulerY / count;
                    avgEulerX = sumEulerX / count;
                    avgEulerZ = sumEulerZ / count;
                    conv_avgEulerY = conv_sumEulerY / count;
                    conv_avgEulerX = conv_sumEulerX / count;
                    conv_avgEulerZ = conv_sumEulerZ / count;

                    // Y-Axis
                    string logMsgForYawHM = 
                        "Time:" + Time.time +
                        ", period: " + checkTimer +
                        ", prev:" + conv_prevEulerY + "(" + prevEulerY + ") " +
                        ", cur: " + conv_curEulerY + "(" + curEulerY + ") " +
                        ", diff: " + conv_diffEulerY +                     
                        ", sum: " + conv_sumEulerY+
                        ", count: " + count +
                        ", avg: " + conv_avgEulerY;

                    string logMsgForPitchHM = 
                        "Time:" + Time.time +
                        ", period: " + checkTimer +
                        ", prev:" + conv_prevEulerX + "(" + prevEulerX + ") " +
                        ", cur: " + conv_curEulerX + "(" + curEulerX + ") " +
                        ", diff: " + conv_diffEulerX +
                        ", sum: " + conv_sumEulerX +
                        ", count: " + count +
                        ", avg: " + conv_avgEulerX;

                    string logMsgForRollHM = 
                        "Time:" + Time.time +
                        ", period: " + checkTimer +
                        ", prev:" + conv_prevEulerZ + "(" + prevEulerZ + ") " +
                        ", cur: " + conv_curEulerZ + "(" + curEulerZ + ") " +
                        ", diff: " + conv_diffEulerZ +                  
                        ", sum: " + conv_sumEulerZ +
                        ", count: " + count +
                        ", avg: " + conv_avgEulerZ;

                    string logMsgForHeadPosition = 
                        "Time:" + Time.time +
                        ", period: " + checkTimer +
                        ", count: " + count +
                        ", x:" + headsetPosX+
                        ", y: " + headsetPosY +
                        ", z: " + headsetPosZ +                    
                        ", Vector3: (" + headsetLocalPosition.x +"," + headsetLocalPosition.y +","+ headsetLocalPosition.z +")";

                    conv_prevEulerY = conv_curEulerY;
                    conv_prevEulerX = conv_curEulerX;
                    conv_prevEulerZ = conv_curEulerZ;
                    prevEulerY = curEulerY;
                    prevEulerX = curEulerX;
                    prevEulerZ = curEulerZ;
                    logManager.WriteLogForYawHeadMovement(logMsgForYawHM);
                    logManager.WriteLogForPitchHeadMovement(logMsgForPitchHM);
                    logManager.WriteLogForRollHeadMovement(logMsgForRollHM);
                    logManager.WriteLogForHeadPosition(logMsgForHeadPosition);
                }

                checkTimer = 0f;
            }
        }
    }

    public float GetYawHeadMovement()
    {
        return conv_avgEulerY;
    }

    public float GetRollHeadMovement()
    {
        return conv_avgEulerZ;
    }

    public float GetPitchHeadMovement()
    {
        return conv_avgEulerX;
    }
}
