using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCanvasEventCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        // Assign Camera.main to the Canvas if it isn't already assigned
        if(canvas && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
}
