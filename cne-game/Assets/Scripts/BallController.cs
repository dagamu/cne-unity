using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(-10.0f, 10.0f),0,Random.Range(-10.0f, 10.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
