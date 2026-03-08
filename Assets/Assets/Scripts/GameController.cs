using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

	[SerializeField] private List<GameObject> playerObjArr;
    [Header("Death Animation")]
    public DeathAnim deathAnim;

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
		playersArr = new List<TrackedPlayer>();
		deadPlayers = 0;
		roundStart.Invoke();
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
					playersArr[j].playerObj
							.GetComponent<CarController>().CurrentState = CarController.CarStates.Actionable;

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
			temp.isDead = false;
			temp.score = 0;
			playersArr.Add(temp);
		}

		//starting round
		//TODO: Countdown
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
			if (player.playerObj == inPlayer)
			{
				return player.isDead;
			}
		}

		return false;
	}

	public void KillPlayer(GameObject player)
	{
		for (int i = 0; i < playersArr.Count; i++)
		{
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
		yield return new WaitForSeconds(3f);
		roundStart.Invoke();
	}

	public void DoWin(TrackedPlayer winner)
	{
		Debug.Log($"{winner.playerObj.name} is the winner");
		//TODO: boot to main menu or go to win scene
	}
}
