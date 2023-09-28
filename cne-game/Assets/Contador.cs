using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Contador : MonoBehaviour
{
    DateTime Entrega;
    public TMP_Text messageText;

    void Start(){
        Entrega = new DateTime(2023, 11, 3, 0, 0, 0);
        messageText.SetText( "Faltan: " + Entrega.Subtract(DateTime.Now).ToString("dd") + " días" );
    }
}
