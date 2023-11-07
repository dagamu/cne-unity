using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamePlayerSpace;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;

    public Rigidbody sphereRB;

    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public float backwardsTime;
    public float yOffset;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;
    public bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;
    public float modifiedAngularDrag;

    public float alignToGroundTime;

    public float maxAngVel;


    public gamePlayer playerData;

    [Header("Car Settings")]
   

    public float kartSpeed;

    void Start()
    {
        sphereRB.GetComponent<MotorController>().carParent = gameObject;
        sphereRB.transform.parent = null;
        kartSpeed = 0;
        normalDrag = sphereRB.drag;
    }

    void Update()
    {

        GetInput();
        // Get Input
        moveInput = verticalInput * kartSpeed;
        turnInput = horizontalInput;

        // Calculate Turning Rotation
        float newRot = turnInput * turnSpeed * Time.deltaTime * sphereRB.velocity.magnitude;

        if (isCarGrounded)
            transform.Rotate(0, newRot, 0, Space.World);

        // Set Cars Position to Our Sphere
        transform.position = sphereRB.transform.position;

        // Raycast to the ground and get normal to align car with it.
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        // Rotate Car to align with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        // Calculate Movement Direction
        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        // Calculate Drag
        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;
        //sphereRB.angularDrag = isBreaking ? 0 : modifiedAngularDrag;

        //sphereRB.angularVelocity = sphereRB.angularVelocity.y > maxAngVel ? sphereRB.angularVelocity.normalized * maxAngVel : sphereRB.angularVelocity;

    }

    private void FixedUpdate()
    {

        if (isCarGrounded)
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration); // Add Movement
        else
            sphereRB.AddForce(transform.up * -200f); // Add Gravity

        transform.eulerAngles += -Vector3.forward * transform.eulerAngles.z;
    }


    float breakTime = 0;
    bool isBreaking;

    private void GetInput()
    {
        horizontalInput = playerData.gamepadData[0];
        verticalInput = playerData.gamepadData[3];
        isBreaking = playerData.gamepadData[5] == 1;

        if (playerData.gamepadData[2] == 1)
        {
            sphereRB.transform.position = GetComponent<RaceCarController>()
                    .CheckPointParent.GetChild(GetComponent<RaceCarController>().nextCheckpoint.GetSiblingIndex() - 1).transform.position + Vector3.up;
            sphereRB.velocity = Vector3.zero;
            sphereRB.angularVelocity = Vector3.zero;
        }

        if( isBreaking ){
            breakTime += Time.deltaTime;
            if( breakTime > backwardsTime )
            { 
                verticalInput = -1;
                isBreaking = false;
            }
            
        } else breakTime = 0;
    }

    
}