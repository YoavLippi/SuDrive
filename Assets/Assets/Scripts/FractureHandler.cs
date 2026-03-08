using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FractureHandler : MonoBehaviour
{
    [Header("Setup")] 
    [SerializeField] private GameController _gameController;
    [SerializeField] private GameObject[] fractures;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform fracturesParent;
    [SerializeField] private Collider2D arenaCollider;
    [SerializeField] private Transform outerParent;
    [SerializeField] private Transform innerParent;
    
    [Header("Runtime")]
    [SerializeField] private List<Transform> outerRing;
    [SerializeField] private List<Transform> innerRing;
    
    [Header("Fracturing")]
    [SerializeField] private float fractureSpawnDelay;
    [SerializeField] private float startDelay;
    [SerializeField] private bool isFracturing;
    [SerializeField] private Transform spawnPosDebug;

    private IEnumerator fracturing;

    private void Awake()
    {
        fracturing = DoFractures();
    }

    private void Start()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    public void KillOverlappingPlayers()
    {
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("Environment Collision Box");

        int overlapCount = arenaCollider.Overlap(filter, results);

        if (overlapCount > 0)
        {
            // Colliders were found within or overlapping arena
            foreach (Collider2D otherCollider in results)
            {
                if (otherCollider.IsTouching(arenaCollider))
                {
                    GameObject player = otherCollider.transform.parent.gameObject;
                    //Debug.Log(player.name + " is the parent of what overlapped with " + arenaCollider.name);
                    if (!_gameController.IsPlayerDead(player))
                    {
                        Debug.Log("Killed player because of overlap");
                        _gameController.KillPlayer(player);
                    }
                }
            }
        }
    }

    public void DoNewRound()
    {
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        StopFracturing();
        ClearArenaFractures();
        yield return new WaitForSeconds(startDelay);
        StartFracturing();
    }

    public void StopFracturing()
    {
        isFracturing = false;
        StopCoroutine(fracturing);
    }

    public void ClearArenaFractures()
    {
        foreach(var child in fracturesParent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != fracturesParent.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StartFracturing()
    {
        outerRing.Clear();
        innerRing.Clear();
        //populating rings
        foreach(var child in outerParent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != outerParent.gameObject)
            {
                outerRing.Add(child.transform);
            }
        }
        
        foreach(var child in innerParent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != innerParent.gameObject)
            {
                innerRing.Add(child.transform);
            }
        }
        
        isFracturing = true;
        StartCoroutine(fracturing);
    }

    private Quaternion AlignToPosition(Vector2 initial, Transform target)
    {
        //Aligning x to a target
        Vector2 direction = target.position - (Vector3) initial;
        Quaternion targetRot = Quaternion.FromToRotation(Vector3.right, direction);
        return targetRot;
    }

    private Transform NextLocation()
    {
        Transform output = transform;
        
        if (outerRing.Count > 0)
        {
            int ringIndex = Random.Range(0, outerRing.Count);
            output = outerRing[ringIndex];
            outerRing.Remove(outerRing[ringIndex]);
        } else if (innerRing.Count > 0)
        {
            int ringIndex = Random.Range(0, innerRing.Count);
            output = innerRing[ringIndex];
            innerRing.Remove(innerRing[ringIndex]);
        }

        return output;
    }

    private IEnumerator DoFractures()
    {
        while (isFracturing)
        {
            //Getting correct position and location
            Vector2 initialPos = NextLocation().position;
            Quaternion rot = AlignToPosition(initialPos, lookTarget);
            
            //spawning fractures
            int fractureIndex = Random.Range(0, 9);
            GameObject newFracture = Instantiate(fractures[fractureIndex], initialPos, rot, fracturesParent);
            float scaleRandomised = Random.Range(0.045f, 0.06f);
            newFracture.transform.localScale = new Vector3(scaleRandomised,scaleRandomised,scaleRandomised);
            
            //need to do flashing here first?
            float flashTime = 0.2f;
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(flashTime);
                newFracture.SetActive(false);
                yield return new WaitForSeconds(flashTime);
                newFracture.SetActive(true);
            }
            newFracture.GetComponent<IndividualFractureHandler>().SetCollidersActive(true);
            
            //Debug.Log("spawn");
            yield return new WaitForFixedUpdate();
            KillOverlappingPlayers();
            yield return new WaitForSeconds(fractureSpawnDelay);
        }
    }
}
