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

    public GameObject MinigameUI, MinigameEnd;
    public GameObject RedKart, BlueKart, GreenKart, YellowKart;

    public float MinigameTime;

    Camera cam;
    string currentScene;
    GameObject CurrentMinigameUI;

    string[] defaultModels = { "mujica", "Nigga", "Pucho", "Tatita" };

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
    void endMinigame()
    {
        Instantiate(MinigameEnd);
    }

    public void InstantiateKart(gamePlayer playerObj, Vector3 pos, Quaternion newRotation, float i)
    {
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
        }
    }
}
