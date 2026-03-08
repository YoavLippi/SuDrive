using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
		public int score;
		public bool isDead;
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
		if (!hasPlayedAudio)
		{
			if (audioSource != null && beepSound != null)
			{
				audioSource.clip = beepSound;
				audioSource.Play();
			}

			hasPlayedAudio = true;
		}

		float timer = 0f;
		//Countdown
		bool showCountDown = true;
		while (showCountDown)
		{
			timer += Time.deltaTime;
			float remainingTime = Mathf.Max(0, countdownDuration - timer);

			// Update the text with the remaining seconds
			if (roundStartText != null) { roundStartText.text = Mathf.CeilToInt(remainingTime).ToString(); }

			// Wait for exactly one frame before continuing the loop
			yield return new WaitForEndOfFrame();

			showCountDown = remainingTime > 0;
		}

		// After the loop finishes...
		if (roundStartText != null) { roundStartText.text = "START!"; }

		//onFinished?.Invoke();

		// After the loop finishes...
		if (roundStartText != null)
		{
			// Delay for 0.6 seconds safely!
			yield return new WaitForSeconds(0.6f);
			roundStartText.text = " ";
		}
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
			playersArr.Add(temp);
		}
		
		int correspondingIndex = playersArr.Count - 1;
		for (int i = 0; i < playersArr.Count; i++)
		{
			playersArr[i].playerObj.transform.position = spawnPointParents[correspondingIndex].GetChild(i).position;
			playersArr[i].playerObj.transform.rotation = spawnPointParents[correspondingIndex].GetChild(i).rotation;
		}
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

					if (temp.score == requiredRoundWins)
					{
						Win.Invoke(playersArr[i]);
					}
					else
					{
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
		Debug.Log($"{winner.playerObj.name} is the winner");
		//TODO: boot to main menu or go to win scene
	}
}
