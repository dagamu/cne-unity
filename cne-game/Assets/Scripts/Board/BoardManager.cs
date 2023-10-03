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
    bool chosingPath, movingBoard, waitingTurn, onTurn = false;

    public List<string> Minigames = new List<string>();

     Vector3 autoMove;

    float timer = 0;
    int playersWithTurn = 0;

    public int currentTurn = 1;
    public float boardSpeed = 100;
    int boardStepsLeft = 0;

    public void managePlayerOnBoard()
    {
        var player = gameObject;
        Vector3 playerPos = player.transform.position;

        if ( Utility.getData(player).turn + Utility.getData(player).turnRoll == 0 && !rolling && timer > 2 ) {
             startRoll( playerPos );
             if( (int) playerBoxContainer.transform.childCount == 0 ){ 
                transform.parent.GetComponent<BoardManager>().setBoardUIBoxes();
            }
        }

        else if( waitingTurn && Utility.getData(player).turn > 0 && !onTurn 
            && transform.parent.GetComponent<BoardManager>().currentTurn == Utility.getData(player).turn ){

            onTurn = true;
            setCamera();
            waitingTurn = false;
            startRoll( playerPos );
        } else if( chosingPath ) { managePathSelection(); }
        else if( movingBoard ){ manageBoardMove(); }

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

    Vector2 nearPoint = new Vector2(0, 0); // X: next Point Index on hover, Y: Angle 

    void getHoverPathPoint( BoardPointManager bP ){

        var nextPoints = bP.nextPoints;

        for (int i = 0; i < nextPoints.Count; i++)

            {
                var playerData = Utility.getData(gameObject);
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

                var currentTargetPoint = bP.nextPoints[i].GetComponent<BoardPointManager>();
                var nextTargetPoint = currentTargetPoint.nextPoints[0]
                                .GetComponent<BoardPointManager>();

                while( currentTargetPoint.nextPoints.Count == 1 && currentTargetPoint != bP ){
                    
                    
                    if( currentTargetPoint.pathLines.Count == 0 ){
                        currentTargetPoint.showLine();
                    } else {
                        currentTargetPoint.pathLines[0].GetComponent<LineRenderer>().material.color = Color.white;
                    }

                    var aux = currentTargetPoint;
                    currentTargetPoint = nextTargetPoint;
                    nextTargetPoint = aux.nextPoints[0]
                                .GetComponent<BoardPointManager>();
                    
                }

                if(bP.pathLines.Count == 0){ bP.showLine(); };

                bP.pathLines[ (int)i ]
                    .GetComponent<LineRenderer>().material.color = Color.white;
            }

            bP.pathLines[ (int) nearPoint.x ]
                .GetComponent<LineRenderer>().material.color = Utility.getController(gameObject).playerColor;

            var selectedTargetPoint = bP.nextPoints[ (int) nearPoint.x ].GetComponent<BoardPointManager>();
            while( selectedTargetPoint.pathLines.Count == 1 ){
                selectedTargetPoint.pathLines[ 0 ]
                .GetComponent<LineRenderer>().material.color = Utility.getController(gameObject).playerColor;
                selectedTargetPoint = selectedTargetPoint.nextPoints[0]
                                        .GetComponent<BoardPointManager>();
        }
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
                    .GetComponent<LineRenderer>().material.color = Utility.getController(gameObject).playerColor;
            }

            var nextPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];

            if (movingBoard)
            {
                targetPoint = nextPoint;
            }
        }

        if ( Utility.getData(gameObject).gamepadData[2] == 1 ){

            chosingPath = false;
            movingBoard = true;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            targetPoint = boardPoint.nextPoints[ (int)nearPoint.x ];
            //targetPoint.GetComponent<BoardPointManager>().showLine();

        }

    }

    void manageBoardMove(){

        Vector3 dis = targetPoint.transform.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(targetPoint.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, timer);
        if(Quaternion.Angle(transform.rotation, lookRotation) == 0) { timer = 0; }

        if (dis.magnitude < 0.5f){

            boardStepsLeft--;
            if (boardStepsLeft == 0)
            {
                int newMinigame;
                if (DevMode) {
                    newMinigame = 0;    
                }
                else
                {
                    newMinigame = (int) Mathf.Round( Random.Range(0, Minigames.Count - 1) );
                }

                SceneManager.LoadScene(transform.parent.GetComponent<BoardManager>().Minigames[newMinigame]);
            }

               currentBoardPoint.GetComponent<BoardPointManager>().hideLine();
            currentBoardPoint = targetPoint;

            if ( currentBoardPoint.GetComponent<BoardPointManager>().nextPoints.Count > 1 ){

                movingBoard = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.position = currentBoardPoint.transform.position;
                Utility.getController(gameObject).playerModel
                    .GetComponent<Animator>().SetBool("Running", false);
                targetPoint = null;
                chosingPath = true;
            } else
            {
                currentBoardPoint.GetComponent<BoardPointManager>().showLine();
                targetPoint = currentBoardPoint.GetComponent<BoardPointManager>().nextPoints[0];
            }
        }

        var normVel = Vector3.Normalize(dis);
        autoMove = new Vector3(normVel.x, 0, normVel.z);
        GetComponent<Rigidbody>().velocity =
                             autoMove * boardSpeed * 0.7f * Time.deltaTime;
        Utility.getController(gameObject).playerModel
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
            chosingPath = true;
            boardStepsLeft = rollNum;
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

    

    void Update(){ timer += Time.deltaTime; }
   
}
