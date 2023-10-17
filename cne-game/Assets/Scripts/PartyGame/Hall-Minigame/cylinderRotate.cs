using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cylinderRotate : MonoBehaviour
{
    
    float delay;
    int direction;
    Vector3 startPosition;

    void Start()
    {
        delay = Random.Range(0,100);
        startPosition = transform.position;
        direction = Random.Range(0,1);
        if( direction == 0){
            direction = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + new Vector3( 
            0,
            Mathf.Sin( Time.time * 3 + delay ) * 0.1f * direction,
            Mathf.Cos( Time.time * 3 + delay ) * 0.1f
            );
        
    }
}
