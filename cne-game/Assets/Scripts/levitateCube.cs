using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levitateCube : MonoBehaviour
{
    // Start is called before the first frame update
    
    float delay;

    void Start()
    {
        delay = Random.Range(0,10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3( transform.position.x, Mathf.Sin( Time.time * 3 + delay ) * 0.15f + 0.15f, transform.position.z );
        
    }
}
