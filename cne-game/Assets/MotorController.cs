using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour
{
    public GameObject carParent; 
    private void OnTriggerEnter(Collider other)
    {
        carParent.GetComponent<RaceCarController>().triggerController( other );
    }
}
