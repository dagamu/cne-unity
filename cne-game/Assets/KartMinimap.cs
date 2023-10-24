using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartMinimap : MonoBehaviour
{
   
    public float scale;
    public Transform playersParent;
    public Transform Map;
    public float MapRoation;
    public Vector2 Offset;


    // Update is called once per frame
    void Update()
    {
        for( var i = 0; i < playersParent.childCount; i++){
            if( i > transform.childCount - 1) Instantiate( transform.GetChild(0), transform );
            var mapImg = transform.GetChild(i);
            var playerObj = playersParent.GetChild(i);
            var playerPos = playerObj.position;

            mapImg.localPosition = Rotate( new Vector2( playerPos.x, playerPos.z ), MapRoation)  * scale + Offset;
        }
    }

    public Vector2 Rotate(Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}
