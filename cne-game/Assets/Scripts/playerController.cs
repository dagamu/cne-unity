using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using System;
using gamePlayerSpace;

public class playerController : MonoBehaviour
{
    // Start is called before the first frame update

    public string gameId;
    public Color playerColor;
    
    [HideInInspector]
    public gamePlayer playerData;

    public float jumpForce = 4f,
                fallMultiplier = 2.5f,
                lowJumpMultiplier = 2f,
                speed = 30f,
                turnSmooth = 0.1f,
                smoothInputSpeed,
                takedObjectRadius,
                takedObjectInterpolation;

    float turnSmoothVel;

    GameObject takedObject;

    bool grounded, onBoard;

    Rigidbody rb;
    public GameObject cam, PointingSprite;
    Vector3 moveDirection;

    [HideInInspector]
    public GameObject targetPoint, newDice, UIBoardBox, playerModel;

    BoardManager boardManager;

    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera");

    }

    void Start()
    {

        onBoard = SceneManager.GetActiveScene().name == "Board";
        if( onBoard ){ 
            boardManager = GetComponent<BoardManager>();
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.name == "Pointing Sprite")
            {
                PointingSprite = child;
                Color SpriteColor = playerColor;
                SpriteColor.a = 0.4f;
                PointingSprite.GetComponent<SpriteRenderer>().color = SpriteColor;
            }
            else if (child.name == playerData.model + "(Clone)")
            {
                playerModel = child;
            }
        }
    }

    void Update()
    {

        var data = playerData.gamepadData;

        if( onBoard ){ boardManager.managePlayerOnBoard(); }

        Jump(data);
        if (!onBoard) { Move(data); }


        BetterJump(data[3] == 0);
        SpeedControl();

        manageTakedObject();

    }
    
    private void Move(float[] data)
    {

        Vector3 direction = new Vector3(
           data[0], 0f, data[1]
       );
       //currentInputVector = Vector3.SmoothDamp(currentInputVector, direction, ref smoothInputVelocity, smoothInputSpeed );
       currentInputVector = direction;

        float targetAngle = Mathf.Atan2(currentInputVector.x, currentInputVector.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmooth);

        if (data[0] != 0 && data[1] != 0){
            transform.rotation = Quaternion.Euler(0f, angle, 0f); }
        else { rb.angularVelocity = Vector3.zero; }

        moveDirection = transform.forward * data[1] + transform.right * data[0];

        rb.AddRelativeForce(Vector3.forward * speed * moveDirection.magnitude * Time.deltaTime, ForceMode.Force);

        playerModel.GetComponent<Animator>().SetBool("Running", moveDirection.magnitude > 0.1);

    }

    private void Jump(float[] data)
    {

        if (data[3] == 1 && grounded)
        {
            if(onBoard){
                if ( boardManager.rolling ) { boardManager.rollingTrigger( gameObject, playerData ); }
            }
            
            playerModel.GetComponent<Animator>().SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }

    }

    private void manageTakedObject()
    {
        if (takedObject != null && Vector3.Distance(takedObject.transform.position, transform.position) > takedObjectRadius)
        {
            Vector3 newPos = Vector3.Lerp(takedObject.transform.position, transform.position, takedObjectInterpolation);
            takedObject.transform.position = new Vector3(newPos.x, takedObject.transform.position.y, newPos.z);
        }
    }

    private void BetterJump(bool jumpBtn)
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity = Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && jumpBtn)
        {
            rb.velocity = Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void SpeedControl()
    {

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

    }

    public void deletePlayer() { Destroy(gameObject); }

    
    public void setColor(string colorStr)
    {
        playerColor = Utility.stringToColor( colorStr );
    }

    void OnTriggerEnter(Collider collision)
    {
        

        if (collision.gameObject.CompareTag("Takable") && takedObject == null) { takedObject = collision.gameObject; }
        else if (collision.gameObject.CompareTag("TakableBase")){
            Debug.Log(collision.gameObject.name + "; " + collision.transform.GetSiblingIndex() + 1);
            if (collision.gameObject.name == "Base" + (collision.transform.GetSiblingIndex() + 1).ToString())
            {
                var aux = takedObject;
                takedObject = null;
                aux.transform.position += new Vector3(-2, 0, 0);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { grounded = true; }

    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { grounded = false; }
  
    }

}
