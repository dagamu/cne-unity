using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
      if( transform.position.z < 10 || transform.position.y < 0.1f){
        Destroy( gameObject );
      }  
    }
}
