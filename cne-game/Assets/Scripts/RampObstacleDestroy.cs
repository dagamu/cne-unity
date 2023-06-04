using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampObstacleDestroy : MonoBehaviour
{
    void Update()
    {
      if( transform.position.z < 10 || transform.position.y < 0.1f){
        Destroy( gameObject );
      }  
    }
}
