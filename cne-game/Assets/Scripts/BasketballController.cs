using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballController : MonoBehaviour
{

    public float MoveSpeed = 10;
    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;
    public GameObject pointText;

    // variables
    public bool IsBallInHands = false;
    public bool IsBallFlying = false;
    private float T = 0;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
       

        // walking
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);

        // ball in hands
        if (IsBallInHands)
        {

            // hold over head
            if (Input.GetKey(KeyCode.Space))
            {
                Ball.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;

                // look towards the target
                transform.LookAt(Target.parent.position);
            }

            // dribbling
            else
            {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            // throw ball
            if (Input.GetKeyUp(KeyCode.Space))
            {
                IsBallInHands = false;
                timer = -3f;
                IsBallFlying = true;
                T = 0;
                //pointText++
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
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Basketball" && 
            !IsBallInHands && !IsBallFlying)
        {

            Debug.Log("trigger");
            IsBallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}