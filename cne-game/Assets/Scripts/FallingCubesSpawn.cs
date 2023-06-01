using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallingCubesSpawn : MonoBehaviour
{

    public GameObject cubePrefab;

    public int gridWeight = 10;
    public float cubeWeight = 1;

    private GameObject[,] cubeMatrix = new GameObject[ 10, 10 ];

    public delegate void actionsDelegate( GameObject cube );
    List<actionsDelegate> cubeActions = new List<actionsDelegate>();

    void fallCube( GameObject cube ){
        cube.GetComponent<Rigidbody>().useGravity = true;
    }

    void RestartCubes(){

    }

    void LoopCubeMatrix(){
        
    }

    // Start is called before the first frame update
    void Start()
    {

        cubeActions.Add( ( GameObject cube ) => {
             if( cube.transform.position.x > 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube ) => {
             if( (cube.transform.position.x + cubeWeight/2) % 2 != 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube ) => {
             if( Vector3.Distance( cube.transform.position, Vector3.zero ) > gridWeight/3 ) { fallCube( cube ); } 
        } );

        foreach (var column in Enumerable.Range( 0, gridWeight))
        {
            foreach (var row in Enumerable.Range( 0, gridWeight) )
            {
                Vector3 cubePos = new Vector3( 
                    ( (column) - gridWeight / 2 ) + cubeWeight / 2,
                    transform.position.y,
                    ( (row) - gridWeight / 2 ) + cubeWeight / 2
                     );

                cubeMatrix[column, row] = Instantiate( cubePrefab, cubePos, Quaternion.identity );
                cubeMatrix[column, row].transform.parent = transform;

            }
        }

        actionsDelegate d = cubeActions[UnityEngine.Random.Range( 0, cubeActions.Count )];
         d = cubeActions[2];

        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
             for (int j = 0; j < cubeMatrix.GetLength(0); j++)
                {
                    d( cubeMatrix[i,j] );
                }
        }


    }

}
