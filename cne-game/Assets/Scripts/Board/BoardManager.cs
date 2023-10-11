using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using gamePlayerSpace;

public class BoardManager : MonoBehaviour
{

    public GameObject diceObj, rollText, UIPlayerBox;
    public bool DevMode;

    [HideInInspector]
    public GameObject targetPoint, currentBoardPoint;
    public Transform playerBoxContainer;
    public bool rolling;
    bool chosingPath, movingBoard, onTurn = false;
    bool waitingTurn = true;

    public List<string> Minigames = new List<string>();

     Vector3 autoMove;

    float timer = 0;
    int playersWithTurn = 0;

    public float boardSpeed = 100;
    int boardStepsLeft = 0;

    public void managePlayerOnBoard()
    {
        if( timer > 1.5f ) transform.parent.GetComponent<BoardManager>().setBoardUIBoxes();

        var player = gameObject;
        Vector3 playerPos = player.transform.position;

        Debug.Log(string.Format("{0}; {1}; {2}; {3}; {4}", gameObject.name, waitingTurn, Utility.getData(player).turn, GameObject.Find("GamepadConnect").GetComponent<GamepadConnect>().currentTurn, onTurn ));

        if ( Utility.getData(player).turn + Utility.getData(player).turnRoll == 0 && !rolling && timer > 1.5f ) {
             startRoll( playerPos );
        }
        else if( waitingTurn && Utility.getData(player).turn > 0 && !onTurn 
            && GameObject.Find("GamepadConnect").GetComponent<GamepadConnect>().currentTurn == Utility.getData(player).turn )
        {
            onTurn = true;
            setCamera();
            waitingTurn = false;
            startRoll( playerPos );
        } 
        else if( chosingPath ) managePathSelection();
        else if( movingBoard ) manageBoardMove(); 

    }

    void nextTurn(){
        var currentTurn = GameObject.Find("GamepadConnect").GetComponent<GamepadConnect>().currentTurn;
        GameObject.Find("GamepadConnect").GetComponent<GamepadConnect>().currentTurn = currentTurn == playerBoxContainer.transform.childCount ? 1 : currentTurn + 1;

        onTurn = false;
        waitingTurn = true;
        chosingPath = false;
        movingBoard = false;
    }

    void setCamera(){
        GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>().targets = new List<Transform>() { transform };
    }

    void setBoardUIBoxes(){
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject newPlayerBox = Instantiate( UIPlayerBox );

            newPlayerBox.transform.SetParent(playerBoxContainer);
            newPlayerBox.GetComponent<RectTransform>().localPosition = new Vector3(-520, 90 - 90 * i, 0);
            var playerModelName = Utility.getData( transform.GetChild(i).gameObject ).model;
                                        
            Sprite newSprite = Resources.Load<Sprite>("Thumbnails/"+playerModelName);
            newPlayerBox.transform.GetChild(0).GetComponent<Image>().sprite = newSprite;

            Utility.getController(transform.GetChild(i).gameObject).UIBoardBox = newPlayerBox;
        }
    }

    void managePathSelection()
    {
        var boardPoint = currentBoardPoint.GetComponent<BoardPointManager>();
        var playerColor = Utility.getController(gameObject).playerColor;

        if (boardPoint.nextPoints.Count > 1) 
            targetPoint = currentBoardPoint.GetComponent<BoardPointManager>()
                            .getHoverPathPoint( transform.position, Utility.getData(gameObject), playerColor ); 

        else if ( boardPoint.nextPoints.Count == 1 )
        {
            targetPoint = boardPoint.nextPoints[0];
            if ( boardPoint.pathLines.Count > 0 )
                boardPoint.pathLines[0]
                    .GetComponent<LineRenderer>().material.color = Utility.getController(gameObject).playerColor;

            var nextPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];

            if (movingBoard)
                targetPoint = nextPoint;

        }

        if ( Utility.getData(gameObject).gamepadData[2] == 1 ){

            chosingPath = false;
            movingBoard = true;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            targetPoint = currentBoardPoint.GetComponent<BoardPointManager>()
                            .getHoverPathPoint( transform.position, Utility.getData(gameObject), playerColor );
            //targetPoint.GetComponent<BoardPointManager>().showLine();

        }

    }
    void manageBoardMove(){

        Vector3 dis = targetPoint.transform.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(targetPoint.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, timer);
        if( Quaternion.Angle(transform.rotation, lookRotation) == 0 ) timer = 0;

        if (dis.magnitude < 0.5f){

            currentBoardPoint.GetComponent<BoardPointManager>().hideLine();
            currentBoardPoint = targetPoint;
            Utility.getData(gameObject).updateCurrentBoardPos(currentBoardPoint.transform.position);

            boardStepsLeft--;
            if (boardStepsLeft == 0)
            {
                int newMinigame;
                if (DevMode)
                    newMinigame = 0;    
                else
                    newMinigame = (int) Mathf.Round( Random.Range(0, Minigames.Count - 1) );

                nextTurn();
                SceneManager.LoadScene(transform.parent.GetComponent<BoardManager>().Minigames[newMinigame]);
            }

            if ( currentBoardPoint.GetComponent<BoardPointManager>().nextPoints.Count > 1 )
            {
                movingBoard = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.position = currentBoardPoint.transform.position;
                Utility.getController(gameObject).playerModel
                    .GetComponent<Animator>().SetBool("Running", false);
                targetPoint = null;
                chosingPath = true;
            } else
                targetPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];
        }

        var normVel = Vector3.Normalize(dis);
        autoMove = new Vector3(normVel.x, 0, normVel.z);
        GetComponent<Rigidbody>().velocity =
                             autoMove * boardSpeed * 0.7f * Time.deltaTime;
        Utility.getController(gameObject).playerModel
            .GetComponent<Animator>().SetBool("Running", autoMove.magnitude > 0.1);

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
        GameObject rollTag = Instantiate(rollText);

        rollTag.transform.position = transform.position + Vector3.up * 3;
        rollTag.transform.parent = transform;
        rollTag.GetComponent<TextMesh>().text = rollNum.ToString();

        Destroy(player.GetComponent<playerController>().newDice);
        Destroy(rollTag, 1f);

        if( playerData.turn == 0 ){
            playerData.turnRoll = rollNum;
            transform.parent.GetComponent<BoardManager>().updateTurnRoll();
            waitingTurn = true;
        } else if( onTurn ){
            chosingPath = true;
            boardStepsLeft = rollNum;
        }
    }


    void updateTurnRoll(){

        playersWithTurn++;

        if( playersWithTurn == transform.childCount ){

            List<GameObject> Children = new List<GameObject>(); 
            
            foreach (Transform child in transform)
                Children.Add(child.gameObject);

            for (int min = 0; min < transform.childCount; min++)
            {
                for (int j = min+1; j < transform.childCount; j++)
                    {
                        var minValue = Utility.getData(Children[min]).turnRoll;
                        var eValue = Utility.getData(Children[j]).turnRoll;

                        if( minValue < eValue ){
                            var aux = Children[min];
                            Children[min] = Children[j];
                            Children[j] = aux;
                        }
                    } 
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Utility.getData(Children[i]).updateTurn(i+1);
                Utility.getController(Children[i])
                                .UIBoardBox
                                .GetComponent<UIPlayerBoxController>()
                                .yPositionTarget = 90 - 90 * i;
                Utility.getData(Children[i]).turnRoll = 0;
            }
        }
    }
    void Update() => timer += Time.deltaTime;
   
}
