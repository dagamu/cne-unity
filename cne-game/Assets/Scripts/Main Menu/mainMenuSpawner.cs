using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuSpawner : MonoBehaviour
{
  
    public GameObject cursorPrefab;
    public Object FirstScene;

    private void NewPlayer( string[] args ) 
    {
        var playerCursor = Instantiate(cursorPrefab) as GameObject;
        var cursorController = playerCursor.GetComponent<cursorControl>();
        playerCursor.transform.SetParent(gameObject.transform);
        playerCursor.transform.position = 
                gameObject.transform.position + new Vector3(-470+150*transform.childCount,0,0);

        cursorController.id = args[0];

    }

    public void PlayGame()
    {
        SceneManager.LoadScene(FirstScene.name);
     }
}
