using System;
using UnityEngine;

public class OobHandler : MonoBehaviour
{
	[SerializeField] private GameController _gameController;

	[SerializeField] private float bounciness;
	[SerializeField] private float requiredSpeed;
	[SerializeField] private float requiredRotationSpeed;
	[SerializeField] private float minimumBounceForce;

	[SerializeField] private ParticleSystem bounceParticles;
	[SerializeField] private ParticleSystem breakParticles;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Rigidbody2D rbOther = other.transform.parent.gameObject.GetComponent<Rigidbody2D>();
			Collider2D myCol = GetComponent<Collider2D>();
			//getting contact point because this is a trigger
			Vector2 contactPoint = myCol.ClosestPoint(other.transform.position);

			//using empty gameobject to automatically align the particles
			GameObject temp = new GameObject();
			temp.transform.position = contactPoint;
			temp.transform.LookAt(other.transform.position);
			Quaternion rot = temp.transform.rotation;
			Destroy(temp);

			if (rbOther.linearVelocity.magnitude < requiredSpeed && Mathf.Abs(rbOther.angularVelocity) < requiredRotationSpeed)
			{
				Debug.Log(other.gameObject);

				rbOther.AddForce(Mathf.Max((rbOther.linearVelocity * bounciness / rbOther.mass).magnitude, minimumBounceForce) * -rbOther.linearVelocity.normalized, ForceMode2D.Impulse);
				Instantiate(bounceParticles, contactPoint, rot);
			}
			else
			{
				Instantiate(breakParticles, contactPoint, rot);
				_gameController.KillPlayer(other.transform.parent.gameObject);
			}
		}
	}
}
