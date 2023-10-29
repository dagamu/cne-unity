using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceCarController : MonoBehaviour
{
    public int Laps = 0;
    Transform CheckPointParent;
    public Transform nextCheckpoint;
    public List<GameObject> playersPositions = new List<GameObject>();
    public GameObject PositionLabel, CharacterModel;

    public int position;

    void Start(){
        CheckPointParent = GameObject.Find("Checkpoints").transform;
        nextCheckpoint = CheckPointParent.GetChild(1);
        foreach( Transform p in transform.parent ){
            playersPositions.Add(p.gameObject);
        }
    }
    float timer;
    void Update(){
        timer += Time.deltaTime;
        Debug.DrawLine( transform.position, nextCheckpoint.position );
        if( timer < 3 ) return;
        setPosition();
        
    }

    void setPosition(){
        for( var i = 0; i < playersPositions.Count; i++){
            for( var j = i+1; j < playersPositions.Count; j++){

                var higher = false;
                var iCtr = playersPositions[i].GetComponent<RaceCarController>();
                var jCtr = playersPositions[j].GetComponent<RaceCarController>();

                if( iCtr.Laps > jCtr.Laps){ // Higher Lap
                    higher = true;
                } else if (iCtr.Laps == jCtr.Laps) {
                    if( iCtr.nextCheckpoint.GetSiblingIndex() > jCtr.nextCheckpoint.GetSiblingIndex() ){ // Higher Checkpoint
                        higher = true;
                    } else if( iCtr.nextCheckpoint.GetSiblingIndex() == jCtr.nextCheckpoint.GetSiblingIndex()){
                        var iDisVec = iCtr.nextCheckpoint.position - iCtr.transform.position;
                        var jDisVec = jCtr.nextCheckpoint.position - jCtr.transform.position;
                        if( iDisVec.magnitude < jDisVec.magnitude){ // Near to Checkpoint
                            higher = true;
                        }
                    }
                }

                if( higher ){
                    var aux = playersPositions[i];
                    playersPositions[i] = playersPositions[j];
                    playersPositions[j] = aux;
                } 
            }
        }
        
        position = playersPositions.Count - playersPositions.IndexOf(gameObject);
        PositionLabel.GetComponent<TMP_Text>().SetText( position.ToString() + "°");
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.transform == nextCheckpoint){
            if( nextCheckpoint.GetSiblingIndex() == CheckPointParent.childCount - 1 ){
                nextCheckpoint = CheckPointParent.GetChild(0);          
            } else if( nextCheckpoint.GetSiblingIndex() == 0){
                Laps++;
                nextCheckpoint = CheckPointParent.GetChild(1);  
            } else {
                nextCheckpoint = CheckPointParent.GetChild(nextCheckpoint.GetSiblingIndex() + 1);
            }   
            
        }
    }


}
