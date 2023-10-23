using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Car")
        {
            other.gameObject.GetComponent<CarController>().motorForce *= 10; 
        }
    }
}
