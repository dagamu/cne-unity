using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using gamePlayerSpace;

namespace gamePlayerSpace
{
    public class gamePlayer {

        public string id;
        public string color;
        public string model;
        public int turnRoll = 0;
        public int turn = 0;

        public GameObject currentBoardPoint;

        public float[] gamepadData = new float[6] {0, 0, 0, 0, 0, 0} ;

        public gamePlayer( string id, string color ){
            this.id = id;
            this.color = color;
        }

        public void updateTurn( int turn ){
            this.turn = turn;
        }

    }
}

public class GamepadConnect : MonoBehaviour
{

    WebSocket ws;

    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>(); 
    private GameObject playerSpawn;
    public List<gamePlayer> players = new List<gamePlayer>();
    
    bool EmitConnection = false;

    public static GamepadConnect gamepadConnect;

    // Start is called before the first frame update
    void Start()
    {
        if( gamepadConnect == null ){
            gamepadConnect = this;
            DontDestroyOnLoad( gameObject );
        } else { Destroy(this); }

        Application.runInBackground = true;
        playerSpawn = GameObject.Find("playerSpawn");

        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) => {
            //Debug.Log("Message received, Data : " + e.Data);

            string[] sMsg = e.Data.Split(":");
            if( sMsg[0] == "newPlayer"){
                string[] data = sMsg[1].Split(',');
                _actions.Enqueue(() => createPlayer(data[0], data[1]));
            } else if( sMsg[0] == "Move"){
                _actions.Enqueue(() => PlayerMove(sMsg[1]));
            } else if( sMsg[0] == "Delete"){
                _actions.Enqueue(() => deletePlayer(sMsg[1]));
            }

            
        };
        ws.Connect();

        
    }

    /* void Awake(){
        playerSpawn = GameObject.Find("playerSpawn");
        foreach( var player in players ){
            Debug.Log(player.id + " " + player.color + " " + player.model);
            playerSpawn.SendMessage("InstantiatePlayer", new string[3]{ player.id, player.color, player.model }  );
        } */
    

    private void createPlayer( string id, string color ) 
    {
        playerSpawn.SendMessage("NewPlayer", new string[2]{ id, color }  );

        gamePlayer newPlayer = new gamePlayer( id, color );
        players.Add(newPlayer);

    }

    private void PlayerMove( string data ){
        string[] sData = data.Split(",");

        players.ForEach( delegate( gamePlayer p) {
            if(sData[0] == p.id ){
                p.gamepadData = new float[6] {
                    float.Parse( sData[1] ),
                    float.Parse( sData[2] ),
                    Convert.ToBoolean( sData[3] ) ? 1 : 0,
                    Convert.ToBoolean( sData[4] ) ? 1 : 0,
                    Convert.ToBoolean( sData[5] ) ? 1 : 0,
                    Convert.ToBoolean( sData[6] ) ? 1 : 0
                };
            }
        });

    }

    private void deletePlayer( string id ){

        players.ForEach( delegate( gamePlayer p ) {
            if(id == p.id ){
                p = null;
                /*var multipleTarget = GetComponent<Camera>().GetComponent<MultipleTargetCamera>();
                multipleTarget.targets.Remove(p.gameObject.transform);*/
            }
        });

        

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

        if( !EmitConnection && ws != null ){
            ws.Send("UnityConnection");
            EmitConnection = true;
        }
        
    }
}
