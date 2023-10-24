using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using gamePlayerSpace;
using TMPro;

public class spawnPlayers : MonoBehaviour
{

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    public GameObject playerPrefab, MinigameUI, MinigameEnd;
    public GameObject firstBoardPoint;

    public float MinigameTime;

    Camera cam;
    string currentScene;
    bool isMinigame = false;
    GameObject CurrentMinigameUI;

    string[] defaultModels = { "mujica", "Nigga", "Pucho", "Tatita" };

    void getPlayers(){
        var i = 0;
        foreach( gamePlayer player in gamepadConnectComponent.players ){
            if( player.model == null ) player.model = defaultModels[i];
            Vector3 newPosition = GameObject.Find("SpawnPoints").transform.GetChild(i).transform.position;
            InstantiatePlayer( player, newPosition );
            i++;
        } 
        if(currentScene != "Board"){
            isMinigame = true;
            CurrentMinigameUI = Instantiate(MinigameUI);
            CurrentMinigameUI.GetComponent<MinigameUIManager>().setUI( gameObject );
            timer = MinigameTime + 3;
        } 
    }

    void Start()
    {
        cam = (Camera) GameObject.FindObjectOfType(typeof(Camera));
        currentScene = SceneManager.GetActiveScene().name;

        GamepadConnect = GameObject.Find("GamepadConnect");
        if( GamepadConnect != null ){ 
            gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();
            Invoke( "getPlayers", gamepadConnectComponent.players.Count > 0 ? 0 : 1f);
        }

    }

    float timer;
    bool isEnd = false;
    void Update(){
        if( isMinigame && !isEnd ){
            timer -= Time.deltaTime;
            string timerStr = Mathf.Round(timer).ToString();
            CurrentMinigameUI.transform.Find("Timer").GetComponent<TMP_Text>().SetText(  Mathf.Round(timer).ToString() );
            if( timerStr == "0"){
                endMinigame();
                isEnd = true;
            }else if(timer > MinigameTime)
            {
                CurrentMinigameUI.transform.Find("Timer").GetComponent<TMP_Text>().SetText(" ");
            }
        }
    }

    void endMinigame(){
        Instantiate(MinigameEnd);
        for( int i = 0; i < transform.childCount; i++){
            var character = transform.GetChild(i);
            
            character.GetComponent<Rigidbody>().useGravity = false;
            character.GetComponent<Rigidbody>().isKinematic = true;
            character.GetComponent<playerController>().enabled = false;
            character.transform.GetChild(0).GetComponent<Animator>().SetBool("Running", false);

            character.transform.position = new Vector3( -3 + 6*(i+1)/(transform.childCount+1), MinigameEnd.transform.position.y, 2f);
            character.transform.rotation = Quaternion.Euler(0,180,0);

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
        newPlayerController.playerModel = playerModel;

        switch(  currentScene ){ 
            case "Board": 
                addBoardManager(player);
                break;
            case "Basketball":
                addBasketballController(player);
                break;
            case "CubeTower":
                manageCubeTowerSpawn(player);
                break;
        }

        if( firstBoardPoint != null ){
            player.GetComponent<BoardManager>().currentBoardPoint = firstBoardPoint;    
        } 

        var multipleTarget = cam.GetComponent<MultipleTargetCamera>();
        if( multipleTarget != null) { multipleTarget.targets.Add(player.transform); }

    }

    void manageCubeTowerSpawn( GameObject player ){
        var CubeParents = GameObject.Find("cubeSpawn");
        var cubeToPlayer = CubeParents.transform.GetChild( player.transform.GetSiblingIndex() );
        cubeToPlayer.gameObject.SetActive(true);
        cubeToPlayer.GetComponent<CubeTowerSpawn>().setLinkedPlayer(player);
    }

    void addBoardManager( GameObject player )
    {
        BoardManager BoardManager = GetComponent<BoardManager>();
        var bm = player.AddComponent<BoardManager>();

        bm.diceObj = BoardManager.diceObj;
        bm.rollText = BoardManager.rollText;
        bm.playerBoxContainer = BoardManager.playerBoxContainer;

        if( Utility.getData(player).currentBoardPoint != new Vector3(0,-1,0) ){
            player.transform.position = Utility.getData(player).currentBoardPoint - Vector3.forward;
        } else {
            Utility.getData(player).updateCurrentBoardPos(firstBoardPoint.transform.position);
        }
    }

    void addBasketballController( GameObject player ){
        BasketballController BasketballController = GetComponent<BasketballController>();
        var bkM = player.AddComponent<BasketballController>();
        bkM.setPoints();
    }
}