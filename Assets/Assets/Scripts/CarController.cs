using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    // Using a lot of reference from Yoav's car controller from CARKOUR!!
    [Header("Setup")]
    [SerializeField] private PlayerInput _playerInput;
    //Serialising separately instead of as an array because it's easier than indexing later
    [SerializeField] private WheelHandler frWheel;
    [SerializeField] private WheelHandler flWheel;
    [SerializeField] private WheelHandler brWheel;
    [SerializeField] private WheelHandler blWheel;
    private List<WheelHandler> allWheels;

    [SerializeField] private Rigidbody2D carBody;

    [Header("Physics")] 
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve reverseCurve;
    //Not sure if it will be necessary but just in case
    [SerializeField] private AnimationCurve steeringCurve;
    [SerializeField] private AnimationCurve softVelocityCurve;
    [SerializeField] private float rubberBandAmount;
    [SerializeField] private float softVelocityCap;
    [SerializeField] private float hardVelocityCap;
    
    [Header("Listeners")]
    [SerializeField] private Vector2 currentMoveDir = Vector2.zero;
    [SerializeField] private float currentAcceleration = 0;
    [SerializeField] private float currentBreakForce = 0;
    [SerializeField] private bool isBoosting;

    [Header("State Control")] 
    [SerializeField] private CarStates currentState;
    
    public Vector2 CurrentMoveDir => currentMoveDir;

    public float CurrentAcceleration => currentAcceleration;

    public float CurrentBreakForce => currentBreakForce;

    public CarStates CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    [Header("Debug")]
    [SerializeField] private TMP_Text debugText;

    [Serializable]
    public enum CarStates
    {
        Actionable,
        Stunned,
        Dead
    }

    /*private void OnEnable()
    {
        _playerInput.actions.Enable();
    }*/

    private void OnDisable()
    {
        _playerInput.actions.Disable();
    }

    void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
        allWheels = new List<WheelHandler> { frWheel, brWheel, blWheel, flWheel };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Physics

        if (isBoosting)
        {
            
        }
        
        //steering control
        float steerAngle = steeringCurve.Evaluate(Mathf.Abs(currentMoveDir.x)) * (currentMoveDir.x <=0 ? 1 : -1);
        flWheel.gameObject.transform.localRotation = Quaternion.Euler(0,0,steerAngle);
        frWheel.gameObject.transform.localRotation = Quaternion.Euler(0,0,steerAngle);
        
        //RubberBanding
        DoHardVelocityClamp();
        DoRubberBanding();

        #endregion

        #region Debug

        printDebug();

        #endregion
    }

    private void DoHardVelocityClamp()
    {
        //getting current linear velocity
        float currentSpeed = carBody.linearVelocity.magnitude;

        if (currentSpeed > hardVelocityCap)
        {
            //Finding the capped velocity in the direction of movement
            Vector2 desiredVel = carBody.linearVelocity.normalized * hardVelocityCap;

            //Getting the difference between the desired velocity and the current one
            Vector2 velocityDiff = desiredVel - carBody.linearVelocity;
        
            carBody.linearVelocity += velocityDiff;   
        }
    }

    private void DoRubberBanding()
    {
        //if (currentState == CarStates.Dead) return;
        //getting current linear velocity
        float currentSpeed = carBody.linearVelocity.magnitude;

        //Finding the capped velocity in the direction of movement
        Vector2 desiredVel = carBody.linearVelocity.normalized * Mathf.Min(currentSpeed, softVelocityCap);

        //Getting the difference between the desired velocity and the current one
        Vector2 velocityDiff = desiredVel - carBody.linearVelocity;

        //Getting the adjustment needed
        Vector2 velocityAdj = velocityDiff * rubberBandAmount;
        carBody.linearVelocity += velocityAdj;
    }

    private void printDebug()
    {
        if (debugText) debugText.text = $"Accel with val of {currentAcceleration}\n" +
                                        $"Reverse with val of {currentBreakForce}\n" +
                                        $"Movedir - x:{currentMoveDir.x} y:{currentMoveDir.y}\n" +
                                        $"Boosting? {isBoosting}";
    }

    private void OnDrive()
    {
        if (!enabled) return;

        if (currentState == CarStates.Actionable)
        {
            float rawInputVal = _playerInput.actions.FindAction("Drive").ReadValue<float>();
            //if (debugText) debugText.text = $"Drive with val of {rawInputVal}";
            currentAcceleration = accelCurve.Evaluate(rawInputVal);
            softVelocityCap = softVelocityCurve.Evaluate(rawInputVal);
        }
    }

    private void OnReverse()
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            float rawInputVal = _playerInput.actions.FindAction("Reverse").ReadValue<float>();
            //if (debugText) debugText.text = $"Reverse with val of {rawInputVal}";
            currentBreakForce = reverseCurve.Evaluate(rawInputVal);   
            softVelocityCap = softVelocityCurve.Evaluate(rawInputVal);
        }
    }

    private void OnMove()
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            currentMoveDir = _playerInput.actions.FindAction("Move").ReadValue<Vector2>();   
        }
    }

    private void OnBoost()
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            isBoosting = (_playerInput.actions.FindAction("Boost").ReadValue<int>() == 1);
        }
    }
}
