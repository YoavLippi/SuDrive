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
	public Color readyColor = Color.darkSeaGreen;

	// original colour of start button - EB391E
	private Color originalColor;
	private SpriteRenderer sprite;

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
			metaController = FindObjectOfType<MetaController>();
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

			if (playersOnPad <= 0)
			{
				playersOnPad = 0;
				timer = 0f;
				sprite.color = originalColor;
				if (countdownText != null) countdownText.text = "";
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		int totalJoined = metaController.joinedPlayers.Count;

		if (totalJoined > 1 && playersOnPad == totalJoined)
		{
			timer += Time.deltaTime;

			float remainingTime = Mathf.Max(0, countdownDuration - timer);
			if (countdownText != null)
			{
				countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

				// The "Haptic" Pulse
				float pulse = 1f + (Mathf.PingPong(timer * 2, 0.15f));
				countdownText.transform.localScale = new Vector3(pulse, pulse, 1);
			}

			sprite.color = Color.Lerp(originalColor, readyColor, timer / countdownDuration);

			if (timer >= countdownDuration)
			{
				SceneManager.LoadScene(gameSceneName);
			}
		}
		else
		{
			ResetCountdown();
		}
	}

	void ResetCountdown()
	{
		timer = 0f;
		sprite.color = originalColor;
		if (countdownText != null)
		{
			int totalJoined = metaController.joinedPlayers.Count;
			if (totalJoined > 1 && playersOnPad < totalJoined)
				countdownText.text = "WAITING FOR OTHERS...";
			else
				countdownText.text = "DRIVE HERE TO START";
		}
	}
}
