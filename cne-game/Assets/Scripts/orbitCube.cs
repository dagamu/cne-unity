using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitCube : MonoBehaviour
{

    public int level;
    float delay;
    int direction;
    
    // Start is called before the first frame update
    void Start()
    {
        delay = Random.Range(0,100);
        direction = 1;
        if( level % 2 == 0 ){
            direction = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + new Vector3( 
            Mathf.Sin( Time.time * 3 + delay * level ) * 0.1f * level * direction,
            0,
            Mathf.Cos( Time.time * 3 + delay * level ) * 0.1f * level
            );
        
    }
}
