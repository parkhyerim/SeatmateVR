using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

public class SocketManager : MonoBehaviour
{
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    public bool socketReady = false; //a true/false variable for connection status
    //try to initiate connection
    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient("localhost", 25001);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }
    //send message to server
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String tmpString = theLine + "\r\n";
        theWriter.Write(tmpString);
        theWriter.Flush();
    }
    //disconnect from the socket
    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }
}
