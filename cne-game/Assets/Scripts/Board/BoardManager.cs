using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gamePlayerSpace;

public class BoardManager : MonoBehaviour
{

    public GameObject diceObj, rollText, UIPlayerBox;
    public Transform playerBoxContainer;
    public bool rolling, chosingPath, movingBoard, waitingTurn, BoardUIsetted = false;

  

    float timer = 0;

    void Update(){ timer += Time.deltaTime; }

    void setBoardUIBoxes(){
        BoardUIsetted = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject newPlayerBox = Instantiate( UIPlayerBox );

            newPlayerBox.transform.parent = playerBoxContainer;
            newPlayerBox.GetComponent<RectTransform>().localPosition = new Vector3(-520, 90 - 90 * i, 0);
            var playerModelName = transform.GetChild(i)
                                        .GetComponent<playerController>()
                                        .playerData
                                        .model;
                                        
            Sprite newSprite = Resources.Load<Sprite>("Thumbnails/"+playerModelName);
            newPlayerBox.transform.GetChild(0).GetComponent<Image>().sprite = newSprite;

            getController(transform.GetChild(i).gameObject).UIBoardBox = newPlayerBox;
        }
    }

    public void managePlayerOnBoard()
    {
        var player = gameObject;
        Vector3 playerPos = player.transform.position;

        if ( getData(player).turn == 0 && !rolling && timer > 2 ) {
             startRoll( playerPos );
             if( (int) playerBoxContainer.transform.childCount == 0 ){ 
                transform.parent.GetComponent<BoardManager>().setBoardUIBoxes();
            }
        }
        else if( waitingTurn && getData(player).turn > 0 ){
            Debug.Log("Your turn");
        }
        //if ( chosingPath ) { managePathSelection(); }

    }

    gamePlayer getData( GameObject player ){
        return player.GetComponent<playerController>()
                                    .playerData;
    }
    playerController getController( GameObject player ){
        return player.GetComponent<playerController>();
    }

    void startRoll( Vector3 pos )
    {
        rolling = true;

         
        var newDice = Instantiate(diceObj);
        newDice.transform.position = new Vector3( pos.x, 3, pos.z);

        Rigidbody diceRb = newDice.GetComponent<Rigidbody>();
        diceRb.angularVelocity = new Vector3(
                                UnityEngine.Random.Range(-200, 200),
                                UnityEngine.Random.Range(-200, 200),
                                UnityEngine.Random.Range(-200, 200));

        GetComponent<playerController>().newDice = newDice;
    }

    public void rollingTrigger( GameObject player, gamePlayer playerData )
    {

        rolling = false;

        int rollNum = (int) Mathf.Floor(UnityEngine.Random.Range(1, 6));
        //boardStepsLeft = rollNum;
        GameObject rollTag = Instantiate(rollText);

        rollTag.transform.position = transform.position + Vector3.up * 3;
        rollTag.transform.parent = transform;
        rollTag.GetComponent<TextMesh>().text = rollNum.ToString();

        Destroy(player.GetComponent<playerController>().newDice);
        Destroy(rollTag, 2f);

        if( playerData.turn == 0 ){
            playerData.turnRoll = rollNum;
            transform.parent.GetComponent<BoardManager>().updateTurnRoll();
            waitingTurn = true;
        }
        //currentBoardPoint.GetComponent<BoardPointManager>().showLine();
        //chosingPath = true;
    }

int playersWithTurn = 0;
public void updateTurnRoll(){

    playersWithTurn++;

    if( playersWithTurn == transform.childCount ){

        List<GameObject> Children = new List<GameObject>(); 
        
        foreach (Transform child in transform)
        {
            Children.Add(child.gameObject);
        }

        for (int min = 0; min < transform.childCount; min++)
        {
           for (int j = min+1; j < transform.childCount; j++)
            {
                var minValue = Children[min]
                                    .GetComponent<playerController>()
                                    .playerData
                                    .turnRoll;
                var eValue = Children[j]
                                   .GetComponent<playerController>()
                                    .playerData
                                    .turnRoll;
                if( minValue < eValue ){
                    var aux = Children[min];
                    Children[min] = Children[j];
                    Children[j] = aux;
                }
            } 
        }

        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].GetComponent<playerController>()
                            .playerData
                            .updateTurn(i+1);
            Children[i].GetComponent<playerController>()
                            .UIBoardBox
                            .GetComponent<UIPlayerBoxController>()
                            .yPositionTarget = 90 - 90 * i;
        }
    }

}
   
}
