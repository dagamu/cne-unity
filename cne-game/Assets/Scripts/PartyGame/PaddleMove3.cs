using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleMove3 : MonoBehaviour
{


    public float speed;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()

    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector3(0, 0, speed);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector3(0, 0, -speed);
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
}

