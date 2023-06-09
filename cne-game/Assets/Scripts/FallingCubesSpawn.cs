using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FallingCubesSpawn : MonoBehaviour
{

    public GameObject cubePrefab;

    public int gridWeight = 10;
    public float cubeWeight = 1;

    public float inAirTime;
    public float fallingTime;
    public float angularSmooth;

    float timer;
    public float riseSpeed;
    string cubesState = "inAir";

    private GameObject[,] cubeMatrix = new GameObject[ 10, 10 ];

    public delegate void actionsDelegate( GameObject cube, float aux );
    List<actionsDelegate> cubeActions = new List<actionsDelegate>();

    void fallCube( GameObject cube ){

        var cubeRb = cube.GetComponent<Rigidbody>();
        cubeRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        cubeRb.useGravity = true;
        cubeRb.angularVelocity = new Vector3( Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1));
    }


    // Start is called before the first frame update
    void Start()
    {

        cubeActions.Add( ( GameObject cube, float aux ) => { 
            bool[] conditions = new bool[]{
                cube.transform.position.x > 0,
                cube.transform.position.x < 0,
                cube.transform.position.z > 0,
                cube.transform.position.z < 0
            };
            int index = (int) Mathf.Round( aux % conditions.Length );
            if( conditions[ index  ] ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube, float aux ) => { // Filas Pares
             if( (cube.transform.position.x + cubeWeight/2) % 2 != 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube, float aux ) => { // Columnas Pares
             if( (cube.transform.position.z + cubeWeight/2) % 2 != 0 ) { fallCube( cube ); } 
        } );
        cubeActions.Add( ( GameObject cube, float aux ) => { // Circulo
             if( Vector3.Distance( cube.transform.position, Vector3.zero ) > gridWeight/3 ) { fallCube( cube ); } 
        } );
        cubeActions.Add((GameObject cube, float aux) => { // Circulo
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

        if (timer >= inAirTime && cubesState == "inAir") {
            fallCubes();
            cubesState = "falling";
            timer = 0;
        }

        if(timer >= fallingTime  && cubesState == "falling")
        {
            if( riseCubes() == 0 ){
                
                setCubes();
                cubesState = "inAir";
                timer = 0;
            };

        }
    }

    void setCubes(){
        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cubeMatrix.GetLength(0); j++)
            {
                var currentCube = cubeMatrix[i, j];
                var cubePos = currentCube.transform.position;
                cubePos = new Vector3( cubePos.y, 0f, cubePos.z );
                currentCube.transform.rotation = Quaternion.identity;
                currentCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation ;
   
            } 
        }
    }

    private float riseCubes()
    {

        float diffCount = 0;

        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cubeMatrix.GetLength(0); j++)
            {
                cubeMatrix[i, j].GetComponent<Rigidbody>().useGravity = false;
                cubeMatrix[i, j].GetComponent<Rigidbody>().velocity = Vector3.zero;
                cubeMatrix[i, j].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                
                Vector3 cubePos = cubeMatrix[i, j].transform.position;

                Vector3 newPos = Vector3.Lerp(cubePos,
                    new Vector3(cubePos.x, transform.position.y , cubePos.z), riseSpeed);

                newPos.y = newPos.y > transform.position.y - 0.01f ? transform.position.y : newPos.y; 

                cubeMatrix[i, j].transform.position = new Vector3( Mathf.Round( cubePos.x * 10 ) / 10, newPos.y, Mathf.Round( cubePos.z * 10  ) / 10 );
                cubeMatrix[i, j].transform.rotation = Quaternion.Lerp(cubeMatrix[i, j].transform.rotation, Quaternion.identity, angularSmooth);

                diffCount += transform.position.y-newPos.y;

            }
        }
        
        return diffCount;
    }

    private void fallCubes()
    {
        actionsDelegate d = cubeActions[ UnityEngine.Random.Range(0, cubeActions.Count) ];
        float aux = Random.value;
        //d = cubeActions[3];

        for (int i = 0; i < cubeMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < cubeMatrix.GetLength(0); j++)
            {
                d(cubeMatrix[i, j], aux);
            } 
        }
    }

}
