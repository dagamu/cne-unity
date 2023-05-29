using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuSpawner : MonoBehaviour
{
  
    public GameObject cursorPrefab;

    private void NewPlayer( string[] args ) 
    {
        var playerCursor = Instantiate(cursorPrefab) as GameObject;
        var cursorController = playerCursor.GetComponent<cursorControl>();
        playerCursor.transform.parent = gameObject.transform;
        playerCursor.transform.position = gameObject.transform.position;

        cursorController.id = args[0];

    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
     }
}
