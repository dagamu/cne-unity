using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallObstaclesSpawners : MonoBehaviour
{
    public float stepX = 3f;
    public float stepZ = 5;

    public int obstaclesNumber = 7;
    public List<GameObject> HallObstacles;

    void Start()
    {
        for (int i = 0; i < obstaclesNumber; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject toSpawn = HallObstacles[Random.Range(0, HallObstacles.Count )];
                var newObstacle = Instantiate(toSpawn, transform.position, Quaternion.identity);
                transform.position += new Vector3( stepX, 0, 0);
            }

            transform.position += new Vector3( -stepX * 2, 0, stepZ);
        }
    }


}
