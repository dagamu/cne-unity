using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuSpawner : MonoBehaviour
{
  
    public GameObject cursorPrefab;
    public Object FirstScene;

    public float loadingTime;

    private void NewPlayer( string[] args ) 
    {
        var playerCursor = Instantiate(cursorPrefab) as GameObject;
        var cursorController = playerCursor.GetComponent<cursorControl>();
        playerCursor.transform.SetParent(gameObject.transform);
        playerCursor.transform.position = 
                gameObject.transform.position + new Vector3(-420+150*transform.childCount,0,0);

        cursorController.id = args[0];

    }
    void nextScene() { SceneManager.LoadScene(FirstScene.name); }
    public void PlayGame()
    {
        Invoke("nextScene", loadingTime);
        
     }
}
