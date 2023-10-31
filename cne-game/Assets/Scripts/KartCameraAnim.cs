using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KartCameraAnim : MonoBehaviour
{
    public int currentIndex = 0;
    Transform currentStart, currentTarget;
    public Transform MovPoints;
    public UnityEvent setCameras;
    public GameObject Countdown, NextMapText;
    public float CountdownTime;

    float totalAnimTime;

    Vector3 vel;
    Vector3 rVel;
    void goTo( Transform target ){
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void hideCountdown(){ Countdown.SetActive(false); }
    
    void setNextMapText(){ NextMapText.SetActive(true); }

    void setCountdown(){
        
        Countdown.SetActive(true);
        this.Invoke( "hideCountdown", Countdown.GetComponent<Animator>()
                                    .GetCurrentAnimatorClipInfo(0)[0]
                                    .clip.length);
    }

    void Start(){newMove();}
    void newMove()
    {
        currentIndex = 0;
        goTo( MovPoints.GetChild(0) );
        currentStart = MovPoints.GetChild(0);
        currentTarget = MovPoints.GetChild(1);

        totalAnimTime = 0;
        foreach(Transform p in MovPoints ){
            if( p.GetSiblingIndex() % 2 == 0 || p.gameObject.name.Substring(0,1) == "T" ){
                totalAnimTime += p.localScale.x;
            }
        }

        if (!MovPoints.parent) Invoke("setCountdown", CountdownTime);
        else Invoke( "setNextMapText", 5f);

    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp( transform.position, currentTarget.position, ref vel, currentStart.localScale.x );
        transform.rotation = SmoothDampQuaternion(transform.rotation, currentTarget.rotation, ref rVel, currentStart.localScale.x);

        if( (transform.position - currentTarget.position).magnitude < 0.3 ){
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
                if (MovPoints.parent) newMove();
                else setCameras.Invoke();
            }
        }
    }

    public Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
    Vector3 c = current.eulerAngles;
    Vector3 t = target.eulerAngles;
    return Quaternion.Euler(
        Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
        Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
        Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
    );
    }
}
