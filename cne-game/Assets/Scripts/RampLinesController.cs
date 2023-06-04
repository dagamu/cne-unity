using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using gamePlayerSpace;

public class RampLinesController : MonoBehaviour
{

    public List<GameObject> ScoreLines;

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    // Start is called before the first frame update
    void Start()
    {
        GamepadConnect = GameObject.Find("GamepadConnect");
        gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();

        for (int i = 0; i < gamepadConnectComponent.players.Count; i++)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
