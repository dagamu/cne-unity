using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerBoxController : MonoBehaviour
{
    // Start is called before the first frame update
    public float yPositionTarget, animationSpeed;
    RectTransform rt;
    
    void Start(){
        rt = GetComponent<RectTransform>();
        yPositionTarget = rt.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        rt.localPosition += Vector3.up * (yPositionTarget-rt.localPosition.y) * animationSpeed * Time.deltaTime;
    }
}
