using System;
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
    [SerializeField] private GameObject frWheel;
    [SerializeField] private GameObject flWheel;
    [SerializeField] private GameObject brWheel;
    [SerializeField] private GameObject blWheel;

    [SerializeField] private Rigidbody2D carBody;

    [Header("Physics")] 
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve reverseCurve;
    //Not sure if it will be necessary but just in case
    [SerializeField] private AnimationCurve steeringCurve;
    
    [Header("Listeners")]
    [SerializeField] private Vector2 currentMoveDir = Vector2.zero;
    [SerializeField] private float currentAcceleration = 0;
    [SerializeField] private float currentBreakForce = 0;
    [SerializeField] private bool isBoosting;
    
    [Header("Debug")]
    [SerializeField] private TMP_Text debugText;

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
        debugText = GameObject.Find("DebugText").GetComponent<TMP_Text>();
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
        flWheel.transform.localRotation = Quaternion.Euler(0,0,steerAngle);
        frWheel.transform.localRotation = Quaternion.Euler(0,0,steerAngle);

        #endregion

        #region Debug

        printDebug();

        #endregion
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
        float rawInputVal = _playerInput.actions.FindAction("Drive").ReadValue<float>();
        //if (debugText) debugText.text = $"Drive with val of {rawInputVal}";
        currentAcceleration = accelCurve.Evaluate(rawInputVal);
    }

    private void OnReverse()
    {
        if (!enabled) return;
        float rawInputVal = _playerInput.actions.FindAction("Reverse").ReadValue<float>();
        //if (debugText) debugText.text = $"Reverse with val of {rawInputVal}";
        currentBreakForce = reverseCurve.Evaluate(rawInputVal);
    }

    private void OnMove()
    {
        if (!enabled) return;
        currentMoveDir = _playerInput.actions.FindAction("Move").ReadValue<Vector2>();
    }

    private void OnBoost()
    {
        if (!enabled) return;
        isBoosting = (_playerInput.actions.FindAction("Boost").ReadValue<float>() == 1);
        if (debugText) debugText.text = $"Boost is currently {isBoosting}";
    }
}
