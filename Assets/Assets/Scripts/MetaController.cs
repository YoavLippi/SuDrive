using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInputManager))]
public class MetaController : MonoBehaviour
{
	//Singleton behaviour for referencing joined players
	public static MetaController Instance;
	[SerializeField] private PlayerInputManager _manager;
	[SerializeField] private Transform defaultSpawnPointParent;
	public List<GameObject> joinedPlayers = new List<GameObject>();
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += onSceneLoaded;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void onSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Loaded a new scene " + scene.name);
		if(scene.name == "UI testing")
		{
			defaultSpawnPointParent = GameObject.Find("SpawnPointParent").transform;
		}
		
	}


	//Currently fills the place with empty gameObjects that can be tracked.
	private void OnPlayerJoined(PlayerInput playerInput)
	{
		joinedPlayers.Add(playerInput.gameObject);
		Debug.Log($"User number {playerInput.playerIndex} joined");
		if (defaultSpawnPointParent)
		{
			if (defaultSpawnPointParent.GetChild(playerInput.playerIndex))
			{
				playerInput.gameObject.transform.position = defaultSpawnPointParent.GetChild(playerInput.playerIndex).position;
			}
		}
		DontDestroyOnLoad(playerInput);
		playerInput.gameObject.GetComponent<PlayerController>().OnJoin(playerInput);
		Debug.Log($"New device added: {playerInput.GetDevice<InputDevice>()}");
		//playerInput.GetComponent<CarController>().CurrentState = CarController.CarStates.Dead;
	}

	private void OnPlayerLeft(PlayerInput playerInput)
	{
		joinedPlayers.Remove(playerInput.gameObject);
		Destroy(playerInput.gameObject);
		Debug.Log($"Device removed: {playerInput.GetDevice<InputDevice>()}");
	}

	public void ResetForNewGame()
	{
		// Clear the list of tracked player objects
		joinedPlayers.Clear();

		// If you have a 'player count' variable, reset it too
		// playerCount = 0; 

		Debug.Log("MetaController wiped. Ready for new players.");
	}
}
