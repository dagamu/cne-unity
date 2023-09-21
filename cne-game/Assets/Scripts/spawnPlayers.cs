using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using gamePlayerSpace;

public class spawnPlayers : MonoBehaviour
{

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    public GameObject playerPrefab;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {

        cam = (Camera) GameObject.FindObjectOfType(typeof(Camera));

        GamepadConnect = GameObject.Find("GamepadConnect");
        if( GamepadConnect != null ){ 
        
            gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();

            var i = 0;
            foreach( gamePlayer player in gamepadConnectComponent.players ){
                Vector3 newPosition = GameObject.Find("SpawnPoints").transform.GetChild(i).transform.position;
                InstantiatePlayer( player, newPosition );
                i++;
            }
        }

        

    }

    public void InstantiatePlayer( gamePlayer playerObj, Vector3 pos ){

        GameObject characterPrefab = Resources.Load<GameObject>("Characters/"+playerObj.model);
        var player = Instantiate(playerPrefab) as GameObject;
        var newPlayerController = player.GetComponent<playerController>();
        var playerModel = Instantiate(characterPrefab) as GameObject;
       
        player.transform.parent = gameObject.transform;
        player.transform.position = pos;
        playerModel.transform.position = pos;
        playerModel.transform.parent = player.transform;

        newPlayerController.gameId = playerObj.id;
        newPlayerController.playerData = playerObj;
        newPlayerController.setColor( playerObj.color );
    

        var multipleTarget = cam.GetComponent<MultipleTargetCamera>();
        if( multipleTarget != null) { multipleTarget.targets.Add(player.transform); }

    }
    /*
    WebSocket ws;
    Camera camera;

    public GameObject playerPrefab;
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>(); 
    List<playerController> players = new List<playerController>();
    
    bool EmitConnection = false;

    // Start is called before the first frame update
    void Start()
    {

        camera = (Camera) GameObject.FindObjectOfType(typeof(Camera));

        Application.runInBackground = true;
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) => {
            //Debug.Log("Message received, Data : " + e.Data);

            string[] sMsg = e.Data.Split(":");
            if( sMsg[0] == "newPlayer"){
                string[] data = sMsg[1].Split(',');
                _actions.Enqueue(() => NewPlayer(data[0], data[1]));
            } else if( sMsg[0] == "Move"){
                _actions.Enqueue(() => PlayerMove(sMsg[1]));
            } else if( sMsg[0] == "Delete"){
                _actions.Enqueue(() => deletePlayer(sMsg[1]));
            }

            
        };
        ws.Connect();
        
    }

    private void NewPlayer( string id, string color ) 
    {
        var player = Instantiate(playerPrefab) as GameObject;
        var newPlayerController = player.GetComponent<playerController>();
        
        newPlayerController.setId( id );
        newPlayerController.setColor( color );
        players.Add(newPlayerController);

        var multipleTarget = camera.GetComponent<MultipleTargetCamera>();
        multipleTarget.targets.Add(player.transform);
    }

    private void PlayerMove( string data ){
        string[] sData = data.Split(",");

        players.ForEach( delegate( playerController p) {
            if(sData[0] == p.gameId){
                p.playerMove( sData );
            }
        });

    }

    private void deletePlayer( string id ){

        players.ForEach( delegate( playerController p) {
            if(id == p.gameId){
                p.deletePlayer();
                var multipleTarget = camera.GetComponent<MultipleTargetCamera>();
                multipleTarget.targets.Remove(p.gameObject.transform);
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
        
    }*/
}
