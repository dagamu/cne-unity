using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    public GameObject Obstacle; //Arrastrar Prefab de Obstáculo Aquí
    public float minX;
    public float maxX;

    public float TimeBetweenSpawn;
    private float SpawnTime;

    void Start() { }

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

        Instantiate(Obstacle, transform.position + new Vector3(X, 0, 0), newRotation);
    }
}
