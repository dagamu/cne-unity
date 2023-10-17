using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBulletDestroy : MonoBehaviour
{


    public float radius; 

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        if( pos.magnitude > radius)
        {
            Destroy(gameObject);
        }
    }
}
