using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuitZone : MonoBehaviour
{
	[Header("UI Reference")]
	[SerializeField] private TextMeshProUGUI statusText;

	private List<GameObject> carsInZone = new List<GameObject>();

	void Start()
	{
		UpdateStatusUI(); // Show the initial count (e.g., 0 / 2)
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player") && !carsInZone.Contains(other.gameObject))
		{
			carsInZone.Add(other.gameObject);
			UpdateStatusUI();
			CheckForFullGroup();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (carsInZone.Contains(other.gameObject))
		{
			carsInZone.Remove(other.gameObject);
			UpdateStatusUI();
		}
	}

	private void UpdateStatusUI()
	{
		int aliveCount = GetActivePlayerCount();

		if (statusText != null)
		{
			// Now it will correctly say "0 / 2" 
			statusText.text = $"{carsInZone.Count} / {aliveCount} Players Ready";
			//statusText.color = (carsInZone.Count >= aliveCount && aliveCount > 0) ? Color.green : Color.white;
		}
	}

	private void CheckForFullGroup()
	{
		int aliveCount = GetActivePlayerCount();

		if (aliveCount > 0 && carsInZone.Count >= aliveCount)
		{
			ExecuteHardQuit();
		}
	}

	// Helper function to keep the code clean
	private int GetActivePlayerCount()
	{
		
			// If you are using the MetaController to track joined players:
			if (MetaController.Instance != null)
			{
				// This only counts players who actually joined the match
				return MetaController.Instance.joinedPlayers.Count;
			}

			// Fallback just in case
			return GameObject.FindGameObjectsWithTag("Player").Length;
		
	}

	private void ExecuteHardQuit()
	{
		Debug.Log("Game Exiting...");
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}