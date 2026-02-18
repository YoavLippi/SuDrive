using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInputManager))]
public class MetaController : MonoBehaviour
{
    //Singleton behaviour for referencing joined players
    public static MetaController Instance;
    [SerializeField] private PlayerInputManager _manager;
    public List<GameObject> joinedPlayers = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Currently fills the place with empty gameObjects that can be tracked.
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        joinedPlayers.Add(playerInput.gameObject);
        DontDestroyOnLoad(playerInput);
        Debug.Log($"New device added: {playerInput.GetDevice<InputDevice>()}");
        playerInput.GetComponent<CarController>().CurrentState = CarController.CarStates.Dead;
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        joinedPlayers.Remove(playerInput.gameObject);
        Destroy(playerInput.gameObject);
        Debug.Log($"Device removed: {playerInput.GetDevice<InputDevice>()}");
    }
}
