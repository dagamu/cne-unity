using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gamePlayerSpace;

public class cursorControl : MonoBehaviour
{

    public string id;
    public float cursorSpeed;
    public Color playerColor;
    public GameObject characterSelect;

    public Camera Camera;

    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;
    gamePlayer playerObject;

    Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        GamepadConnect = GameObject.Find("GamepadConnect");
        gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();

        var players = gamepadConnectComponent.players;

        foreach (var p in players ){
            if( id == p.id ){

                playerObject = p;

                string[] colorData = p.color.Split(";");
                playerColor = new Color(
                    float.Parse(colorData[0]),
                    float.Parse(colorData[1]),
                    float.Parse(colorData[2]),
                    1f
                );
                gameObject.GetComponent<Image>().color = playerColor;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
         
        var players = gamepadConnectComponent.players;

       
        velocity = new Vector3( 
            playerObject.gamepadData[0] * cursorSpeed,
            playerObject.gamepadData[1] * cursorSpeed,
            0
        );
        gameObject.transform.position += velocity;

        var xpos = Mathf.Clamp(transform.position.x, 0, Screen.width);
        var ypos = Mathf.Clamp(transform.position.y, 0, Screen.height);

        transform.position = new Vector3( xpos, ypos, 0f);



    }

    void OnTriggerEnter2D (Collider2D  col) {

        Color collisionHighlight = col.gameObject.GetComponent<Image>().color;

        if( col.gameObject.GetComponent<cursorControl>() != null || collisionHighlight != Color.white ){
            return;
        }
 
        if( characterSelect != null ){
            characterSelect.GetComponent<Image>().color = Color.white;
        }

        characterSelect = col.gameObject; 
        characterSelect.GetComponent<Image>().color = playerColor;
        playerObject.model = col.gameObject.transform.GetChild(0).GetComponent<Image>().sprite.name;
         
     }
}
