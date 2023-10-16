using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gamePlayerSpace;
using TMPro;

public class MinigameUIManager : MonoBehaviour
{
    public Transform pointBoxParents; 
    GameObject playersParernt;
    bool uiSetted = false;
    float pointsVel;
    void Update(){
        if( uiSetted ){
            for( int i = 0; i < pointBoxParents.childCount; i++ )
            {
                var pointBox = pointBoxParents.GetChild(i);
                var player = playersParernt.transform.GetChild(i);
                var textComp = pointBox.Find("Points").GetComponent<TextMeshProUGUI>();

                var currentPoints = int.Parse(textComp.text);
                var playerPoints = Utility.getController(player.gameObject).MinigamePoints;

                var pointText = Mathf.Abs(currentPoints-playerPoints) < 10 ? playerPoints : Mathf.Round( Mathf.SmoothDamp( currentPoints, playerPoints, ref pointsVel, 0.3f) );
                textComp.SetText(pointText.ToString());
            }
        }
    }
    public void setUI( GameObject pParernt )
    {
        playersParernt = pParernt;
        for( int i = 0; i < playersParernt.transform.childCount; i++ )
        {
            var pController = Utility.getController(playersParernt.transform.GetChild(i).gameObject);

            Debug.Log(pointBoxParents.GetChild(i).gameObject.name);
            pointBoxParents.GetChild(i).Find("Outline").GetComponent<Image>().color = pController.playerColor;
            pointBoxParents.GetChild(i).Find("Points").GetComponent<TextMeshProUGUI>().color = pController.playerColor;
            
            if( playersParernt.transform.childCount > pointBoxParents.childCount ){
                Instantiate( pointBoxParents.GetChild(0), pointBoxParents );
            }
        }
        uiSetted = true;
    }
}
