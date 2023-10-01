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
}