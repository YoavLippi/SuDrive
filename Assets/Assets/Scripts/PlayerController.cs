using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private CarController _carController;

    [SerializeField] private GameObject[] currentSprites;

    private int spritesIndex = 0;

    [SerializeField] private CarSprites[] carSpriteArr;

    [Serializable]
    public struct CarSprites
    {
        [SerializeField] private Sprite body;
        [SerializeField] private Sprite wheelFr;
        [SerializeField] private Sprite wheelBr;
        [SerializeField] private Sprite wheelBl;
        [SerializeField] private Sprite wheelFl;

        public Sprite Body
        {
            get => body;
            set => body = value;
        }

        public Sprite WheelFr
        {
            get => wheelFr;
            set => wheelFr = value;
        }

        public Sprite WheelBr
        {
            get => wheelBr;
            set => wheelBr = value;
        }

        public Sprite WheelBl
        {
            get => wheelBl;
            set => wheelBl = value;
        }

        public Sprite WheelFl
        {
            get => wheelFl;
            set => wheelFl = value;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _carController = GetComponent<CarController>();
        try
        {
            currentSprites[0].GetComponent<SpriteRenderer>().sprite = carSpriteArr[spritesIndex].Body;
            currentSprites[1].GetComponent<SpriteRenderer>().sprite = carSpriteArr[spritesIndex].WheelFr;
            currentSprites[2].GetComponent<SpriteRenderer>().sprite = carSpriteArr[spritesIndex].WheelBr;
            currentSprites[3].GetComponent<SpriteRenderer>().sprite = carSpriteArr[spritesIndex].WheelBl;
            currentSprites[4].GetComponent<SpriteRenderer>().sprite = carSpriteArr[spritesIndex].WheelFl;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void OnJoin(PlayerInput playerInput)
    {
        spritesIndex = playerInput.playerIndex;
    }

    public void DoDeath()
    {
        //TODO: play a death animation
        
        //Disabling and re-enabling the input should clear all of the inputs
        _playerInput.actions.Disable();
        _playerInput.actions.Enable();
        _carController.CurrentState = CarController.CarStates.Dead;
    }
}
