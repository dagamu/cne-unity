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
    Transform thumbnailsParent;


    GameObject GamepadConnect;
    GamepadConnect gamepadConnectComponent;
    gamePlayer playerObject;

    private GameObject characterSelect;

    Vector3 velocity;
    Rigidbody2D rb;
    Transform targetSelectable;

    void Start()
    {
        GamepadConnect = GameObject.Find("GamepadConnect");
        gamepadConnectComponent = GamepadConnect.GetComponent<GamepadConnect> ();

        rb = GetComponent<Rigidbody2D>();

        //thumbnailsParent = GameObject.Find("/Canvas/selectPlayer/Character Layout").transform;

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

    public float raycastOffset;
    void Update()
    {
        var players = gamepadConnectComponent.players;
       
        velocity = new Vector2( 
            playerObject.gamepadData[0],
            playerObject.gamepadData[1]
        );
      

        //gameObject.transform.position += velocity;

        var xpos = Mathf.Clamp(transform.position.x, 0, Screen.width);
        var ypos = Mathf.Clamp(transform.position.y, 0, Screen.height);

        var newPos = new Vector3( xpos, ypos, 0f);

        if( targetSelectable )
            transform.position = Vector3.Lerp(newPos, targetSelectable.position + new Vector3(0,-25,0), cursorSpeed);

        RaycastHit2D hit = Physics2D.Raycast( transform.position + velocity * raycastOffset, velocity );
        if ( hit.collider != null && hit.transform.CompareTag("UISelectable") && velocity.magnitude > 0.5f )
        {
            Debug.DrawRay(transform.position + velocity * raycastOffset, velocity * hit.distance, Color.yellow);
            targetSelectable = hit.transform;
        } else {
            Debug.DrawRay(transform.position + velocity * raycastOffset, velocity * 1000, Color.yellow);
            rb.velocity = Vector3.Lerp( rb.velocity, velocity * cursorSpeed * 3000f, 0.1f );
            if( velocity.magnitude > 0.5f ) targetSelectable = null;
        }

    }

    void OnTriggerEnter2D (Collider2D  col) 
    {
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
