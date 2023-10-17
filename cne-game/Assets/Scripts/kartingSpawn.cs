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

    public GameObject kartingPrefab, MinigameUI, MinigameEnd;
    public KartingCamera kartingCamera;

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
            InstantiateKart(player, newPosition, newRotation);
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
    bool isEnd = false;
    void Update()
    {

       
        /*
        if (!isEnd)
        {
            timer -= Time.deltaTime;
            string timerStr = Mathf.Round(timer).ToString();
            CurrentMinigameUI.transform.Find("Timer").GetComponent<TMP_Text>().SetText(Mathf.Round(timer).ToString());
            if (timerStr == "0")
            {
                endMinigame();
                isEnd = true;
            }
            else if (timer > MinigameTime)
            {
                CurrentMinigameUI.transform.Find("Timer").GetComponent<TMP_Text>().SetText(" ");
            }
        }*/
    }

    void endMinigame()
    {
        Instantiate(MinigameEnd);
        
    }

    public void InstantiateKart(gamePlayer playerObj, Vector3 pos, Quaternion newRotation)
    {
        GameObject newKarting = Instantiate( kartingPrefab, pos, newRotation, transform );
        newKarting.GetComponent<CarController>().playerData = playerObj;

        kartingCamera.target = newKarting.transform;
    }
}
