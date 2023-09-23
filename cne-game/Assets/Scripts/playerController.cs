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
                takedObjectRadius,
                takedObjectInterpolation;

    float turnSmoothVel;

    GameObject takedObject;

    bool grounded, onBoard;

    Rigidbody rb;
    GameObject cam, playerModel, PointingSprite ;
    Vector3 moveDirection, autoMove;

    int boardStepsLeft = 0;

    [HideInInspector]
    public GameObject currentBoardPoint, targetPoint, newDice, UIBoardBox;

    BoardManager boardManager;

    public void setColor(string colorStr)
    {
        string[] colorList = colorStr.Split(';');
        playerColor = new Color(
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );

    }


    public void deletePlayer() { Destroy(gameObject); }

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

    /*  
    Vector2 nearPoint = new Vector2(0, 0);
    void managePathSelection()
    {

        var boardPoint = currentBoardPoint.GetComponent<BoardPointManager>();

        if (boardPoint.nextPoints.Count > 0 && nearPoint.x < boardPoint.nextPoints.Count)
        {
            targetPoint = boardPoint.nextPoints[(int)nearPoint.x];
        }

        if (boardPoint.nextPoints.Count > 1)
        {

            for (int i = 0; i < boardPoint.nextPoints.Count; i++)
            {
                Vector2 mouseDir = new Vector2(playerData.gamepadData[0], playerData.gamepadData[1]);
                Vector2 nPointDir = new Vector2(
                    boardPoint.nextPoints[i].transform.position.x - transform.position.x,
                    boardPoint.nextPoints[i].transform.position.y - transform.position.y
                    );

                if (Vector2.Angle(mouseDir, nPointDir) > nearPoint.y)
                {
                    nearPoint.x = i;
                    nearPoint.y = Vector2.Angle(mouseDir, nPointDir);
                }

                boardPoint.pathLines[(int)i].GetComponent<LineRenderer>().material.color = Color.white;
            }

            boardPoint.pathLines[(int)nearPoint.x].GetComponent<LineRenderer>().material.color = playerColor;

        }
        else if (boardPoint.nextPoints.Count > 0)
        {

            if (boardPoint.pathLines.Count > 0)
            {
                boardPoint.pathLines[0].GetComponent<LineRenderer>().material.color = playerColor;
            }

            var nextPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];

            if (movingBoard)
            {
                targetPoint = nextPoint;
            }
        }

        if (playerData.gamepadData[2] == 1 )
        {

            chosingPath = false;
            movingBoard = true;
            rb.velocity = Vector3.zero;
            targetPoint = boardPoint.nextPoints[(int)nearPoint.x];
            targetPoint.GetComponent<BoardPointManager>().showLine();

        }

        Vector3 dis = targetPoint.transform.position - transform.position;
        if (dis.magnitude < 0.5f)
        {

            currentBoardPoint = targetPoint;

            if (currentBoardPoint.GetComponent<BoardPointManager>().nextPoints.Count != 1)
            {

                movingBoard = false;
                rb.velocity = Vector3.zero;
                playerModel.GetComponent<Animator>().SetBool("Running", false);
                targetPoint = null;

            }

            boardPoint.hideLine();
        }
        if (movingBoard)
        {

            var normVel = Vector3.Normalize(dis);
            autoMove = new Vector3(normVel.x, 0, normVel.z);
            rb.velocity = autoMove * speed * 0.7f * Time.deltaTime;
            playerModel.GetComponent<Animator>().SetBool("Running", autoMove.magnitude > 0.1);

        }

    }
    */

    private void Move(float[] data)
    {

        Vector3 direction = new Vector3(
           data[0], 0f, data[1]
       ).normalized;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
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
            if ( boardManager.rolling ) { boardManager.rollingTrigger( gameObject, playerData ); }
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

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Takable") && takedObject == null) { takedObject = collision.gameObject; }

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
