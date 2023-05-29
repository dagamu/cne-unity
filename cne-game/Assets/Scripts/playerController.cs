using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;
using System;
using gamePlayerSpace;

public class playerController : MonoBehaviour
{
    // Start is called before the first frame update
    public string gameId;
    public Color playerColor;
    public CharacterController cController;
    public gamePlayer playerData;
    public Transform orientation;

    public float speed = 30f;
    public float turnSmooth = 0.1f;
    public float sensX = 10;
    float turnSmoothVel;
    
    WebSocket ws;
    Rigidbody rb;
    SpriteRenderer sr;

    GameObject PointingSprite;
    Vector3 moveDirection;

    public void setColor( string colorStr ){
        string[] colorList = colorStr.Split(';');
        playerColor = new Color( 
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );
        
    }


    public void deletePlayer(){
        Destroy(gameObject);
    }
    
    void Awake(){
        rb = GetComponent<Rigidbody> ();
        
    }

    void Start()
    {
       for (int i = 0; i < gameObject.transform.childCount; i++){

    GameObject child = gameObject.transform.GetChild(i).gameObject;
    if( child.name == "Pointing Sprite"){
        PointingSprite = child;
        Color SpriteColor = playerColor;
        SpriteColor.a = 0.4f;
        PointingSprite.GetComponent<SpriteRenderer>().color = SpriteColor;
    }
}
    }

    // Update is called once per frame
    void Update()
    {

        var data = playerData.gamepadData;

        Vector3 direction = new Vector3(
            data[0], 0f, data[1]
        ).normalized;

        float targetAngle = Mathf.Atan2( direction.x, direction.z ) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle( 
            transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmooth );
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        moveDirection = transform.forward * data[1] + transform.right * data[0];
        rb.AddRelativeForce(Vector3.forward * speed * moveDirection.magnitude, ForceMode.Force );

        SpeedControl();
    }

    private void SpeedControl(){
        Vector3 flatVel = new Vector3( rb.velocity.x, 0f, rb.velocity.z );
        if( flatVel.magnitude > speed ){
            Vector3 limitedVel = flatVel.normalized*speed;
            rb.velocity = new Vector3( limitedVel.x, rb.velocity.y, limitedVel.z );
        }
    }

    
}
