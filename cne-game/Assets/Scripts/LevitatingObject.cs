using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitatingObject : MonoBehaviour
{
    
    public float yRange;
    public float speed;
    float startY;
    float timer;

    void Start(){
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.up * Mathf.Sin( timer * speed ) * yRange;
    }
}
