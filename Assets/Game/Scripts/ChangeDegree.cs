using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDegree : MonoBehaviour
{
    public Text directionText;
    public float degrees;

    private void Start()
    {
        directionText.text = "Direction: 0 (front)";
    }

    private void Update()
    {
        degrees += 1;
        directionText.text = "Direction: " + degrees;
    }
}
