using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using gamePlayerSpace;

public class RampLinesController : MonoBehaviour
{

    public GameObject PlayersParent;
    float timer;

    public float lineSpeed;

    Transform PlayerToFollow;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        
        if( timer >= 1f && PlayerToFollow == null ){

            if( transform.GetSiblingIndex() < PlayersParent.transform.childCount ){
                PlayerToFollow = PlayersParent.transform.GetChild( transform.GetSiblingIndex() );
                gameObject.GetComponent<Renderer>().enabled = true;
                Color lineColor = PlayerToFollow.GetComponent<playerController>().playerColor;;
                lineColor.a = 0.3f;
                gameObject.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
                gameObject.GetComponent<Renderer>().material.shader = Shader.Find( "Transparent/Diffuse" );
                gameObject.GetComponent<Renderer>().material.color = lineColor;
            }

           
        } else if( PlayerToFollow != null ){

                Vector3 playerFollowZ =  new Vector3( 0f, 0f, PlayerToFollow.position.z );
                Vector3 posZ =  new Vector3( 0f, 0f, transform.position.z);

                if( PlayerToFollow.position.z > transform.position.z ){
                    transform.position += transform.forward * lineSpeed * Time.deltaTime ;
                }

        }


        
        
    }
}
