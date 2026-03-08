using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private PlayerInput _playerInput;

	[SerializeField] private CarController _carController;

	[SerializeField] private GameObject[] currentSprites;

	private int playerIndex = 0;

	[SerializeField] private CarSprites[] carSpriteArr;

	[SerializeField] private Gradient[] trailColorArray;

	[SerializeField] private DeathAnim _deathAnim;

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
			currentSprites[0].GetComponent<SpriteRenderer>().sprite = carSpriteArr[playerIndex].Body;
			currentSprites[1].GetComponent<SpriteRenderer>().sprite = carSpriteArr[playerIndex].WheelFr;
			currentSprites[2].GetComponent<SpriteRenderer>().sprite = carSpriteArr[playerIndex].WheelBr;
			currentSprites[3].GetComponent<SpriteRenderer>().sprite = carSpriteArr[playerIndex].WheelBl;
			currentSprites[4].GetComponent<SpriteRenderer>().sprite = carSpriteArr[playerIndex].WheelFl;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		foreach (var wheel in _carController.allWheels)
		{
			wheel.SetGradient(trailColorArray[playerIndex]);
		}
	}

	public void OnJoin(PlayerInput playerInput)
	{
		playerIndex = playerInput.playerIndex;
	}

	public void ClearAnim()
	{
		_deathAnim.ResetDeathAnim();
	}

	public IEnumerator ClearActions()
	{
		_playerInput.actions.Disable();
		//Disabling and re-enabling the input should clear all of the inputs
		yield return new WaitForEndOfFrame();
		_playerInput.actions.Enable();
	}

	public void DoDeath()
	{
		//TODO: play a death animation
		_deathAnim.EffectiveDeath();

		StartCoroutine(ClearActions());
		_carController.CurrentState = CarController.CarStates.Dead;
	}
}
