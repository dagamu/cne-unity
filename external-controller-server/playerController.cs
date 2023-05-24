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
    public float velocity = 0.1f;
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    WebSocket ws;
    Rigidbody2D rb;
    SpriteRenderer sr;

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
    public void deletePlayer(){
        Destroy(gameObject);
    }
    public void playerMove( string[] data ){

        float[] joyAxis = {0f, 0f};

        joyAxis[0] = float.Parse(data[1]);
        joyAxis[1] = float.Parse(data[2]);

         rb.AddForce( new Vector3( 
             velocity * joyAxis[0] * Time.deltaTime,
             velocity * joyAxis[1] * Time.deltaTime,
             0
        ));

        if( data[3] == "true" ){
            sr.color = new Color(0f, 0f, 0f, 1f);
        } else if( data[4] == "true" ){
            sr.color = new Color(1f, 0f, 0f, 1f);
        } else if( data[5] == "true" ){
            sr.color = new Color(0f, 1f, 0f, 1f);
        }else if( data[6] == "true" ){
            sr.color = new Color(0f, 0f, 1f, 1f);
        }else {
            sr.color = playerColor;
        }

        /*
        rb.velocity = new Vector3( 
            velocity * joyAxis[0] * Time.deltaTime,
            velocity * joyAxis[1] * Time.deltaTime,
             0 
            ) ;*/
    }

    
    void Awake(){
        rb = GetComponent<Rigidbody2D> ();
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
