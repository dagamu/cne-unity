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
}