using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountObjectSpawner : MonoBehaviour
{
    public List<GameObject> ObjectsToCount = new List<GameObject>();
    public List<int> FinalCounts = new List<int>();
    List<int> toSpawnLeft;

    public float introTime, spawnRange, maxSpawnTime;
    public int minToSpawn, maxToSpawn;

    float timer;
    float currentSpawnProb = 0.03f;
    float lastSpawn = 0f;

    void Start()
    {
        foreach( var obj in ObjectsToCount)
            FinalCounts.Add(Random.Range(minToSpawn, maxToSpawn));
      
        toSpawnLeft = new List<int>(FinalCounts);
    }

    
    void Update()
    {
        timer += Time.deltaTime;
        if( timer > introTime)
        {
            if( Random.value < currentSpawnProb && timer > lastSpawn + 0.2f)
            {
                var objIndex = Random.Range(0, ObjectsToCount.Count);
                if (toSpawnLeft[objIndex] > 0) {
                    var newObj = Instantiate(ObjectsToCount[objIndex]);
                    newObj.transform.parent = transform.parent;
                    newObj.transform.position = transform.position + new Vector3(0, 0, Random.Range(-spawnRange / 2, spawnRange / 2));
                    toSpawnLeft[objIndex]--;
                }
                lastSpawn = timer;

                //Debug.Log(string.Format("{0}, {1}, {2}, {3}", timer, toSpawnLeft[0], toSpawnLeft[1], toSpawnLeft[2]));
            }

            if( timer > maxSpawnTime)
                currentSpawnProb = 1;
                   
        }
    }
}
