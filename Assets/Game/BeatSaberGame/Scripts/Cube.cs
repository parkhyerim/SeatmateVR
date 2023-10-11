using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float speed = 4f;
    private bool stopMoving;
    private GameObject gameManagerObject;
    private BSGameManager bsGameManager;

    private void Start()
    {
        gameManagerObject = GameObject.Find("GameManager");
        bsGameManager = gameManagerObject.GetComponent<BSGameManager>();
    }

    void Update()
    {
        if (!stopMoving)
        {
            transform.position += Time.deltaTime * transform.forward * speed;
        }
        else
        {
            transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerBoundary")
        {
            Destroy(this.gameObject);
            bsGameManager.MissCube();
        }
    }

    public void StopMove()
    {
        stopMoving = true;
    }

    public void StartMove()
    {
        stopMoving = false;
    }
}
