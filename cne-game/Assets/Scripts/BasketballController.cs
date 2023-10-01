using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BasketballController : MonoBehaviour
{

    public Transform Ball;
    Transform PosDribble;
    Transform PosOverHead;
    Transform Arms;
    Transform Target;
    public GameObject pointText;

    // variables
    public bool IsBallInHands = false;
    public bool IsBallFlying = false;
    private float T = 0;
    [SerializeField] private AudioSource CanastaSoundEffect;
    private float timer = 0f;

    bool previusShootStatus = false;

    public void setPoints(){

        var modelTransform = GetComponent<playerController>().playerModel.transform;
        PosDribble = modelTransform.Find("Points/Cadera");
        PosOverHead = modelTransform.Find("Points/Cabeza");
        Arms = modelTransform.Find("Points/Mano");
        Target = GameObject.Find("BasketballTarget").transform;
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        // ball in hands
        if (IsBallInHands)
        {

            // hold over head
            if ( Utility.getData(gameObject).gamepadData[2] == 1 )
            {
                previusShootStatus = Utility.getData(gameObject).gamepadData[2] == 1;
                Ball.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;

                // look towards the target
                transform.LookAt(Target.position);
            }

            // dribbling
            else
            {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            // throw ball
            if ( Utility.getData(gameObject).gamepadData[2] == 0 && previusShootStatus  )
            {
                previusShootStatus = false;
                //CanastaSoundEffect.Play();
                IsBallInHands = false;
                timer = -3f;
                IsBallFlying = true;
                T = 0;

               
            }
        } 
        

        // ball in the air
        if (IsBallFlying)
        {
            T += Time.deltaTime;
            float duration = 0.66f;
            float t01 = T / duration;

            // move to target
            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);

            // move in arc
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            Ball.position = pos + arc;

            // moment when ball arrives at the target
            if (t01 >= 1)
            {
                /*var pText = pointText.GetComponent<TMP_Text>();
                pText.SetText((int.Parse(pText.text) + 1).ToString());*/
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Basketball" && 
            !IsBallInHands && !IsBallFlying)
        {

            Ball = other.transform;
            IsBallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}