using UnityEngine;
using System;
using System.Globalization;
using System.IO;

public class BSLogManager : MonoBehaviour
{
    UserStudyManager userstudyManager;

    private string participantID;
    private string currentDateAndTime;
    private string currentTime;
    private string currentTimeinMilliseconds;
    int count = 1;
    private double currentTimeInEpoch;

    private void Awake()
    {
        userstudyManager = FindObjectOfType<UserStudyManager>();
        participantID = userstudyManager.GetParticipantID();
        currentDateAndTime = GetCurrentDateAndTime();
    }

    void Start()
    {    
        WriteLogForOverview("====================GAME LOG==============================" +
           "\n                    ID: " + participantID +
           ", LOG STRAT (Date & Time): " + currentDateAndTime);

        WriteLogForYawHeadMovement("===============HEAD MOVEMENT YAW (Y)========================" +
          "\n                    ID: " + participantID +
          ", LOG STRAT (Date & Time): " + currentDateAndTime);

        WriteLogForPitchHeadMovement("===============HEAD MOVEMENT PITCH (X)========================" +
          "\n                    ID: " + participantID +
          ", LOG STRAT (Date & Time): " + currentDateAndTime);

        WriteLogForRollHeadMovement("===============HEAD MOVEMENT Roll (Z)========================" +
          "\n                    ID: " + participantID +
          ", LOG STRAT (Date & Time): " + currentDateAndTime);

        WriteLogForEyeGaze("===============Eye Gaze======================================" +
          "\n                    ID: " + participantID +
          ", LOG STRAT (Date & Time): " + currentDateAndTime);

        WriteLogForHeadPosition("===============HEAD POSITIONS======================================" +
         "\n                    ID: " + participantID +
          ", LOG STRAT (Date & Time): " + currentDateAndTime);
    }

    // For the overview logfile
    public void WriteLogForOverview(string message)
    {        
        using (System.IO.StreamWriter logFile =
            new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_Overview.txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
           // logFile.Write("[" + currentTimeinMilliseconds + "] ");
          //  logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");           
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForYawHeadMovement(string message) // Horizontal (Y-Axis) ex.: Turn Left/Right Head
    {

        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement_Yaw(Y).txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
           // logFile.Write("[" + currentTimeinMilliseconds + "] ");
         //   logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForPitchHeadMovement(string message) // Vertical (X-Axis) ex.: Nodding Head
    {
        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement_Pitch(X).txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
           // logFile.Write("[" + currentTimeinMilliseconds + "] ");
         //   logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForRollHeadMovement(string message) // (Z-Axis) ex.: Streching out Neck to leftdown/rightdown
    {
        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement_Roll(Z).txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
            //logFile.Write("[" + currentTimeinMilliseconds + "] ");
          //  logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");
            logFile.WriteLine(message);
        }
    }



    public void WriteLogForHeadPosition(string message)
    {
        using (System.IO.StreamWriter logFile =
                   new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadPosition.txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
            //logFile.Write("[" + currentTimeinMilliseconds + "] ");
           // logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForEyeGaze(string message)
    {
        using (System.IO.StreamWriter logFile =
                   new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_Gaze.txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
          //  logFile.Write("[" + currentTimeinMilliseconds + "] ");
          //  logFile.Write("[" + currentTime + "] ");
            logFile.Write("[" + currentTimeInEpoch + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForExcel(string message, bool init)
    {
        using (System.IO.StreamWriter logFile =
                   new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_Timelog.txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            currentTimeInEpoch = GetTimeFromEpoch();
            //  logFile.Write("[" + currentTimeinMilliseconds + "] ");
            // logFile.Write("[" + currentTime + "] ");
            if (init)
            {
               // logFile.Write("[" + timeInEpoch + "] ");
                logFile.WriteLine(message);
            }
            else
            {
                logFile.Write(currentTimeInEpoch + ",");
                logFile.WriteLine(message);
            }         
        }
    }

    private string GetCurrentDateAndTime()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        return name;
    }

    private string GetCurrentTime()
    {
        DateTime localDate = DateTime.Now;
        string name = localDate.ToString("HH:mm:ss");

        return name;
    }

    private string GetCurrentTimeMilliseconds()
    {
        DateTime localDate = DateTime.Now;
        string name = localDate.ToString("HH:mm:ss:ms");

        return name;
    }

    private double GetTimeFromEpoch()
    {
        var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var timeStamp = (DateTime.UtcNow - epochStart).TotalSeconds;
        return timeStamp;
    }
}
