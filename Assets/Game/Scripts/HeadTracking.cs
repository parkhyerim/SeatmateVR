using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadTracking : MonoBehaviour
{

    GameObject centerEye;
    Vector3 headsetPos;
    Quaternion devPos;
    GameObject mcam;
    public GameObject avatar;
    Renderer[] avatarRenderer;
    


    // Start is called before the first frame update
    void Start()
    {
      // centerEye = GameObject.Find("centerEyePosition");
        mcam = GameObject.Find("Main Camera");

         avatarRenderer = avatar.GetComponentsInChildren<Renderer>();
    
    }

    // Update is called once per frame
    void Update()
    {
      //  headsetPos = centerEye.transform.position;
       devPos = mcam.transform.rotation;

        Invoke("ShowRotation", 3);
      
        
        //if (devPos.y > 30)
        //{
        //    Debug.Log("plus 30");
        //}
        
        //if (devPos.y < -30)
        //{
        //    Debug.Log("minus 30");
        //}
        
        //if(devPos.y == 0)
        //{
        //    Debug.Log("0 degree");
        //}
    }

    public void ShowRotation()
    {
        if(devPos.y >= 0.3 && devPos.y < 0.9)
        {

            // avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
            for(int i = 0; i < avatarRenderer.Length; i++)
            {
                avatarRenderer[i].material.SetColor("_Color", Color.cyan);

            }
        }
        else if(devPos.y <= -0.3 && devPos.y >= -0.9)
        {
            // avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
            for (int i = 0; i < avatarRenderer.Length; i++)
            {
                avatarRenderer[i].material.SetColor("_Color", Color.yellow);

            }
        }
        else if(devPos.y < 0.3 && devPos.y > -0.3)
        {
            //avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            avatarRenderer[0].material.SetColor("_Color", Color.red);
            avatarRenderer[1].material.SetColor("_Color", Color.red);

        }
        else
        {
            for (int i = 0; i < avatarRenderer.Length; i++)
            {
                avatarRenderer[i].material.SetColor("_Color", Color.white);

            }
        }


        // Debug.Log(devPos);
    }
}
