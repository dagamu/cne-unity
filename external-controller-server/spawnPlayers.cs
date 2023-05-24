using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;

public class spawnPlayers : MonoBehaviour
{

    WebSocket ws;

    public GameObject Squareprefab;
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>(); 
    List<playerController> players = new List<playerController>();
    
    bool EmitConnection = false;

    // Start is called before the first frame update
    void Start()
    {
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
        var player = Instantiate(Squareprefab) as GameObject;
        var newPlayerController = player.GetComponent<playerController>();
        
        newPlayerController.setId( id );
        newPlayerController.setColor( color );
        players.Add(newPlayerController);
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
