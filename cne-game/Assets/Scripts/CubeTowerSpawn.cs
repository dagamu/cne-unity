using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTowerSpawn : MonoBehaviour
{

    public float startY;
    public float zPos;
    public float maxX;
    public float speed;
    float startX;

    public string keySpawn;
    public GameObject cubePrefab;
    public GameObject Tower;
    public LayerMask towerCubeLayer;

    int siblingIndex;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        siblingIndex = transform.GetSiblingIndex();
        startX = siblingIndex * 8 - 18 + 0.5f;
        transform.position = new Vector3( startX, startY, zPos);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float newX = startX + (Mathf.Sin(timer * speed) / 2 + 1f) * maxX;
        newX = (float)Mathf.Round(newX) + 0.5f; 
        transform.position = new Vector3( newX, transform.position.y , zPos );

        if (Input.GetKeyDown(keySpawn))
        {
            var newCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            newCube.transform.parent = Tower.transform;

            if ( Physics.Raycast(transform.position, Vector3.down, 1) )
            {
                transform.position += new Vector3(0f, 1f, 0f);
            }
        }

    }
}
