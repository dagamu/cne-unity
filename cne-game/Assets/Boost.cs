using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{

    public float boostForce;
    Transform playersParent;
    void Start(){
        playersParent = GameObject.Find("playerSpawn").transform;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Motor")
        {
            Debug.Log(other.gameObject.name);
            other.GetComponent<Rigidbody>().AddForce(transform.right * boostForce * Time.deltaTime, ForceMode.Impulse); 
        }
    }
}
