using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSpawner : MonoBehaviour
{
    
    public GameObject CannonPrefab;
    public float radius;

    class Cannon{

        GameObject obj;
        float currentAngle;
        float targetAnglePos;

        Vector3 target;

        public Cannon( GameObject prefab, Transform parent, float radius ){

            GameObject obj = Instantiate( prefab, parent );
            obj.transform.position = new Vector3( radius, 0f, 0f );

            currentAngle = 0;
            targetAnglePos = Random.Range( 0, 360 );

            target = new Vector3( Random.Range( -radius*2/3, radius*2/3 ), 0f, Random.Range( -radius*2/3, radius*2/3 ) );
        }

        

        public void Move( float radius ){

            float newAngle = Mathf.Lerp( currentAngle, targetAnglePos, 0.1f );
            obj.transform.position = new Vector3( Mathf.Sin(newAngle), 0f, Mathf.Cos(newAngle) ) * radius; 
            currentAngle = newAngle;

            Vector3 targetDirection = target - obj.transform.position;
            Vector3 newDirection = Vector3.RotateTowards( obj.transform.forward, targetDirection, 1.0f, 0.0f);
            obj.transform.rotation = Quaternion.LookRotation( newDirection );
            obj.transform.Rotate( 0, 90, 90 );

        }

    }

    
    List<Cannon> Cannons = new List<Cannon>();


    int Level;
    float timer;

    Cannon test;

    private void AddCannon(){

    }

    void Start()
    {
        AddCannon();
        test = new Cannon( CannonPrefab, transform, radius );
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        test.Move( radius );
        
    }
}
