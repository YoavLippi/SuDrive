using UnityEngine;
using UnityEngine.InputSystem;

public class PanelController : MonoBehaviour
{
	[SerializeField] private GameObject panelToDisable;
	[SerializeField] private GameObject panelToToggle;
	[SerializeField] private PlayerInput playerInput;

	//void Start()
	//{
	//	// Ensure we start in driving mode so the "I" key works!
	//	playerInput.SwitchCurrentActionMap("Gameplay");

	//	// Safety: make sure the panel is actually off at the start
	//	if (panelToToggle != null) panelToToggle.SetActive(false);
	//}
	private void OnEnable()
	{
		// Bind to the actions defined in your Input Action Asset
		playerInput.actions["Cancel"].performed += OnTogglePressed;     // B / East
		playerInput.actions["ToggleInfo"].performed += OnTogglePressed; // I / North
	}

	private void OnDisable()
	{
		playerInput.actions["Cancel"].performed -= OnTogglePressed;
		playerInput.actions["ToggleInfo"].performed -= OnTogglePressed;
	}

	private void OnTogglePressed(InputAction.CallbackContext context)
	{
		if (panelToToggle != null)
		{
			// 1. Toggle the panel visibility
			bool turningOn = !panelToToggle.activeSelf;
			panelToToggle.SetActive(turningOn);

			// 2. Control Time and Input Maps
			if (turningOn)
			{
				PauseGame();
			}
			else
			{
				ResumeGame();
			}
		}
	}

	private void PauseGame()
	{
		// Stops Physics, Timers (DeltaTime), and Arena Crumbling
		Time.timeScale = 0f;

		// Switch to UI map so only 'Cancel' or 'Navigate' work
		playerInput.SwitchCurrentActionMap("UI");

		Debug.Log("Game Paused");
	}

	private void ResumeGame()
	{
		// Restores everything to normal speed
		Time.timeScale = 1f;

		// Switch back to driving mode
		playerInput.SwitchCurrentActionMap("Gameplay");

		Debug.Log("Game Resumed");
	}
}
