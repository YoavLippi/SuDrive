using UnityEngine;
using System.Collections.Generic;
using System.Collections; 
using TMPro;

public class QuitZone : MonoBehaviour
{
	[Header("UI Reference")]
	[SerializeField] private TextMeshProUGUI statusText;

	private List<GameObject> carsInZone = new List<GameObject>();
	private Coroutine quitCoroutine; // To keep track of the countdown

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

			// If someone leaves, STOP the countdown
			if (quitCoroutine != null)
			{
				StopCoroutine(quitCoroutine);
				quitCoroutine = null;
			}

			UpdateStatusUI();
		}
	}

	private void UpdateStatusUI()
	{
		int aliveCount = GetActivePlayerCount();

		if (statusText != null && quitCoroutine == null) // Don't overwrite the "Quitting in..." text
		{
			statusText.text = $"{carsInZone.Count} / {aliveCount} Players Ready";
			statusText.color = (carsInZone.Count >= aliveCount && aliveCount > 0) ? Color.green : Color.black;
		}
	}

	private void CheckForFullGroup()
	{
		int aliveCount = GetActivePlayerCount();

		// If everyone is in and we aren't already counting down
		if (aliveCount > 0 && carsInZone.Count >= aliveCount && quitCoroutine == null)
		{
			quitCoroutine = StartCoroutine(QuitCountdownRoutine());
		}
	}

	private IEnumerator QuitCountdownRoutine()
	{
		float timer = 3f;

		while (timer > 0)
		{
			if (statusText != null)
			{
				//Quitting in 3... 2... 1...
				statusText.text = $"Quitting in {Mathf.Ceil(timer)}...";
				statusText.color = Color.black;
			}

			yield return new WaitForSeconds(1f);
			timer -= 1f;
		}

		ExecuteHardQuit();
	}

	private int GetActivePlayerCount()
	{
		if (MetaController.Instance != null)
			return MetaController.Instance.joinedPlayers.Count;

		return GameObject.FindGameObjectsWithTag("Player").Length;
	}

	private void ExecuteHardQuit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}