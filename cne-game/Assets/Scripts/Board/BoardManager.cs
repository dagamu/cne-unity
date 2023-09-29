using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gamePlayerSpace;

public class BoardManager : MonoBehaviour
{

    public GameObject diceObj, rollText, UIPlayerBox;

    [HideInInspector]
    public GameObject targetPoint, currentBoardPoint;
    public Transform playerBoxContainer;
    public bool rolling, chosingPath, movingBoard,
     waitingTurn, BoardUIsetted, onTurn = false;

     Vector3 autoMove;

    float timer = 0;
    int playersWithTurn = 0;

    public int currentTurn = 1;

    public void managePlayerOnBoard()
    {
        var player = gameObject;
        Vector3 playerPos = player.transform.position;

        if ( getData(player).turn + getData(player).turnRoll == 0 && !rolling && timer > 2 ) {
             startRoll( playerPos );
             if( (int) playerBoxContainer.transform.childCount == 0 ){ 
                transform.parent.GetComponent<BoardManager>().setBoardUIBoxes();
            }
        }

        else if( waitingTurn && getData(player).turn > 0 && !onTurn 
            && transform.parent.GetComponent<BoardManager>().currentTurn == getData(player).turn ){

            onTurn = true;
            waitingTurn = false;
            startRoll( playerPos );
        } else if( chosingPath ) { managePathSelection(); }
        else if( movingBoard ){ manageBoardMove(); }

    }

    void setBoardUIBoxes(){
        BoardUIsetted = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject newPlayerBox = Instantiate( UIPlayerBox );

            newPlayerBox.transform.parent = playerBoxContainer;
            newPlayerBox.GetComponent<RectTransform>().localPosition = new Vector3(-520, 90 - 90 * i, 0);
            var playerModelName = getData( transform.GetChild(i).gameObject ).model;
                                        
            Sprite newSprite = Resources.Load<Sprite>("Thumbnails/"+playerModelName);
            newPlayerBox.transform.GetChild(0).GetComponent<Image>().sprite = newSprite;

            getController(transform.GetChild(i).gameObject).UIBoardBox = newPlayerBox;
        }
    }

    Vector2 nearPoint = new Vector2(0, 0); // X: next Point Index on hover, Y: Angle 

    void getHoverPathPoint( BoardPointManager bP ){

        var nextPoints = bP.nextPoints;

        for (int i = 0; i < nextPoints.Count; i++)

            {
                var playerData = getData(gameObject);
                Vector2 mouseDir = new Vector2(playerData.gamepadData[0], playerData.gamepadData[1]);
                Vector2 nPointDir = new Vector2(
                    nextPoints[i].transform.position.x - transform.position.x,
                    nextPoints[i].transform.position.y - transform.position.y
                    );

                if( Vector2.Angle(mouseDir, nPointDir) > nearPoint.y )
                {
                    nearPoint.x = i;
                    nearPoint.y = Vector2.Angle(mouseDir, nPointDir);
                }

                bP.pathLines[(int)i]
                    .GetComponent<LineRenderer>().material.color = Color.white;
            }

            bP.pathLines[(int)nearPoint.x]
                .GetComponent<LineRenderer>().material.color = getController(gameObject).playerColor;

    }


    void managePathSelection()
    {

        var boardPoint = currentBoardPoint.GetComponent<BoardPointManager>();

        if ( boardPoint.nextPoints.Count > 0 && nearPoint.x < boardPoint.nextPoints.Count ){
            targetPoint = boardPoint.nextPoints[ (int) nearPoint.x ];
        }

        if (boardPoint.nextPoints.Count > 1) { 
            getHoverPathPoint( boardPoint.GetComponent<BoardPointManager>() );
        }

        else if ( boardPoint.nextPoints.Count == 1 ){

            if ( boardPoint.pathLines.Count > 0 )
            {
                boardPoint.pathLines[0]
                    .GetComponent<LineRenderer>().material.color = getController(gameObject).playerColor;
            }

            var nextPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];

            if (movingBoard)
            {
                targetPoint = nextPoint;
            }
        }

        if ( getData(gameObject).gamepadData[2] == 1 ){

            chosingPath = false;
            movingBoard = true;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            targetPoint = boardPoint.nextPoints[ (int)nearPoint.x ];
            targetPoint.GetComponent<BoardPointManager>().showLine();

        }

    }

    void manageBoardMove(){

        Vector3 dis = targetPoint.transform.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(targetPoint.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, timer);
        if(Quaternion.Angle(transform.rotation, lookRotation) == 0) { timer = 0; }

        if (dis.magnitude < 0.5f){

            Debug.Log("Arrive");

            currentBoardPoint = targetPoint;

            if ( currentBoardPoint.GetComponent<BoardPointManager>().nextPoints.Count > 1 ){

                movingBoard = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.position = currentBoardPoint.transform.position;
                getController(gameObject).playerModel
                    .GetComponent<Animator>().SetBool("Running", false);
                targetPoint = null;
            } else
            {
                targetPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];
            }
        }

        var normVel = Vector3.Normalize(dis);
        autoMove = new Vector3(normVel.x, 0, normVel.z);
        GetComponent<Rigidbody>().velocity =
                             autoMove * getController(gameObject).speed * 0.7f * Time.deltaTime;
        getController(gameObject).playerModel
            .GetComponent<Animator>().SetBool("Running", autoMove.magnitude > 0.1);

    }

    void startRoll( Vector3 pos ){
            
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
        } else if( onTurn ){
            currentBoardPoint.GetComponent<BoardPointManager>().showLine();
            chosingPath = true;
        }
    }


    void updateTurnRoll(){

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
                        var minValue = getData(Children[min]).turnRoll;
                        var eValue = getData(Children[j]).turnRoll;

                        if( minValue < eValue ){
                            var aux = Children[min];
                            Children[min] = Children[j];
                            Children[j] = aux;
                        }
                    } 
            }

            for (int i = 0; i < Children.Count; i++)
            {
                getData(Children[i]).updateTurn(i+1);
                getController(Children[i])
                                .UIBoardBox
                                .GetComponent<UIPlayerBoxController>()
                                .yPositionTarget = 90 - 90 * i;
                getData(Children[i]).turnRoll = 0;
            }
        }

    }

    

    void Update(){ timer += Time.deltaTime; }

    gamePlayer getData( GameObject player ){
        return player.GetComponent<playerController>()
                                    .playerData;
    }
    playerController getController( GameObject player ){
        return player.GetComponent<playerController>();
    }
   
}
