using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject[] cubes;
    public Transform[] points;
    // TODO: Set the right beat for the selected music
    public float beat = (60 / 150) * 2; // 1.142...
    private float timer = 0.0f;
    private float fastTimer;
    private bool canSpawn;
    private bool stopSpawn;
    private bool stopMoving;
    private int count;
    public bool CanSpawn { get => canSpawn; set => canSpawn = value; }
    public bool StopSpawn { get => stopSpawn; set => stopSpawn = value; }
    public bool StopMoving { get => stopMoving; set => stopMoving = value; }

    private void Start()
    {
        count = 0;
    }
    void Update()
    {
        if (CanSpawn)
        {
            // Check if we have reached beyond beat(1.142..) seconds
            // Subtracting beat seconds is more accurate over time than resetting to zero.
            if (timer > beat)
            {
                GameObject cube = Instantiate(cubes[Random.Range(0, cubes.Length)], points[Random.Range(0, points.Length)]);
                cube.transform.localPosition = Vector3.zero;
                cube.transform.Rotate(transform.forward, 90 * Random.Range(0, 4));
                count++;
                //Debug.Log(count);
                // Remove the recorded beat seconds.
                timer -= beat;
            }
            timer += Time.deltaTime;
        }  
    }

    public void SetSpawner()
    {
        CanSpawn = true;
    }

    public int GetCountCubes()
    {
        return count;
    }
}
