using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackGroundMusic : MonoBehaviour
{
    public AudioClip MainTheme, Map1, Map2, Map3, Victory;
    string sceneName;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        
    }

    void Start(){
        sceneName =  SceneManager.GetActiveScene().name;
        switch( sceneName ){
            case "Main menu":
                GetComponent<AudioSource>().clip = MainTheme;
                break;
            case "Karting Map I":
                GetComponent<AudioSource>().clip = Map1;
                break;
            case "Karting Map II":
                GetComponent<AudioSource>().clip = Map2;
                break;
            case "Karting Map III":
                GetComponent<AudioSource>().clip = Map3;
                break;
        }
    }

}
