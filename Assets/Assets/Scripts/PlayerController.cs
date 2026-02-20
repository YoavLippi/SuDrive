using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private CarController _carController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoDeath()
    {
        //TODO: play a death animation
        
        //Disabling and re-enabling the input should clear all of the inputs
        _playerInput.actions.Disable();
        _playerInput.actions.Enable();
        
        _carController.CurrentState = CarController.CarStates.Dead;
    }
}
