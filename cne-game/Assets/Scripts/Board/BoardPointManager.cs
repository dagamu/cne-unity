using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPointManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> nextPoints = new List<GameObject>();
    public List<GameObject> pathLines = new List<GameObject>();
    public GameObject LineObject; 

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
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

}
