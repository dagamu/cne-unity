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
    public gamePlayer playerData;

    public float jumpHeight = 1f;
    public LayerMask whatIsGround;
    bool grounded;

    public float speed = 30f;
    public float groundDrag = 0;
    public float turnSmooth = 0.1f;
    float turnSmoothVel;

    WebSocket ws;
    Rigidbody rb;
    GameObject playerModel;

    GameObject PointingSprite;
    Vector3 moveDirection;

    public void setColor(string colorStr) {
        string[] colorList = colorStr.Split(';');
        playerColor = new Color(
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );

    }


    public void deletePlayer() {
        Destroy(gameObject);
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++) {

            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.name == "Pointing Sprite")
            {
                PointingSprite = child;
                Color SpriteColor = playerColor;
                SpriteColor.a = 0.4f;
                PointingSprite.GetComponent<SpriteRenderer>().color = SpriteColor;
            }
            else if (child.name == playerData.model + "(Clone)") {
                playerModel = child;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        var data = playerData.gamepadData;
        grounded = Physics.Raycast(transform.position, Vector3.down, 2 * 0.5f + 0.3f, whatIsGround);

        if (data[3] == 1 && grounded )
        {
            playerModel.GetComponent<Animator>().SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        Vector3 direction = new Vector3(
            data[0], 0f, data[1]
        ).normalized;

        float targetAngle = Mathf.Atan2( direction.x, direction.z ) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle( 
            transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmooth );
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        moveDirection = transform.forward * data[1] + transform.right * data[0];
        rb.AddRelativeForce(Vector3.forward * speed * moveDirection.magnitude, ForceMode.Force );

        playerModel.GetComponent<Animator>().SetBool("Running", moveDirection.magnitude > 0.1 );


        SpeedControl();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void SpeedControl(){
        Vector3 flatVel = new Vector3( rb.velocity.x, 0f, rb.velocity.z );
        if( flatVel.magnitude > speed ){
            Vector3 limitedVel = flatVel.normalized*speed;
            rb.velocity = new Vector3( limitedVel.x, rb.velocity.y, limitedVel.z );
        }
    }

}
