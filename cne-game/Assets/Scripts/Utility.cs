using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamePlayerSpace;

public static class Utility {
    public static gamePlayer getData( GameObject player ){
        return player.GetComponent<playerController>()
                                    .playerData;
    }
    public static playerController getController( GameObject player ){
        return player.GetComponent<playerController>();
    }

    public static Color stringToColor( string str ){
        string[] colorList = str.Split(';');
        return new Color(
            float.Parse(colorList[0]),
            float.Parse(colorList[1]),
            float.Parse(colorList[2]),
            1f
        );
    }

    public static void DebugLine( string name, List<Vector2> list){
        string result = name + ": ";
        foreach (var item in list)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);
    }

    public static Vector2 ToVector2( Vector3 vec ){
        return new Vector2(vec.x, vec.z );
    }

    public static BoardPointManager getBPM( GameObject point ){
        return point.GetComponent<BoardPointManager>();
    }

    public static Vector2 RotateV2( Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}


}