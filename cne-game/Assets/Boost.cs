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
        if(other.transform.parent == playersParent)
        {
            Debug.Log(other.gameObject.name);
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.right * boostForce * Time.deltaTime, ForceMode.Impulse); 
        }
    }
}
