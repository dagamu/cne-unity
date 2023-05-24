using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;
using System;



public class playerController : MonoBehaviour
{
    // Start is called before the first frame update
    public string gameId;
    public Color playerColor;
    public float velocity = 10f;
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    WebSocket ws;
    Rigidbody rb;
    SpriteRenderer sr;

    Vector3 moveDirection;
    public Transform orientation;

    public void setId( string id ){
        gameId = id;
    }
    public void setColor( string colorStr ){
        string[] colorList = colorStr.Split(';');
        playerColor = new Color( 
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > velocity)
        {
            Vector3 limitedVel = flatVel.normalized * velocity;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void deletePlayer(){
        Destroy(gameObject);
    }
    public void playerMove( string[] data ){

        float[] joyAxis = {0f, 0f};

        joyAxis[0] = float.Parse(data[1]);
        joyAxis[1] = float.Parse(data[2]);

        moveDirection = orientation.forward * joyAxis[1] + orientation.right * joyAxis[0];
        rb.AddForce(moveDirection.normalized * velocity * 10f, ForceMode.Force);



        /*
        rb.velocity = new Vector3( 
            velocity * joyAxis[0] * Time.deltaTime,
            velocity * joyAxis[1] * Time.deltaTime,
             0 
            ) ;*/
    }

    
    void Awake(){
        rb = GetComponent<Rigidbody> ();
        sr = GetComponent<SpriteRenderer> ();
        
    }

    void Start()
    {
        //rb.AddForce( new Vector3( -100f * Time.deltaTime, 0, 0 ) );  
        /*ws = new WebSocket("wss://localhost:8080/");
        ws.OnMessage += (sender, e) =>
        {
            string[] sMsg = e.Data.Split(",");
            Debug.Log(e.Data);
            if( sMsg[0] == gameId){
                _actions.Enqueue(() => socketController(sMsg));
            }
        };   
        ws.Connect();*/
    }

    // Update is called once per frame
    void Update()
    {
        while(_actions.Count > 0)
            {
                if(_actions.TryDequeue(out var action))
                {
                    action?.Invoke();
                }
            }
        if( Input.GetKeyDown(KeyCode.Space) ){
            ws.Send("Hello");
        }

        SpeedControl();
    }

    private static void socketController(string[] data) 
    {

        Debug.Log(data[1] + " " + data[2]);

        /*

        moves[0] = float.Parse(moves[0]);
        moves[1] = float.Parse(moves[1]);

        rb.velocity = new Vector3( 
            velocity * moves[0] * Time.deltaTime,
            velocity * moves[1] * Time.deltaTime,
             0 
            ) ; */
    }
}
