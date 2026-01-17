using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum ControlMode { Keyboard}
    public enum Axel { Front, Rear }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public ControlMode control = ControlMode.Keyboard;

    [Header("Drive Settings")]
    public float maxAcceleration = 600f;
    public float brakeForce = 3000f;
    public float deceleration = 50f;

    [Header("Steering Settings")]
    public float turnSensitivity = 2f;
    public float maxSteerAngle = 45f;

    public Vector3 centerOfMass = new Vector3(0f, -0.5f, 0f);

    public List<Wheel> wheels = new List<Wheel>();

    float moveInput;
    float steerInput;

    Rigidbody carRb;

    // Store original values for reset
    [HideInInspector] public float originalAcceleration;
    [HideInInspector] public float originalBrake;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = centerOfMass;

        originalAcceleration = maxAcceleration;
        originalBrake = brakeForce;
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
    }

    void FixedUpdate()
    {
        ApplyReverseLogic();
        Move();
        Steer();
        Brake();
        NaturalDeceleration();
    }

    
    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }
    }

    
    void Move()
    {
        
        float torque = moveInput * maxAcceleration;
        
        if (moveInput > 0.1f)                    
    {
        GlobalAudio.Instance.StartEngineSound();
    }
        else if (moveInput < -0.1f)              
    {
        GlobalAudio.Instance.StartReverseSound();
    } 
        else                                      
    {
        GlobalAudio.Instance.StopEngineSound();
    }

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Rear)
            {
                wheel.wheelCollider.motorTorque = torque;
            }
        }
    }

    void Steer()
    {
        float targetAngle = steerInput * turnSensitivity * maxSteerAngle;

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                wheel.wheelCollider.steerAngle =
                    Mathf.Lerp(wheel.wheelCollider.steerAngle, targetAngle, Time.fixedDeltaTime * 8f);
            }
        }
    }

    void Brake()
{
    bool braking = Input.GetKey(KeyCode.Space);

    if (braking)
    {
        
        GlobalAudio.Instance.StopEngineSound();
        GlobalAudio.Instance.StopReverseSound();

    
        GlobalAudio.Instance.StartBrakeLoop();
    }
    else
    {
    
        GlobalAudio.Instance.StopBrakeLoop();
    }


    foreach (var wheel in wheels)
    {
        wheel.wheelCollider.brakeTorque = braking ? brakeForce : 0f;
    }
}
    void NaturalDeceleration()
    {
    
        if (Mathf.Abs(moveInput) < 0.05f && !Input.GetKey(KeyCode.Space))
        {
            carRb.linearVelocity = Vector3.Lerp(carRb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * (deceleration / 10f));
        }
    }

    
    void ApplyReverseLogic()
    {
        bool tryingToReverse = (moveInput < 0);

        // If moving forward & suddenly pressing reverse → apply brake first
        if (tryingToReverse && carRb.linearVelocity.magnitude > 2f)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0;
                wheel.wheelCollider.brakeTorque = brakeForce;
            }
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    public void ResetCar()
    {
        if (carRb == null)
        carRb = GetComponent<Rigidbody>();
        maxAcceleration = originalAcceleration;
        brakeForce = originalBrake;

        // Stop car instantly
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;
    }
}
