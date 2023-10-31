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
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    public gamePlayer playerData;

    [SerializeField] public float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    public float kartSpeed = 0;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        transform.eulerAngles += -Vector3.forward * transform.eulerAngles.z;
    }


    float breakTime = 0;
    private void GetInput()
    {
        horizontalInput = playerData.gamepadData[0];
        verticalInput = playerData.gamepadData[3];
        isBreaking = playerData.gamepadData[5] == 1;

        if( isBreaking ){
            breakTime += Time.deltaTime;
            if( breakTime > 2 ){ 
                verticalInput = -1;
                isBreaking = false;
            }
            
        } else breakTime = 0;
    }

    private void HandleMotor()
    {
        var input = new Vector2(horizontalInput, verticalInput);
        if( input.magnitude > 0.1 ){
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce * Mathf.Pow(10, 30) * Time.deltaTime * kartSpeed;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce * Mathf.Pow(10, 30) * Time.deltaTime * kartSpeed;
        }
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }   

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot
;       wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}