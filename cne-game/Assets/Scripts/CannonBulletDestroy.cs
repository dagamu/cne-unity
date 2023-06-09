using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnBorder : MonoBehaviour
{

    void OnCollisionEnter(Collision col)
    {
        if( col.collider.CompareTag("Border")) { Destroy(gameObject); }
    }
}
