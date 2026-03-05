using System;
using System.Collections;
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
    [SerializeField] [Range(0,1f)] private float baseTraction;
    [FormerlySerializedAs("driftTraction")][SerializeField] [Range(0,1f)] private float driftTractionBack;
    [SerializeField] [Range(0,2f)] private float driftTractionFront;
    
    [Header("Debug")]
    [SerializeField] private float currentMoveDir = 0;
    [SerializeField] private float currentAcceleration = 0;
    [SerializeField] private float currentBreakForce = 0;
    [SerializeField] private bool isBoosting;
    [SerializeField] private bool isDrifting;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isAbilityOn;

    [Header("State Control")] 
    [SerializeField] private CarStates currentState;
    
    public float CurrentMoveDir => currentMoveDir;

    public float CurrentAcceleration => currentAcceleration;

    public float CurrentBreakForce => currentBreakForce;

    public float BaseTraction => baseTraction;

    public float DriftTractionBack => driftTractionBack;

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
    private AbilityController abilityController;

    private void OnEnable()
    {
        _playerInput.actions.Enable();
    }

    private void OnDisable()
    {
        _playerInput.actions.Disable();
    }

    void Start()
    {
        carBody = GetComponent<Rigidbody2D>();
        abilityController = GetComponent<AbilityController>();
        //going clockwise around the car body, starting at the top right
        allWheels = new List<WheelHandler> { frWheel, brWheel, blWheel, flWheel };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentSpeed = carBody.linearVelocity.magnitude;
        #region Physics

        //RubberBanding
        DoHardVelocityClamp();
        if (!isBoosting)
        {
            DoRubberBanding();
        }
        
        //steering control
        float steerAngle = steeringCurve.Evaluate(Mathf.Abs(currentMoveDir)) * (currentMoveDir <=0 ? 1 : -1);
        flWheel.gameObject.transform.localRotation = Quaternion.Euler(0,0,steerAngle);
        frWheel.gameObject.transform.localRotation = Quaternion.Euler(0,0,steerAngle);

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
                                        $"Movedir - x:{currentMoveDir}\n" +
                                        $"Boosting? {isBoosting}";
    }

    public void OnDrive(InputAction.CallbackContext context)
    {
        if (!enabled) return;

        if (currentState == CarStates.Actionable)
        {
            float rawInputVal = context.ReadValue<float>();;
            //if (debugText) debugText.text = $"Drive with val of {rawInputVal}";
            currentAcceleration = accelCurve.Evaluate(rawInputVal)* (isBoosting?1.3f:1);
            softVelocityCap = softVelocityCurve.Evaluate(rawInputVal);
        }
    }

    public void OnReverse(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        
        if (currentState == CarStates.Actionable)
        {
            float rawInputVal = context.ReadValue<float>();
            //if (debugText) debugText.text = $"Reverse with val of {rawInputVal}";
            currentBreakForce = reverseCurve.Evaluate(rawInputVal);   
            softVelocityCap = softVelocityCurve.Evaluate(rawInputVal);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            currentMoveDir = context.ReadValue<float>();
            //currentMoveDir = _playerInput.actions.FindAction("Move").ReadValue<Vector2>();   
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            isBoosting = context.performed;
        }
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        //targeting only back wheels
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            float newTraction = context.performed ? driftTractionBack : baseTraction;

            allWheels[1].GripFactor = newTraction;
            allWheels[2].GripFactor = newTraction;
            
            float newTractionFront = context.performed ? driftTractionFront : baseTraction;
            
            allWheels[0].GripFactor = newTractionFront;
            allWheels[3].GripFactor = newTractionFront;

            isDrifting = context.performed;
        }
    }
    

    /// <summary>
    /// Coroutine for stunning the car
    /// </summary>
    /// <param name="stunTime">The amount of time, in seconds, to stun the car</param>
    public IEnumerator DoStun(float stunTime)
    {
        foreach (var wheel in allWheels)
        {
            wheel.GripFactor = 0f;
        }

        isBoosting = false;
        currentBreakForce = 0;
        currentAcceleration = 0;
        softVelocityCap = 50;

        currentState = CarStates.Stunned;
        
        yield return new WaitForSeconds(stunTime);
        
        _playerInput.actions.Disable();
        _playerInput.actions.Enable();
        
        foreach (var wheel in allWheels)
        {
            wheel.GripFactor = baseTraction;
        }

        softVelocityCap = 0;
        currentState = CarStates.Actionable;
    }

    public void GetStunned(float stunTime)
    {
        StartCoroutine(DoStun(stunTime));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && isBoosting)
        {
            other.gameObject.GetComponent<CarController>().GetStunned(1.5f);
        }
    }

    private void OnAbility ()
    {
        if (!enabled) return;
        if (currentState == CarStates.Actionable)
        {
            isAbilityOn = _playerInput.actions.FindAction("Ability").ReadValue<float>() == 1;
            Debug.Log($"Ability button pressed: {isAbilityOn}");
            if (isAbilityOn)
                abilityController.bumperAbility.Activate(this);
        }
    }
}
