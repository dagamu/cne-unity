using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Countdown : MonoBehaviour
{

    public float timeLeft = 3.0f;
    public TMP_Text startText; //used for showing countdown from 3,2,1 


    void Update()
    {
        timeLeft -= Time.deltaTime;
        startText.SetText((timeLeft).ToString("0"));
        if (timeLeft < 0)
        {
            startText.SetText("");
            Destroy(startText.gameObject, 2f);
            //Do something useful or Load a new game scene
        }
    }
}