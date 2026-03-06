using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FractureHandler : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject[] fractures;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Transform arenaParent;
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
        foreach(var child in arenaParent.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != arenaParent.gameObject)
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
            Vector2 initialPos = NextLocation().position;
            Quaternion rot = AlignToPosition(initialPos, lookTarget);
            int fractureIndex = Random.Range(0, 9);
            GameObject tester = Instantiate(fractures[fractureIndex], initialPos, rot, arenaParent);
            float scaleRandomised = Random.Range(0.045f, 0.06f);
            tester.transform.localScale = new Vector3(scaleRandomised,scaleRandomised,scaleRandomised);
            Debug.Log("spawn");
            yield return new WaitForSeconds(fractureSpawnDelay);
        }
    }
}
