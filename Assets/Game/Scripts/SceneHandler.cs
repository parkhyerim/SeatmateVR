using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SceneHandler : MonoBehaviour
{
    public SteamVR_LaserPointer[] laserPointer;
    public MemoryCard memoryCard;
    private GameObject cardObject;

    private void Awake()
    {
        for(int i= 0; i < laserPointer.Length; i++)
        {
            laserPointer[i].PointerIn += PointerInside;
            laserPointer[i].PointerOut += PointerOutside;
            laserPointer[i].PointerClick += PointerClick;
        }

       // mc = GetComponent<MemoryCard>();
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if(e.target.tag == "MemoryCard")
        {
           // Debug.Log(e.target.gameObject);
            cardObject = e.target.gameObject;
            memoryCard = cardObject.GetComponent<MemoryCard>();
            FindObjectOfType<GameManager>().CardClicked(memoryCard);
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        //if (e.target.name == "Table")
        //{
        //}
        //else if (e.target.name == "placeholder")
        //{
        //}
    }
}
