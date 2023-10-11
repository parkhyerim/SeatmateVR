using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MemoryCube : MonoBehaviour
{
    public GameObject pos;

    public int identifier;

    private void Awake()
    {
      //  transform.position = pos.transform.position;
    }

    private void Update()
    {
       // if (Input.GetButtonDown("Fire1"))
          
        
    }

    public void OnMouseDown()
    {
        Debug.Log("clicked");
        FindObjectOfType<MCGameManager>().CubeClicked(this);
    }

    public void OnPointerIn()
    {
        Debug.Log("pointer in");
    }

    public void OnPointerClick()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "RightHand")
            Debug.Log("trigger");
    }
}
