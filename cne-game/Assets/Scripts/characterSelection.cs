using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelection : MonoBehaviour
{

    public List<Sprite> charactersThumbnails;
    public GameObject characterThumbnail;
    // Start is called before the first frame update
    void Start()
    {
        foreach( var sprite in charactersThumbnails ){
            GameObject newCharacter = Instantiate(characterThumbnail);
            newCharacter.transform.parent = gameObject.transform;
            Image imageComponent = newCharacter.transform.GetChild(0).GetComponent<Image>();
            imageComponent.sprite = sprite;

        }      
    }

}
