using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintOnOff : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake");
    }
    private void OnDisable()
    {
        Debug.Log("PrintOnDisable: script was disabled");
    }

    private void OnEnable()
    {
        Debug.Log("PrintOnEnable: script was enabled");
    }

    private void Start()
    {
        Debug.Log("Start is called");
    }

    private void Update()
    {
#if UNITY_IOS

        Debug.Log("Unity iOS");

#elif UNITY_EDITOR

        //Debug.Log("Unity Editor");

#else

        Debug.Log("Any other platform");

#endif
    }
}
