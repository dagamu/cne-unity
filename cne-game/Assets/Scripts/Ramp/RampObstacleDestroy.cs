using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RampObstacleDestroy : MonoBehaviour
{
    public List<GameObject> ObstacleList = new List<GameObject>();
    void Start()
    {
        var Obstacle = ObstacleList[(int) Mathf.Floor(Random.Range(0,ObstacleList.Count))];
        Instantiate(Obstacle, transform);
    }
    void Update()
    {
      if( transform.position.z < 10 || transform.position.y < 0.1f){
        Destroy( gameObject );
      }  
    }
}
