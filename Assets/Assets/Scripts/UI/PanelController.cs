using UnityEngine;
using UnityEngine.InputSystem;

public class PanelController : MonoBehaviour
{
	[SerializeField] private GameObject panelToToggle;
	private bool isPaused = false;
	private bool wasPressedLastFrame = false;

	void Update()
	{
		// Use UnscaledTime or check every frame regardless of Time.timeScale
		if (MetaController.Instance == null) return;

		bool isAnyButtonPressed = false;

		foreach (GameObject player in MetaController.Instance.joinedPlayers)
		{
			if (player == null) continue;
			var playerInput = player.GetComponent<PlayerInput>();

			// Check both potential toggle buttons regardless of current map
			var infoAction = playerInput.actions.FindAction("ToggleInfo");
			//var cancelAction = playerInput.actions.FindAction("Cancel");

			if ((infoAction != null && infoAction.IsPressed()) )
			{
				isAnyButtonPressed = true;
				break;
			}
		}

		// State Check: Trigger only on "Down"
		if (isAnyButtonPressed && !wasPressedLastFrame)
		{
			ToggleMenu();
		}

		wasPressedLastFrame = isAnyButtonPressed;
	}

	void ToggleMenu()
	{
		isPaused = !panelToToggle.activeSelf;
		panelToToggle.SetActive(isPaused);
		Time.timeScale = isPaused ? 0f : 1f;

		foreach (GameObject player in MetaController.Instance.joinedPlayers)
		{
			if (player == null) continue;

			// ALL THE BELOW CODE WAS MADE AND ASSISTED BY AI THROUGH AI DEBUGGING AND TESTING
			// 1. Reset Physics to prevent the "Disappearing" glitch
			Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.linearVelocity = Vector2.zero; // Stops the car from rocket-launching
				rb.angularVelocity = 0f;
				rb.Sleep(); // Briefly puts the physics to sleep to reset the 'math'
				rb.WakeUp();
			}

			// 2. Refresh Input
			PlayerInput pInput = player.GetComponent<PlayerInput>();
			if (pInput != null)
			{
				pInput.SwitchCurrentActionMap(isPaused ? "UI" : "Player");
			}
		}
	}

	void OnQuit(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Debug.Log("Quitting...");
			Application.Quit();

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
	}	
}
