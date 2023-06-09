using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;

    // Start is called before the first frame update
    void Start()
    {
        GamepadConnect = GameObject.Find("GamepadConnect");
        gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();

        
    }

    // Update is called once per frame
    void Update()
    {
        var players = gamepadConnectComponent.players;

        var playerSelected = 0;
        foreach (var p in players ){
            if( p.model != null ){ 
                playerSelected++;
            }
        }
        if(playerSelected == 4){
            gameObject.GetComponent<Button>().onClick.Invoke(); 
        }


    }
}
