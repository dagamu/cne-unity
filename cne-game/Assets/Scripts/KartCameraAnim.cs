using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KartCameraAnim : MonoBehaviour
{
    int currentIndex = 0;
    Transform currentStart, currentTarget;
    public Transform MovPoints;

    public UnityEvent setCameras;

    Vector3 vel;
    Vector3 rVel;
    void goTo( Transform target ){
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
    void Start()
    {
        goTo( MovPoints.GetChild(0) );
        currentStart = MovPoints.GetChild(0);
        currentTarget = MovPoints.GetChild(1);
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp( transform.position, currentTarget.position, ref vel, currentStart.localScale.x );
        transform.rotation = Quaternion.Euler( Vector3.SmoothDamp(transform.rotation.eulerAngles, currentTarget.eulerAngles, ref rVel, currentStart.localScale.x));

        if( (transform.position -currentTarget.position).magnitude < 0.3 ){
            currentIndex += 1;
            if( currentIndex + 1 < MovPoints.childCount ){
                if( MovPoints.GetChild(currentIndex+1).gameObject.name.Substring(0,1) == "T" ){
                    goTo( MovPoints.GetChild(currentIndex+1) );
                    transform.position += transform.forward * 0.5f;
                    currentStart = MovPoints.GetChild(currentIndex+1);
                    currentTarget = currentStart;
                } else {
                    currentStart = MovPoints.GetChild(currentIndex+1);
                    currentTarget = MovPoints.GetChild(currentIndex+2);
                    goTo( currentStart );
                    currentIndex += 1;
                }
                
            } else {
                //Start Race
                setCameras.Invoke();
            }
        }
    }
}
