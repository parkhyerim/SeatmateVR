using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHandler : MonoBehaviour
{

    private Animator anim;
    private bool isOpen;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOpen = !isOpen;
            anim.SetBool("isOpen", isOpen);

        }
    }
}
