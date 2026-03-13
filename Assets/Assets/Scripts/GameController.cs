using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField] private List<TrackedPlayer> playersArr;
	[SerializeField] private List<Transform> spawnPointParents;
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private int requiredRoundWins;
	[SerializeField] private FractureHandler _fractureHandler;

	[Header("Runtime Variables")]
	[SerializeField] private int deadPlayers;

	[Header("Round Starter")]
	[SerializeField] public TextMeshProUGUI roundStartText;

	[Header("Round Tracking")]
	[SerializeField] private int currentRound = 1;

	[Header("Scoreboard UI")]
	[SerializeField] private List<TMPro.TextMeshProUGUI> playerTallyTexts;

	[Header("End Screen UI Assets")]
	[SerializeField] private List<Sprite> carFrontViews;
	[SerializeField] private List<WinScreenSlot> endScreenSlots;
	public GameObject winPanel;

	[Header("End Screen Timer")]
	[SerializeField] private TextMeshProUGUI timerText; // Drag your UI text here
	[SerializeField] private float restartDelay = 10f;

	[Header("Input")]
	[SerializeField] private InputActionAsset inputActions;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip beepSound;

	[SerializeField] private List<GameObject> playerObjArr;

	[Header("Death Animation")]
	public DeathAnim deathAnim;

	private float countdownDuration = 3f;
	private bool hasPlayedAudio = false;
	public UnityEvent<TrackedPlayer> Win;
	public UnityEvent roundStart;
	public UnityEvent roundEnd;

	[Serializable]
	public struct TrackedPlayer
	{
		public GameObject playerObj;
		public PlayerController playerController;
		public Sprite frontViewImage;
		public int score;
		public bool isDead;
	}

	[Serializable]
	public struct WinScreenSlot
	{
		public GameObject slotParent;   // The "Row" or "Box" in the UI
		public UnityEngine.UI.Image carIcon;      // The Image component for the front-view
		public TMPro.TextMeshProUGUI scoreText;   // The Text component for the points
																							//public TMPro.TextMeshProUGUI position;
	}
		

	void Start()
	{
		deadPlayers = 0;
		hasPlayedAudio = false;
		playersArr = new List<TrackedPlayer>();
		InitializePlayers();
		StartCoroutine(NextRound());
	}

	

	private IEnumerator CountdownRoutine()
	{
		if (roundStartText != null)
		{
			roundStartText.text = "ROUND " + currentRound;
		}

		// Brief pause so players can read the round number
		yield return new WaitForSeconds(1.5f);

		// 2. Play Audio once at the start of the numeric countdown
		if (!hasPlayedAudio)
		{
			if (audioSource != null && beepSound != null)
			{
				audioSource.clip = beepSound;
				audioSource.Play();
			}
			hasPlayedAudio = true;
		}

		// 3. Start Numeric Countdown
		float timer = 0f;
		while (timer < countdownDuration)
		{
			timer += Time.deltaTime;
			float remainingTime = Mathf.Max(0, countdownDuration - timer);

			if (roundStartText != null)
			{
				roundStartText.text = Mathf.CeilToInt(remainingTime).ToString();
			}

			yield return new WaitForEndOfFrame();
		}

		// 4. Start Sequence
		if (roundStartText != null) { roundStartText.text = "START!"; }

		// Execute StartRound logic here to enable players
		//StartRound();

		// 5. Cleanup
		yield return new WaitForSeconds(0.6f);
		if (roundStartText != null) { roundStartText.text = " "; }
	}

	//Messy, but it should work until we need to refactor it much later
	private void InitializePlayers()
	{
		playerObjArr = new List<GameObject>(MetaController.Instance.joinedPlayers);
		deadPlayers = 0;

		for (int i = 0; i < playerObjArr.Count; i++)
		{
			TrackedPlayer temp = new TrackedPlayer();
			playerObjArr[i].GetComponent<CarController>().CurrentState = CarController.CarStates.Dead;
			temp.playerObj = playerObjArr[i];
			temp.playerController = playerObjArr[i].GetComponent<PlayerController>();
			temp.playerObj.GetComponent<CarController>().ResetActions();
			temp.isDead = false;
			temp.score = 0;

			if (i < carFrontViews.Count)
			{
				temp.frontViewImage = carFrontViews[i];
			}

			playersArr.Add(temp);
		}

		int correspondingIndex = playersArr.Count - 1;
		for (int i = 0; i < playersArr.Count; i++)
		{
			playersArr[i].playerObj.transform.position = spawnPointParents[correspondingIndex].GetChild(i).position;
			playersArr[i].playerObj.transform.rotation = spawnPointParents[correspondingIndex].GetChild(i).rotation;
		}

		UpdateScoreboard();
	}

	public void StartRound()
	{
		//This will update if new players are added
		playerObjArr = new List<GameObject>(MetaController.Instance.joinedPlayers);
		deadPlayers = 0;

		for (int i = 0; i < playerObjArr.Count; i++)
		{
			Debug.Log(playerObjArr[i]);
			bool foundFlag = false;
			for (int j = 0; j < playersArr.Count; j++)
			{
				if (playersArr[j].playerObj == playerObjArr[i])
				{
					foundFlag = true;
					Debug.Log("The object is already in the array");
					playersArr[j].playerObj.GetComponent<CarController>().CurrentState = CarController.CarStates.Actionable;
					playersArr[j].playerObj.GetComponent<CarController>().ResetActions();
					playersArr[j].playerController.ClearAnim();

					var tempT = playersArr[j];
					tempT.isDead = false;
					tempT.playerObj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
					tempT.playerObj.GetComponent<Rigidbody2D>().angularVelocity = 0f;
					playersArr[j] = tempT;
					break;
				}
			}

			if (foundFlag) continue;

			TrackedPlayer temp = new TrackedPlayer();
			playerObjArr[i].GetComponent<CarController>().CurrentState = CarController.CarStates.Actionable;
			temp.playerObj = playerObjArr[i];
			temp.playerController = playerObjArr[i].GetComponent<PlayerController>();
			temp.playerController.ClearAnim();
			temp.playerObj.GetComponent<CarController>().ResetActions();

			temp.isDead = false;
			temp.score = 0;
			playersArr.Add(temp);
		}

		int correspondingIndex = playersArr.Count - 1;
		for (int i = 0; i < playersArr.Count; i++)
		{
			playersArr[i].playerObj.transform.position = spawnPointParents[correspondingIndex].GetChild(i).position;
			playersArr[i].playerObj.transform.rotation = spawnPointParents[correspondingIndex].GetChild(i).rotation;
		}
	}

	public bool IsPlayerDead(GameObject inPlayer)
	{
		foreach (var player in playersArr)
		{
			if (player.playerObj == inPlayer) { return player.isDead; }
		}

		return false;
	}

	public void KillPlayer(GameObject player)
	{
		for (int i = 0; i < playersArr.Count; i++)
		{
			//Debug.Log($"Comparing {playersArr[i].playerObj.name} with {player.name}");
			if (playersArr[i].playerObj == player)
			{
				playersArr[i].playerController.DoDeath();
				//There is weirdness with indexers and structs, so we're assigning it to a temp variable first
				var temp = playersArr[i];
				temp.isDead = true;
				playersArr[i] = temp;
				deadPlayers++;
				break;
			}
		}

		if (deadPlayers == playersArr.Count - 1)
		{
			for (int i = 0; i < playersArr.Count; i++)
			{
				if (!playersArr[i].isDead)
				{
					var temp = playersArr[i];
					temp.score++;
					playersArr[i] = temp;

					UpdateScoreboard();

					if (temp.score == requiredRoundWins)
					{
						Win.Invoke(playersArr[i]);
					}
					else
					{
						currentRound++;
						roundEnd.Invoke();
						StartCoroutine(NextRound());
					}

					break;
				}
			}
		}
	}

	private IEnumerator NextRound()
	{
		hasPlayedAudio = false;
		yield return StartCoroutine(CountdownRoutine());
		roundStart.Invoke();
	}

	public void DoWin(TrackedPlayer winner)
	{
		Time.timeScale = 0f;
		winPanel.SetActive(true);

		// 2. Create a copy of the player list to sort (so we don't mess up original indices)
		List<TrackedPlayer> sortedPlayers = new List<TrackedPlayer>(playersArr);

		// 3. Sort: Highest score first (b compared to a)
		sortedPlayers.Sort((a, b) => b.score.CompareTo(a.score));

		// 4. Set the Title for the actual winner
		// We find the winner's index in the ORIGINAL list to know if they were P1, P2, etc.
		int winnerOriginalNum = playersArr.IndexOf(winner) + 1;
		//winnerTitle.text = $"PLAYER {winnerOriginalNum} IS THE CHAMPION!";

		// 5. Populate the slots in ranked order
		for (int i = 0; i < endScreenSlots.Count; i++)
		{
			if (i < sortedPlayers.Count)
			{
				endScreenSlots[i].slotParent.SetActive(true);

				// Display the rank (1st, 2nd, etc.) based on the slot index
				//string rankLabel = (i + 1) switch { 1 => "1st", 2 => "2nd", 3 => "3rd", _ => "4th" };

				// Apply the image and score from the sorted list
				endScreenSlots[i].carIcon.sprite = sortedPlayers[i].frontViewImage;
				endScreenSlots[i].scoreText.text = $"Score: {sortedPlayers[i].score}";

				// Highlight the absolute winner (highest score) in Gold/Yellow
				if (i == 0)
					endScreenSlots[i].scoreText.color = Color.yellow;
				else
					endScreenSlots[i].scoreText.color = Color.white;
			}
			else
			{
				// Hide slots if there are fewer than 4 players
				endScreenSlots[i].slotParent.SetActive(false);
			}
		}

		StartCoroutine(TimedRestartRoutine());
	}

	private IEnumerator TimedRestartRoutine()
	{

		float timeLeft = restartDelay;

		while (timeLeft > 0)
		{
			// Update the UI text
			if (timerText != null)
			{
				// "f0" formats the number to 0 decimal places (e.g., 10, 9, 8...)
				timerText.text = "Returning to menu in: " + timeLeft.ToString("") + "s";
			}

			// Wait for 1 real-world second (ignoring the pause)
			yield return new WaitForSecondsRealtime(1f);

			// Reduce the timer
			timeLeft -= 1f;

			if (timeLeft <= 3f && timerText != null)
			{
				timerText.color = Color.red;
				// You could also trigger a "beep" sound here
			}
		}

		// Ensure it shows 0 before switching
		if (timerText != null) timerText.text = "Returning to menu in: 0s";

		Debug.Log("Countdown finished. Restarting...");
		RestartToStart();
	}

	public void RestartToStart()
	{
		// CRITICAL: Reset time scale or the next scene will be frozen!
		Time.timeScale = 1f;
		foreach (var player in playersArr)
		{
			if (player.playerObj != null) Destroy(player.playerObj);
		}

		// 3. Clear the MetaController list
		if (MetaController.Instance != null)
		{
			// Using the method we created earlier to wipe the joinedPlayers list
			MetaController.Instance.ResetForNewGame();
		}
		SceneManager.LoadScene("UI testing"); // Change to your actual scene name
	}

	public void UpdateScoreboard()
	{
		// Loop through our tracked players and update their specific text
		for (int i = 0; i < playersArr.Count; i++)
		{
			if (i < playerTallyTexts.Count)
			{
				// Set the text to "Player [Number]: [Score]"
				// We use (i + 1) because the list index starts at 0
				playerTallyTexts[i].text = "PL " + (i + 1) + ": " + playersArr[i].score.ToString();

				playerTallyTexts[i].gameObject.SetActive(true);
			}
		}

		// Hide the slots for players who aren't in the match
		for (int i = playersArr.Count; i < playerTallyTexts.Count; i++)
		{
			playerTallyTexts[i].gameObject.SetActive(false);
		}
	}
}
