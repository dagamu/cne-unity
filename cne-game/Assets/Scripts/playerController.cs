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
    public gamePlayer playerData;

    public float jumpForce = 4f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    bool grounded;

    public float speed = 30f;
    public float turnSmooth = 0.1f;
    float turnSmoothVel;

    GameObject takedObject;
    public float takedObjectRadius;
    public float takedObjectInterpolation;


    Rigidbody rb;
    GameObject cam;
    GameObject playerModel;

    GameObject PointingSprite;
    Vector3 moveDirection;

    bool rolling = false;
    public GameObject diceObj;
    public GameObject rollText;

    public void setColor(string colorStr) {
        string[] colorList = colorStr.Split(';');
        playerColor = new Color(
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );

    }


    public void deletePlayer() { Destroy(gameObject);}

    void Awake() {
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera");

    }

    GameObject newDice;
    void startRoll(){

        rolling = true;

        newDice = Instantiate(diceObj);
        //newDice.transform.parent = transform;
        newDice.transform.position = new Vector3( transform.position.x, 3, transform.position.z) ;

        Rigidbody diceRb = newDice.GetComponent<Rigidbody>();
        diceRb.angularVelocity = new Vector3(UnityEngine.Random.Range(-200, 200),
                                UnityEngine.Random.Range(-200, 200),
                                UnityEngine.Random.Range(-200, 200)); 

    }

    void Start()
    {

        if( SceneManager.GetActiveScene().name == "Board" ){
            if( playerData.turn == 0 ){
                startRoll();
            }
        }

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

    void Update()
    {

        var data = playerData.gamepadData;

        Jump( data ); 
        Move( data );


        BetterJump( data[3] == 0 );
        SpeedControl();

        manageTakedObject();

    }

    private void Move( float[] data ){

         Vector3 direction = new Vector3(
            data[0], 0f, data[1]
        ).normalized;

        float targetAngle = Mathf.Atan2( direction.x, direction.z ) * Mathf.Rad2Deg + cam.transform.eulerAngles.y ;
        float angle = Mathf.SmoothDampAngle( 
            transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmooth );
            
        if( data[0] != 0 && data[1] != 0){
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        } else { rb.angularVelocity = Vector3.zero; }

        moveDirection = transform.forward * data[1] + transform.right * data[0];
        rb.AddRelativeForce(Vector3.forward * speed * moveDirection.magnitude * Time.deltaTime, ForceMode.Force );

        playerModel.GetComponent<Animator>().SetBool("Running", moveDirection.magnitude > 0.1 );
        
    }

    private void Jump( float[] data ){

        if ( data[3] == 1 && grounded )
        {
            if( rolling ){

                rolling = false;

                Debug.Log("Jump");  
                float rollNum = Mathf.Floor( UnityEngine.Random.Range( 1, 6 ) ) ; 
                GameObject rollTag = Instantiate( rollText );
                rollTag.transform.position = transform.position + Vector3.up*3;
                rollTag.transform.parent = transform;
                rollTag.GetComponent<TextMesh>().text = rollNum.ToString();
                Destroy( newDice );
                Destroy( rollTag, 2f);

            }
            

            playerModel.GetComponent<Animator>().SetTrigger("Jump" );
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }

    }

    private void manageTakedObject(){
        if( takedObject != null && Vector3.Distance( takedObject.transform.position, transform.position ) > takedObjectRadius ){
            Vector3 newPos = Vector3.Lerp( takedObject.transform.position, transform.position, takedObjectInterpolation );
            takedObject.transform.position = new Vector3( newPos.x, takedObject.transform.position.y, newPos.z);
        }
    }

    private void BetterJump( bool jumpBtn ){
        if( rb.velocity.y < 0 ){
            rb.velocity = Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if ( rb.velocity.y > 0 && jumpBtn ){
            rb.velocity = Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void SpeedControl(){

        Vector3 flatVel = new Vector3( rb.velocity.x, 0f, rb.velocity.z );
        if( flatVel.magnitude > speed ){
            Vector3 limitedVel = flatVel.normalized*speed;
            rb.velocity = new Vector3( limitedVel.x, rb.velocity.y, limitedVel.z );
        }

    }

    void OnTriggerEnter( Collider collision ){
        if ( collision.gameObject.CompareTag("Takable") && takedObject == null ){ takedObject = collision.gameObject; }
        
    }

    void OnCollisionEnter( Collision collision ){
        if (collision.gameObject.CompareTag("Ground")){ grounded = true; }
        
    }

    

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { grounded = false; }
    }

}
