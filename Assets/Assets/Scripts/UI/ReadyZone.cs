using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyZone : MonoBehaviour
{
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

		// Hide the text at the start
		if (countdownText != null) countdownText.text = "";
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
		if(playersOnPad > 0)
		{
			timer += Time.deltaTime;
			float remainingTime = Mathf.Max(0, countdownDuration - timer);			
			if (countdownText != null)
			{
				countdownText.text = Mathf.CeilToInt(remainingTime).ToString();
				float scale = 1f + (Mathf.PingPong(timer * 2, 0.2f));               // These two lines bounce the count down text to give it an arcade feel.
				countdownText.transform.localScale = new Vector3(scale, scale, 1);	// sourced from: 
			}

			sprite.color = Color.Lerp(originalColor, readyColor, timer/countdownDuration);	

			if(timer >= countdownDuration)
			{
				if (countdownText != null) countdownText.text = "START!";
				SceneManager.LoadScene(gameSceneName);
			}
		}
	}
}
