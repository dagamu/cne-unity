using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundLoop : MonoBehaviour
{
    
    public float scrollSpeed;
   
    void Update()
    {
        transform.localPosition += Vector3.left * -scrollSpeed * Time.deltaTime;
        if( transform.localPosition.x < -1240 )
        {
            transform.localPosition = Vector3.left * 580;
        }
    }
    
}