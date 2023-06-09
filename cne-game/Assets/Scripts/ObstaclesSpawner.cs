using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    public GameObject Obstacle; 
    public float minX;
    public float maxX;

    public float TimeBetweenSpawn;
    private float SpawnTime;

    void Update()
    {

        if (Time.time > SpawnTime)
        {
            Spawn();
            SpawnTime = Time.time + TimeBetweenSpawn;
        }

    }

    void Spawn()
    {
        float X = Random.Range(minX, maxX);
        Quaternion newRotation = Random.rotation;

        GameObject newObstacle = Instantiate(Obstacle, transform.position + new Vector3(X, 0, 0), newRotation);
        newObstacle.transform.parent = gameObject.transform;

    }
}
