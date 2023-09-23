using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonCursors : MonoBehaviour
{

    private Image ImageComponent;

    void Start()
    {
        ImageComponent = gameObject.GetComponent<Image>();
        ImageComponent.color = new Color(0,0,0,0);
    }

    void OnTriggerEnter2D( Collider2D col ){
        ImageComponent.color = new Color( 0,0,0,
                                    ImageComponent.color.a+0.1f);
        if( ImageComponent.color.a == 0.4f ){
            gameObject.GetComponent<Button>().onClick.Invoke(); 
        }
    }
    void OnTriggerExit2D( Collider2D col ){
        ImageComponent.color = new Color(0,0,0,
                                    ImageComponent.color.a-0.1f);
    }
}
