using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;
    public BSGameManager gameMananger;
    public GameObject blueEffect;
    public GameObject greenEffect;
    public GameObject yellowEffect;
    public GameObject startEffect;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            if(Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130 )// && Vector3.Angle(transform.position - previousPos, hit.transform.up) < 150)
            {
                //  Debug.Log(hit.transform.gameObject.name + ": Hit Destroy " + Vector3.Angle(transform.position - previousPos, hit.transform.up));
               //Debug.Log(Vector3.Angle(transform.position - previousPos, hit.transform.up));
                gameMananger.SliceCube(hit.transform.gameObject);

                //if (hit.transform.gameObject.name.Contains("Blue"))
                //{
                  
                // // Instantiate(blueEffect, hit.transform.position, Quaternion.identity);
                    
                //}
                //else if(hit.transform.gameObject.name.Contains("GreenCube"))
                //{
                    
                //    // Instantiate(greenEffect, hit.transform.position, Quaternion.identity);
                //}
                //else if(hit.transform.gameObject.name.Contains("YellowCube"))
                //{
                  
                //    // Instantiate(yellowEffect, hit.transform.position, Quaternion.identity);
                //}
                //else
                //{
                //   // Instantiate(startEffect, hit.transform.position, Quaternion.identity);
                //}
                Destroy(hit.transform.gameObject);
            }

        }
        previousPos = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // gameMananger.CubeSliced(collision.gameObject);
        //Debug.Log("collision");
        //if (collision.gameObject.tag == "Cube")
        //{
        //    Debug.Log("cube collision");
        //    this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        //}
        //if(collision.gameObject.tag == "Cube")
        //{
        //    //Destroy(collision.gameObject);
        //  //  Debug.Log(collision.gameObject.name);
        //    //this.gameObject.GetComponent<Renderer>().material.color = warningMat.color;
        //}
        //if(collision.gameObject.tag == "HitCube")
        //{
        //   // Destroy(collision.gameObject);
        //  //  Debug.Log(collision.gameObject.name);
        //  //  this.gameObject.GetComponent<Renderer>().material.color = originalMat.color;
        //}
    }
}
