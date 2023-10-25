using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuModels : MonoBehaviour
{
    public float spinSpeed;
    public float changeModelTime;
    int i = 0;
    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        var lookingBack = transform.rotation.eulerAngles.y < -90 || transform.rotation.eulerAngles.y > 90;
        var speed = !lookingBack ? spinSpeed : spinSpeed * 5;
        transform.Rotate(0,speed * Time.deltaTime,0);
        if( timer > changeModelTime){
            timer = 0;
            transform.GetChild(i).gameObject.SetActive(false);
            i = i == transform.childCount - 1 ? 0 : i+1;
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
