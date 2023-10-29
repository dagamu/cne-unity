using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using gamePlayerSpace;
using TMPro;

public class kartingSpawn : MonoBehaviour
{
    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    public GameObject MinigameUI, Podium;
    public GameObject RedKart, BlueKart, GreenKart, YellowKart;

    public float MinigameTime;
    public Vector3 PodiumPos;

    Camera cam;
    string currentScene;
    GameObject CurrentMinigameUI;

    string[] defaultModels = { "Claudio", "Mujica", "Rodrigo", "Andrea" };

    void getPlayers()
    {
        var i = 0;
        foreach (gamePlayer player in gamepadConnectComponent.players)
        {
            if (player.model == null) player.model = defaultModels[i];
            var spawnPoint = GameObject.Find("SpawnPoints").transform.GetChild(i);
            Vector3 newPosition = spawnPoint.transform.position;
            Quaternion newRotation = spawnPoint.transform.rotation;
            InstantiateKart(player, newPosition, newRotation, i);
            i++;
        }
    }
    void Start()
    {
        cam = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        currentScene = SceneManager.GetActiveScene().name;

        GamepadConnect = GameObject.Find("GamepadConnect");
        if (GamepadConnect != null)
        {
            gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect>();
            Invoke("getPlayers", gamepadConnectComponent.players.Count > 0 ? 0 : 1f);
        }

    }

    float timer;
    bool podiumSetted = false;
    void Update()
    {
        if( podiumSetted && Input.anyKey ) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if( Input.GetKeyDown(KeyCode.Return) ) SetPodium();
    }

    public void SetPodium(){
        var pod = Instantiate( Podium, PodiumPos, Quaternion.identity, transform.parent );
        foreach( Transform players in transform ){

            players.Find("CarCamera").gameObject.SetActive(false);
            var cam = GameObject.Find("Main Camera").GetComponent<KartCameraAnim>();
            cam.MovPoints = pod.transform.Find("Camera Mov").transform;
            cam.currentIndex = 0;

            MinigameUI.SetActive(false);

            var charModel = players.Find("Character Model");
            var racePosition = players.GetComponent<RaceCarController>().position;
            var animation = racePosition == 1 ? "Victory" : racePosition == 4 ? "Sad" : "Clapping";

            charModel.GetComponent<Animator>().SetBool( "Driving", false );
            charModel.GetComponent<Animator>().SetBool( animation, true );

            charModel.transform.parent = pod.transform;
            charModel.transform.rotation = Quaternion.Euler(0, 90, 0);
            charModel.transform.position = pod.transform.GetChild(racePosition-1).GetChild(0).position;
        }
        podiumSetted = true;
    }

    public void InstantiateKart(gamePlayer playerObj, Vector3 pos, Quaternion newRotation, float i)
    {
        GameObject characterPrefab = Resources.Load<GameObject>("Characters/"+playerObj.model);

        var color = playerObj.color.Split(';');
        GameObject kartingPrefab;

        if (color[0] == "1")
        {
            if (color[1] == "1") kartingPrefab = YellowKart;
            else kartingPrefab = RedKart;
        }
        else if(color[1] == "1") kartingPrefab = GreenKart;
        else kartingPrefab = BlueKart;

        GameObject newKarting = Instantiate( kartingPrefab, pos, newRotation, transform );

        var modelPos = newKarting.transform.Find("ModelSpawn").transform.position;
        var modelRot = newKarting.transform.Find("ModelSpawn").transform.rotation;
        var model = Instantiate( characterPrefab, modelPos, modelRot, newKarting.transform );

        model.GetComponent<Animator>().SetBool("Driving", true);
        model.name = "Character Model";

        newKarting.GetComponent<RaceCarController>().CharacterModel = model;
        

        newKarting.GetComponent<CarController>().playerData = playerObj;
        var kartingCamera = newKarting.transform.Find("CarCamera");
        if( gamepadConnectComponent.players.Count != 1 ){
            kartingCamera.GetComponent<Camera>().rect = new Rect( i == 1 || i == 2 ? 0.5f: 0, i <= 1 ? 0.5f : 0, 0.5f, 0.5f);
        } else {
            kartingCamera.GetComponent<Camera>().rect = new Rect( 0, 0, 1, 1);
        }
            kartingCamera.GetComponent<KartingCamera>().target = newKarting.transform;
        
    }

    public void SetCameras(){
        foreach(Transform p in transform ){
            p.transform.Find("CarCamera").gameObject.SetActive(true);
            p.GetComponent<CarController>().kartSpeed = 1;
        }
        MinigameUI.SetActive(true);
    }
}
