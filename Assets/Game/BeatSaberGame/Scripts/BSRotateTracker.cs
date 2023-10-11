using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSRotateTracker : MonoBehaviour
{
    public bool doInteraction;
    public bool useTracker;
    public BSLogManager logManager;
    public BSGameManager gameManager;
    private BSBystanderAvatar bystanderAvatar;
    // temporary
    public float rotateSpeed = 2;
    [SerializeField]
    private bool isHeadingTo30, isHeadingToFrontSeat, isHeadingToPlayer;
    private bool askedQuestion;

    public bool IsHeadingToPlayer { get => isHeadingToPlayer; set => isHeadingToPlayer = value; }


    private void Awake()
    {
        bystanderAvatar = FindObjectOfType<BSBystanderAvatar>();
    }
    private void Start()
    {
        doInteraction = true;
       // Debug.Log(transform.eulerAngles.y);
    }

    void Update()
    {
        if (!useTracker)
        {
            // Bystander: 90 -> 0 degrees (towards the VR Player)
            if ((transform.eulerAngles.y <= 90)
                && !isHeadingTo30 && !isHeadingToFrontSeat && isHeadingToPlayer)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);  // Rotate the object around its local Y axis at 1 * speed degrees per second
                                                                              // transform.Rotate(0, 1 * speed * Time.deltaTime, 0); // previous code
                                                                              // RotateForInteraction();

                if (Mathf.Round(transform.eulerAngles.y) == 90)
                { // Heading towards the VR-Player
                    if (!askedQuestion)
                    {
                        gameManager.AskQuestion();
                        askedQuestion = true;
                    }

                    if (doInteraction)
                    {
                        Invoke(nameof(HeadingBackTo30Degrees), 9f); // Stay in 11 seconds (13)
                    }
                    else
                    {
                        Invoke(nameof(HeadingBackTo30Degrees), 0.5f);   // Stay in 0.5 seconds
                    }
                }
            }

            // Bystander: 0 -> 30 degrees (towards the front seat)
            if (isHeadingTo30 && !isHeadingToPlayer)
            {
                if (transform.eulerAngles.y > 50) // 60
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * -1);
                    // transform.Rotate(0, -1 * speed * Time.deltaTime, 0);
                    //anim.SetBool("isInteracting", false);
                    if (Mathf.Round(transform.eulerAngles.y) == 50) // 60
                    {
                        if (doInteraction)
                        {
                            Invoke(nameof(HeadingBacktoFrontSeat), 0f);
                        }
                        else
                        {
                            // TODO
                            Invoke(nameof(HeadingBacktoFrontSeat), 8.5f); //12.5
                        }
                    }
                }
            }

            // Bystander: 30 -> 90 degrees (towards the front seat)
            if (isHeadingToFrontSeat && !isHeadingToPlayer)
            {
                //anim.SetBool("isInteracting", false);
                if (transform.eulerAngles.y > 0)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * -1);

                    if (Mathf.Round(transform.eulerAngles.y) == 0)
                    {
                        isHeadingToFrontSeat = false;
                        Debug.Log("bystander in 90 degrees");
                    }
                }
            }
            // Controll the tracker with arrow keys 
            // transform.Rotate(0, Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0);
        }
    }

    private void HeadingBackTo30Degrees()
    {
        isHeadingTo30 = true;
        isHeadingToPlayer = false;
        // gameManager.BystanderEnd();
        gameManager.BystanderCanHearAnswer = false;
    }

    private void HeadingBacktoFrontSeat()
    {
        isHeadingTo30 = false;
        isHeadingToFrontSeat = true;
        isHeadingToPlayer = false;
        gameManager.BystanderEnd();
        gameManager.BystanderCanHearAnswer = false;
        askedQuestion = false;
    }
}
