using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boost : MonoBehaviour
{
    public float boostValue;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Car(Clone)")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(other.transform.forward * boostValue);
        }
    }

    
}
