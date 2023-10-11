using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MemoryCard : MonoBehaviour
{
    //public XRSimpleInteractable simpleInteractable = null;

    public int identifier;
    public float targetHeight = 10f;
    public float targetRotation = 0;
    private bool isGameStart;
    public bool IsGameStart { get => isGameStart; set => isGameStart = value; }

    private void Awake()
    {
       // simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void Update()
    {
        // Move up/down
        //   float heightValue = Mathf.MoveTowards(transform.position.z, targetHeight, 1 * Time.deltaTime);
        //   transform.position = new Vector3(transform.position.x, transform.position.y, 0);


        if (IsGameStart) {
            // Rotate
            Quaternion rotationValue = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 10 * Time.deltaTime);
        } 
    }

    private void OnSelectEnter()
    {
       Debug.Log("selected");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "RightHand")
            Debug.Log("trigger");
    }

    // XR Interaction
    public void SelectCard()
    {
        FindObjectOfType<GameManager>().CardClicked(this);
    }
}
