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
			var cancelAction = playerInput.actions.FindAction("Cancel");

			if ((infoAction != null && infoAction.IsPressed()) ||
					(cancelAction != null && cancelAction.IsPressed()))
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
		isPaused = !isPaused;
		panelToToggle.SetActive(isPaused);

		// 1. Reset Time Scale
		Time.timeScale = isPaused ? 0f : 1f;

		// 2. Switch Maps for EVERYONE
		string targetMap = isPaused ? "UI" : "Gameplay";
		foreach (GameObject player in MetaController.Instance.joinedPlayers)
		{
			if (player == null) continue;
			player.GetComponent<PlayerInput>().SwitchCurrentActionMap(targetMap);
		}

		Debug.Log($"Switching to {targetMap}. Game Paused: {isPaused}");
	}
}
