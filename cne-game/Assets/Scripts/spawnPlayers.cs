using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using gamePlayerSpace;

public class spawnPlayers : MonoBehaviour
{

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    public GameObject playerPrefab;
    public GameObject firstBoardPoint;

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

        GameObject player = Instantiate( playerPrefab, pos, Quaternion.identity, transform );
        GameObject playerModel = Instantiate( characterPrefab, pos, Quaternion.identity, player.transform );
        var newPlayerController = player.GetComponent<playerController>();

        newPlayerController.gameId = playerObj.id;
        newPlayerController.playerData = playerObj;
        newPlayerController.setColor( playerObj.color );

        BoardManager BoardManager = GetComponent<BoardManager>();
        if( SceneManager.GetActiveScene().name == "Board" ){ 

            var bm = player.AddComponent<BoardManager>();
            bm.diceObj = BoardManager.diceObj;
            bm.rollText = BoardManager.rollText;
            bm.playerBoxContainer = BoardManager.playerBoxContainer;

        }

        newPlayerController.currentBoardPoint = firstBoardPoint;    

        var multipleTarget = cam.GetComponent<MultipleTargetCamera>();
        if( multipleTarget != null) { multipleTarget.targets.Add(player.transform); }

    }
}
