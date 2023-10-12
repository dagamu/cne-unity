using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountObjectManager : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "ObjectKiller")
            Destroy(gameObject);
    }
}
