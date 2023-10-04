using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gamePlayerSpace;
using TMPro;

public class DebugController : MonoBehaviour
{

    public GameObject gamepadBoxContainer;
    string[] btnOrder = {"Up","Down","Left","Right"};

    public void UpdateMovementData( int index, float[] data ){
        var gpBox = gamepadBoxContainer.transform.GetChild(index);
        for( int i = 0; i < btnOrder.Length; i++){
            var btnText = gpBox.transform.Find( btnOrder[i] );
            if( btnText != null){
                btnText.GetComponent<TMP_Text>().SetText(btnOrder[i] + ": " + (data[i+2] == 1 ? "True" : "False"));
            }
        }
        gpBox.transform.Find("Square/Circle").transform.localPosition = new Vector3(data[0],data[1],0) * 20;
        gpBox.transform.Find("Square/Direction")
            .GetComponent<TMP_Text>().SetText(
                    data[0].ToString("F2") + "; " + data[1].ToString("F2")
                    );
        ;
    }

    public void setGamepadBoxes(List<gamePlayer> players){
        for( int i = 0; i < players.Count; i++ ){
            var gpBox = gamepadBoxContainer.transform.GetChild(i);
            gpBox.transform.Find("Square/Circle").GetComponent<Image>().color = Utility.stringToColor( players[i].color );
            if( i + 1 < players.Count){ Instantiate(gpBox, gamepadBoxContainer.transform); }
        }
    }
}
