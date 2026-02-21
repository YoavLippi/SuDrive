using System.Collections;
using UnityEngine;

public class DestructibleTiles : MonoBehaviour
{
	public float fallDelay = 3f;
	private bool isFalling = false;
	private Color originalColor;
	private SpriteRenderer sprite;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		originalColor = sprite.color;

	}
	void Start()
	{
		StopAllCoroutines();
	}

	public void TriggerFall()
	{
		if(!isFalling) { StartCoroutine(FallSequence()); }
	}

	IEnumerator FallSequence()
	{
		isFalling = true;
		int flashCount = 3;

		for(int i = 0; i < flashCount; i++)
		{
			//AudioSource.PlayClipAtPoint(beepSound, transform.position);
			sprite.color = Color.red;
			yield return new WaitForSeconds(0.5f);
			sprite.color = originalColor;
			yield return new WaitForSeconds(0.5f);
		}

		GetComponent<Collider2D>().enabled = false;

		while(transform.localScale.x > 0.01f)
		{
			transform.localScale -= Vector3.one * Time.deltaTime *3;
			yield return null;
		}

		Destroy(gameObject);
	}
}
