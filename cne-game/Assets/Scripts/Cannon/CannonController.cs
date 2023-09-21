using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public float radius;
    public float lerpDuration;
    public GameObject BulletPrefab;
    public float bulletForce;

    float timeElapsed;

    float targetAngle;
    float currentAngle;
    float initialAngle;

    float initialRotation;
    float targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        currentAngle = Mathf.PI / 2;
        initialAngle = currentAngle; 

        transform.position = new Vector3( radius, 0f, 0f );
        newTargetAngle();
    }

    public void newTargetAngle(){

        initialAngle = currentAngle;
        initialRotation = transform.rotation.eulerAngles.y;

        targetAngle = Random.Range( -Mathf.PI, Mathf.PI );
        targetRotation = ( targetAngle*Mathf.Rad2Deg + 90 ) + Random.Range( -30, 30 );
    }

    void throwBullet()
    {
        var newBullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        newBullet.transform.position += transform.up * 1f;
        newBullet.GetComponent<Rigidbody>().AddForce(transform.up * bulletForce);

    }

    // Update is called once per frame
    void Update()
    {

        if( timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;

            float newAngle = Mathf.Lerp(initialAngle, targetAngle, timeElapsed / lerpDuration);
            float newRotation = Mathf.Lerp(initialRotation, targetRotation, timeElapsed / lerpDuration);

            currentAngle = newAngle;

            transform.rotation = Quaternion.Euler(0f, newRotation, transform.rotation.eulerAngles.z);
            transform.position = new Vector3(Mathf.Sin(newAngle), 1 / radius, Mathf.Cos(newAngle)) * radius;
        } else
        {
            throwBullet();
            timeElapsed = 0;
            newTargetAngle();
        }
        
    }
}
