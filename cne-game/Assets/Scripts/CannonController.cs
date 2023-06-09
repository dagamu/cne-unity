using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public float radius;
    public float speed;
    public float spinSpeed;

    float targetAngle;
    float currentAngle;
    float targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        currentAngle = Mathf.PI/2;
        transform.position = new Vector3( radius, 0f, 0f );
        newTargetAngle();
    }

    public void newTargetAngle(){
        targetAngle = Random.Range( -Mathf.PI, Mathf.PI );
        targetRotation = ( targetAngle*Mathf.Rad2Deg + 90 ) + Random.Range( -30, 30 );
    }

    // Update is called once per frame
    void Update()
    {
        float newAngle = Mathf.Lerp( currentAngle, targetAngle, speed );
        float newRotation = Mathf.Lerp( transform.rotation.eulerAngles.y, targetRotation, spinSpeed );

        currentAngle = Mathf.Abs( newAngle - targetAngle ) < 1f * Mathf.Deg2Rad ? targetAngle : newAngle;

        transform.rotation = Quaternion.Euler( 0f, newRotation, transform.rotation.eulerAngles.z );
        transform.position = new Vector3( Mathf.Sin(newAngle), 1/radius, Mathf.Cos(newAngle) ) * radius;
    }
}
