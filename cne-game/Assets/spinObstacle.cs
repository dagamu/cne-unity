using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinObstacle : MonoBehaviour
{

    public Vector3 spinSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinSpeed);
    }
}
