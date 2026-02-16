using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TMP_Text text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrive()
    {
        text.text = $"Drive with val of {_playerInput.actions.FindAction("Drive").ReadValue<float>()}";
    }

    private void OnReverse()
    {
        text.text = $"Reverse with val of {_playerInput.actions.FindAction("Reverse").ReadValue<float>()}";
    }
}
