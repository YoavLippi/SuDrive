using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
	private List<DestructibleTiles> allTiles = new List<DestructibleTiles>();
	public float timeBetweenFalls = 10f;

	void Start()
	{
		// 1. Find every tile with the script attached
		DestructibleTiles[] tileArray = Object.FindObjectsOfType<DestructibleTiles>();

		// 2. Sort them by distance from (0,0,0) - descending (furthest first)
		allTiles = tileArray.OrderByDescending(t => Vector3.Distance(t.transform.position, Vector3.zero)).ToList();

		// 3. Start the repeating timer
		InvokeRepeating("DropNextTile", 5f, timeBetweenFalls);
	}

	void DropNextTile()
	{
		if (allTiles.Count > 0)
		{
			// Pick the furthest tile, trigger it, and remove from the list
			DestructibleTiles tileToDrop = allTiles[0];
			tileToDrop.TriggerFall();
			allTiles.RemoveAt(0);
		}
		else
		{
			CancelInvoke("DropNextTile");
		}
	}
}
