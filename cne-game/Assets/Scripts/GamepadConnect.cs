using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using gamePlayerSpace;

namespace gamePlayerSpace
{
    public class gamePlayer {

        public string id;
        public string color;
        public string model = "default";
        public int turnRoll = 0;
        public int turn = 0;

        public Vector3 currentBoardPoint = new Vector3(0,-1,0);

        public float[] gamepadData = new float[6] {0, 0, 0, 0, 0, 0} ;

        public gamePlayer( string id, string color ){
            this.id = id;
            this.color = color;
        }

        public void updateTurn( int turn ){
            this.turn = turn;
        }

        public void updateCurrentBoardPos( Vector3 newPos ){
            this.currentBoardPoint = newPos;
        }

    }
}

public class GamepadConnect : MonoBehaviour
{
    WebSocket ws;

    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>(); 
    private GameObject playerSpawn;
    public List<gamePlayer> players = new List<gamePlayer>();
    public GameObject GamepadCanvasPrefab;
    public GameObject GamepadCanvas;
    
    bool EmitConnection = false;
    public bool DevMode;
    public int currentTurn = 1;

    public static GamepadConnect gamepadConnect;

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

    public void sendModel() {
        var msg = "ModelChange:";
        foreach(gamePlayer player in players){
            msg += player.id + "," + player.model+",";
        }
        ws.Send(msg);
    }

    /* void Awake(){
        playerSpawn = GameObject.Find("playerSpawn");
        foreach( var player in players ){
            Debug.Log(player.id + " " + player.color + " " + player.model);
            playerSpawn.SendMessage("InstantiatePlayer", new string[3]{ player.id, player.color, player.model }  );
        } */
    

    private void createPlayer( string id, string color ) 
    {
        if(SceneManager.GetActiveScene().name == "Main menu") playerSpawn.SendMessage("NewPlayer", new string[2]{ id, color }  );

        gamePlayer newPlayer = new gamePlayer( id, color );
        players.Add(newPlayer);

    }

    private void PlayerMove( string data ){
        string[] sData = data.Split(",");

        players.ForEach( delegate( gamePlayer p) {
            if(sData[0] == p.id ){
                float[] newData = new float[6] {
                    float.Parse( sData[1] ) / 100,    // X axis
                    float.Parse( sData[2] ) / 100,    // Y axis
                    Convert.ToBoolean( sData[3] ) ? 1 : 0,  //Up
                    Convert.ToBoolean( sData[4] ) ? 1 : 0,  //Down
                    Convert.ToBoolean( sData[5] ) ? 1 : 0,  //Left
                    Convert.ToBoolean( sData[6] ) ? 1 : 0   //Right
                };

                int playerIndex = players.IndexOf(p);

                newData = manageData(newData, playerIndex);

                if( GamepadCanvas != null ){
                    GamepadCanvas.GetComponent<DebugController>()
                    .UpdateMovementData( playerIndex, newData );
                }

                p.gamepadData = newData;
            }
        });
    }

    private void deletePlayer( string id ){
        players.ForEach( delegate( gamePlayer p ) {
            if(id == p.id ){
                p = null;
            }
        });
    }

    Vector2[] currentDir = { new Vector2(), new Vector2(), new Vector2(), new Vector2() };
    Vector2 smoothDir;
    public float dirSmooth;
    float[] manageData( float[] data, int playerIndex ){

        Vector2 dir = new Vector2(data[0],data[1]);
        currentDir[playerIndex] = Vector2.SmoothDamp(currentDir[playerIndex], dir, ref smoothDir, dirSmooth );

        data[0] = currentDir[playerIndex].x > 1 ? 1 : currentDir[playerIndex].x;
        data[1] = currentDir[playerIndex].y > 1 ? 1 : currentDir[playerIndex].y;

        return data;
    }

    // Update is called once per frame
    void Update()
    {
        while(_actions.Count > 0)
        {
            if( _actions.TryDequeue(out var action) ){ action?.Invoke(); }
        }

        if( !EmitConnection && ws != null ){
            ws.Send("UnityConnection");
            EmitConnection = true;
        }
        
        if( Input.GetKeyUp(KeyCode.F3) ){
            if( GamepadCanvas == null ){
                GamepadCanvas = Instantiate(GamepadCanvasPrefab);
                GamepadCanvas.GetComponent<DebugController>().setGamepadBoxes(players);
            } else {
                Destroy(GamepadCanvas);
            }
        }

        if( Input.GetMouseButtonDown(2) ){
            DevMode = !DevMode;
        }

        if( DevMode && players.Count != 0){

            // Left Click -> Right btn
            players[0].gamepadData[2] = Input.GetMouseButton(1) ? 1 : 0;
        
            // Right  Click -> Up btn
            players[0].gamepadData[5] = Input.GetMouseButton(0) ? 1 : 0;
        
            // E -> Left btn
            players[0].gamepadData[4] = Input.GetKeyUp("e") ? 1 : 0;
        
            // Space -> Down bnt
            players[0].gamepadData[3] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        
            if( Input.GetAxis("Horizontal") != 0 ){
                players[0].gamepadData[0] = Input.GetAxis("Horizontal");
            }
            if( Input.GetAxis("Vertical") != 0 ){
                players[0].gamepadData[1] = Input.GetAxis("Vertical");
            }

            if( GamepadCanvas != null ){
                GamepadCanvas.GetComponent<DebugController>().UpdateMovementData( 0, players[0].gamepadData );
            }
        }
        
    }
}
