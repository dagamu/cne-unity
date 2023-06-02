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

    float timer;
    public float riseSpeed;
    string cubesState = "inAir";

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

        cubeActions.Add( ( GameObject cube ) => { //
             if( cube.transform.position.x > 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube ) => { // Filas Pares
             if( (cube.transform.position.x + cubeWeight/2) % 2 != 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube ) => { // Círculo
             if( Vector3.Distance( cube.transform.position, Vector3.zero ) > gridWeight/3 ) { fallCube( cube ); } 
        } );
        cubeActions.Add((GameObject cube) => { // Círculo
            if (Vector3.Distance(cube.transform.position, Vector3.zero) < gridWeight / 3) { fallCube(cube); }
        });

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

        


    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2f && cubesState == "inAir") {
            fallCubes();
            cubesState = "falling";
            timer = 0;
        }

        if(timer >= 2f && cubesState == "falling")
        {
            riseCubes();
        }
    }

    private void riseCubes()
    {
        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cubeMatrix.GetLength(0); j++)
            {
                cubeMatrix[i, j].GetComponent<Rigidbody>().useGravity = false;
                Vector3 cubePos = cubeMatrix[i, j].transform.position;

                Vector3 newPos = Vector3.Lerp(cubePos,
                    new Vector3(cubePos.x, 0, cubePos.z), riseSpeed);
                cubeMatrix[i, j].transform.position = new Vector3(cubePos.x, newPos.y, cubePos.z);



            }
        }
    }

    private void fallCubes()
    {
        actionsDelegate d = cubeActions[UnityEngine.Random.Range(0, cubeActions.Count)];
        //d = cubeActions[3];

        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cubeMatrix.GetLength(0); j++)
            {
                d(cubeMatrix[i, j]);
            }
        }
    }

}
