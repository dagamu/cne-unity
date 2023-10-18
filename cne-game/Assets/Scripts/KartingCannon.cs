using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartingCannon : MonoBehaviour
{

    public List<GameObject> balls;
    public Transform spawnBall;
    public Transform Target;
    public float powerShoot;
    float timer;
    public float ballCd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > ballCd)
        {
            timer = 0;
            var newBall = Instantiate(balls[Random.Range(0,balls.Count)], spawnBall.position, Quaternion.identity, transform);
            newBall.transform.rotation = Quaternion.LookRotation( (Target.position - spawnBall.position).normalized );
            newBall.GetComponent<Rigidbody>().AddForce(newBall.transform.forward * powerShoot);

            Destroy(newBall, 10f);

        }

    }
}
