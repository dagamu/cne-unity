using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPointManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> nextPoints = new List<GameObject>();
    public GameObject LineObject; 

    void Start()
    {
        for (int i = 0; i < nextPoints.Count; i++ ) 
        {

            GameObject newLine = Instantiate(LineObject); 
            newLine.transform.parent = transform;

            LineRenderer _trajectoryLine = newLine.GetComponent<LineRenderer>();

            _trajectoryLine.enabled = true;
            _trajectoryLine.positionCount = 2;

            _trajectoryLine.SetPosition( 0, transform.position );
            _trajectoryLine.SetPosition( 1, nextPoints[i].transform.position );
            
        }
    }

}
