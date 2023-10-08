using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamePlayerSpace;

public class BoardPointManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> nextPoints = new List<GameObject>();
    public List<GameObject> pathLines = new List<GameObject>();
    public GameObject LineObject; 

    public bool showSelectLines = false;

    List<Vector3> selectPathLimits = new List<Vector3>(); // (X,Y,Z): ( First Angle, Final Angle, Target Point Index)

    GameObject cam;
    void Start(){
        cam = GameObject.Find("Main Camera");
        if( nextPoints.Count > 1 ) setSelectPathLimits();

        selectedTargetPoint = nextPoints[0];
    } 

    void Update(){
        if( showSelectLines ) showSelectLinesF();
    }

    void setSelectPathLimits(){
        for (int i = 0; i < nextPoints.Count; i++ ) 
        {
            GameObject previusPoint;
            if( i == 0 )
                previusPoint = nextPoints[nextPoints.Count-1];
             else 
                previusPoint =  nextPoints[i-1];
            

            GameObject nexPoint;
            if( i == nextPoints.Count - 1)
                nexPoint = nextPoints[0];
            else 
                nexPoint = nextPoints[i+1];

            float currentAngle = Vector2.SignedAngle( Vector2.right, Utility.ToVector2(nextPoints[i].transform.position - transform.position) );
            float previusAngle = Vector2.SignedAngle( Vector2.right, Utility.ToVector2(previusPoint.transform.position - transform.position) );
            float nextAngle = Vector2.SignedAngle( Vector2.right, Utility.ToVector2(nexPoint.transform.position - transform.position) );

            currentAngle = currentAngle < 0 ? 360 + currentAngle : currentAngle;
            previusAngle = previusAngle < 0 ? 360 + previusAngle : previusAngle;
            nextAngle = nextAngle < 0 ? 360 + nextAngle : nextAngle;

            float firstSelectAngle = previusAngle < currentAngle ?
                                        ( previusAngle + currentAngle ) / 2:
                                        ( previusAngle + 360 + currentAngle ) / 2;
            float finalSelectAngle = nextAngle > currentAngle ? 
                                        (currentAngle + nextAngle ) / 2:
                                        (currentAngle + nextAngle + 360) / 2;

            /*firstSelectAngle = firstSelectAngle > 180 ? firstSelectAngle - 360 : firstSelectAngle;
            finalSelectAngle = finalSelectAngle > 180 ? finalSelectAngle - 360 : finalSelectAngle;*/

            selectPathLimits.Add( new Vector3(firstSelectAngle, finalSelectAngle, i ));
        }
    }

    void showSelectLinesF(){
        foreach( var limit in selectPathLimits ){
            Vector3 result = new Vector3( Mathf.Cos(limit.x * Mathf.Deg2Rad), 0f, Mathf.Sin(limit.x*Mathf.Deg2Rad) ) * 2; 
            Debug.DrawLine( transform.position, transform.position + result);
        }
    }


    public void showLine()
    {
        for (int i = 0; i < nextPoints.Count; i++ ) 
        {
            GameObject newLine = Instantiate(LineObject); 
            newLine.transform.parent = transform;
            pathLines.Add(newLine);

            LineRenderer _trajectoryLine = newLine.GetComponent<LineRenderer>();

            _trajectoryLine.enabled = true;
            _trajectoryLine.positionCount = 2;

            _trajectoryLine.SetPosition( 0, transform.position );
            _trajectoryLine.SetPosition( 1, nextPoints[i].transform.position );
            
        }
    }

    public void hideLine(){
        foreach (Transform child in transform) 
            GameObject.Destroy(child.gameObject);
    }
    
    GameObject selectedTargetPoint;
    public GameObject getHoverPathPoint( Vector3 playerPos, gamePlayer playerData, Color playerColor ){

        if( playerData.gamepadData[0] == 0 && playerData.gamepadData[1] == 0 )
            return selectedTargetPoint;

        Vector2 mouseDir = Utility.RotateV2( new Vector2(playerData.gamepadData[0], playerData.gamepadData[1]), cam.transform.eulerAngles.y );
        float mouseAngle = Vector2.SignedAngle( Vector2.right, mouseDir );
        mouseAngle = mouseAngle < 0 ? 360 + mouseAngle : mouseAngle;

        Debug.DrawLine(
                transform.position,
                new Vector3(mouseDir[0], 0f, mouseDir[1]) * 2 + transform.position
            );
        Debug.DrawLine(
                transform.position,
                selectedTargetPoint.transform.position
            );

        for (int i = 0; i < selectPathLimits.Count; i++)
        {
            Vector2 limitAngles = selectPathLimits[i];
            if( limitAngles.x > limitAngles.y )
            {
                limitAngles += new Vector2( -360, 0f);
                mouseAngle = mouseAngle < 0 ? mouseAngle - 360 : mouseAngle;
            }

            if( limitAngles.x < mouseAngle && mouseAngle < limitAngles.y ){
                    if( selectedTargetPoint != nextPoints[(int)selectPathLimits[i].z] ){
                        selectedTargetPoint = nextPoints[(int)selectPathLimits[i].z];
                        newTargetPoint(playerColor);
                    }
                    break;
                }
        }

        return selectedTargetPoint;
        
    }

    void newTargetPoint( Color playerColor ){
        
        for( int i = 0; i < nextPoints.Count; i++ )
        {
            var point = nextPoints[i];
            var lineColor = Color.white;
            if( point == selectedTargetPoint)
                lineColor = playerColor;

            if( pathLines.Count == 0 ) showLine();
            pathLines[i]
                .GetComponent<LineRenderer>().material.color = lineColor;

            var currentTargetPoint = Utility.getBPM(point);
            var nextTargetPoint = Utility.getBPM(currentTargetPoint.nextPoints[0]);

            while( currentTargetPoint.nextPoints.Count == 1 && currentTargetPoint != Utility.getBPM(gameObject) )
            {        
                if( currentTargetPoint.pathLines.Count == 0 )
                    currentTargetPoint.showLine();
                else
                    currentTargetPoint.pathLines[0].GetComponent<LineRenderer>().material.color = lineColor;

                var aux = currentTargetPoint;
                currentTargetPoint = nextTargetPoint;
                nextTargetPoint = Utility.getBPM(aux.nextPoints[0]);
            }
        }            
    }
}
