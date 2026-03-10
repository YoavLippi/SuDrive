using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyZone : MonoBehaviour
{
	public MetaController metaController;
	public string gameSceneName = "Car Testing";
	public float countdownDuration = 3f;
	public TextMeshProUGUI countdownText;

	[Header("Visuals")]
	public Color beginColor = Color.indianRed;
	public Color readyColor = Color.darkSeaGreen;

	//[Header("Audio")]
	//public AudioSource audioSource;
	//public AudioClip beepSound;

	[Header("Animations")]
	[SerializeField] private float jumpSpeed = 5f;
	[SerializeField] private float jumpHeight = 10f;
	private Vector3 initialTextPosition;

	// original colour of start button - EB391E
	private Color originalColor;
	private SpriteRenderer sprite;
	private bool hasPlayedAudio = false;
	private int playersOnPad = 0;
	private float timer = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		sprite = GetComponent<SpriteRenderer>();
		originalColor = sprite.color;

		//if (countdownText != null) countdownText.text = "";
		if (metaController == null)
		{
			metaController = FindFirstObjectByType<MetaController>();
		}

		if (countdownText != null)
		{
			initialTextPosition = countdownText.transform.localPosition;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playersOnPad++;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			playersOnPad--;

			if (playersOnPad < 0) playersOnPad = 0;
						
			ResetCountdown();
		}
	}

	// Update is called once per frame
	void Update()
	{
		int totalJoined = metaController.joinedPlayers.Count;

		if (totalJoined > 1 && playersOnPad == totalJoined)
		{
			countdownText.color = Color.white;
			timer += Time.deltaTime;
			float remainingTime = Mathf.Max(0, countdownDuration - timer);

			if (countdownText != null)
			{
				countdownText.transform.localPosition = initialTextPosition;
				if (remainingTime > 0.1f)
				{
					if (remainingTime > 2)
					{
						sprite.color = beginColor;
						countdownText.text = (remainingTime > 2.5f) ? "GET READY" : "";
					}
					else if (remainingTime > 1)
					{
						sprite.color = Color.orange;
						countdownText.text = (remainingTime > 1.5f) ? "GET READY" : "";
					}
					else
					{
						sprite.color = readyColor;
						countdownText.text = (remainingTime > 0.5f) ? "GET READY" : "";
					}
				}
				else
				{
					countdownText.text = "START!";
				}
			}

			if (timer >= countdownDuration + 0.3f)
			{
				SceneManager.LoadScene(gameSceneName);
			}
		}
		else
		{
			ResetCountdown();

			if (countdownText != null)
			{
				float newY = initialTextPosition.y + (Mathf.Sin(Time.time * jumpSpeed) * jumpHeight);
				countdownText.transform.localPosition = new Vector3(initialTextPosition.x, newY, initialTextPosition.z);
			}
		}
		
	}

	//void PlayBeep()
	//{
	//	if (audioSource != null && beepSound != null)
	//	{
	//		audioSource.PlayOneShot(beepSound);
	//	}
	//}

	void ResetCountdown()
	{
		timer = 0f;
		sprite.color = originalColor;

		if (countdownText != null)
		{
			countdownText.color = Color.black;
			int totalJoined = metaController.joinedPlayers.Count;

			// If at least 2 people have joined, show the fraction
			if (totalJoined > 1)
			{
				countdownText.text = $"WAITING FOR {playersOnPad}/{totalJoined} PLAYERS";
			}
			else
			{
				// If only 1 or 0 people have joined the game session
				countdownText.text = "DRIVE HERE TO START";
			}

			countdownText.transform.localScale = Vector3.one;
		}
	}
}
