using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationCanvas : MonoBehaviour
{
    public Transform selectedCharacter;
    public Transform modelsParent; 

    void Start(){
        selectedCharacter = transform.Find("Icons").GetChild(0);
    }

    public void setCharacter( string model ){
        foreach(Transform icon in transform.Find("Icons") ){
            icon.GetChild(0).GetComponent<Image>().color = icon.gameObject.name == model ? Color.green : Color.white;
            selectedCharacter = icon.gameObject.name == model ? icon : selectedCharacter; 
        }
    }

    string[] animations = {"Driving", "Sad", "Victory"};
    public void setAnimation( string anim ){
        var animator = modelsParent.Find(selectedCharacter.gameObject.name).gameObject.GetComponent<Animator>();
        if(anim == "Honking"){
            animator.SetBool( "Driving", true );
            animator.SetTrigger( "Honking" );
            anim = "Driving";
        }

        foreach( var a in animations ){
            animator.SetBool( a, a == anim );
        }
    }
}
