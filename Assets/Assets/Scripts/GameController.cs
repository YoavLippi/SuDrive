using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private List<TrackedPlayer> playersArr;
    [SerializeField] private GameObject playerPrefab;
    
    [Header("Runtime Variables")]
    [SerializeField] private int deadPlayers;

    [SerializeField] private List<GameObject> playerObjArr;

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
    }

    public void StartRound()
    {
        /*_inputDevices = MetaController.Instance.joinedDevices;
        //PlayerInput player = PlayerInput.Instantiate(playerPrefab, pairWithDevice:_inputDevices[0]);
        if (playersArr.Count > 0)
        {
           foreach (var player in playersArr)
           {
               playersArr.Remove(player);
               Destroy(player.playerObj);
           } 
        }
        
        Debug.Log($"input devices: {_inputDevices.Count}");
        for (int i = 0; i < _inputDevices.Count; i++)
        {
            Debug.Log(_inputDevices[i]);
            PlayerInput player = PlayerInput.Instantiate(playerPrefab, pairWithDevice:_inputDevices[i]);
            TrackedPlayer temp = new TrackedPlayer();
            temp.playerObj = player.gameObject;
            temp.playerController = player.GetComponent<PlayerController>();
            temp.isDead = false;
            temp.score = 0;
            playersArr.Add(temp);
        }*/
        
        //This will update if new players are added
        playerObjArr = new List<GameObject>(MetaController.Instance.joinedPlayers);
        
        for (int i = 0; i < playerObjArr.Count; i++)
        {
            Debug.Log(playerObjArr[i]);
            bool foundFlag = false;
            foreach (var trackedPlayer in playersArr)
            {
                if (trackedPlayer.playerObj == playerObjArr[i])
                {
                    foundFlag = true;
                }
            }
            if (foundFlag) break;
            
            TrackedPlayer temp = new TrackedPlayer();
            playerObjArr[i].GetComponent<CarController>().CurrentState = CarController.CarStates.Actionable;
            temp.playerObj = playerObjArr[i];
            temp.playerController = playerObjArr[i].GetComponent<PlayerController>();
            temp.isDead = false;
            temp.score = 0;
            playersArr.Add(temp);
        }
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
                
                break;
            }
        }
    }
}
