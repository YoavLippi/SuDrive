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
    [SerializeField] private List<Transform> outerRing;
    [SerializeField] private List<Transform> innerRing;
    
    [Header("Fracturing")]
    [SerializeField] private float fractureSpawnDelay;
    [SerializeField] private float startDelay;
    [SerializeField] private bool isFracturing;
    [SerializeField] private Transform spawnPosDebug;

    private void Start()
    {
        isFracturing = true;
        StartCoroutine(DoFractures());
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
        yield return new WaitForSeconds(startDelay);
        while (isFracturing)
        {
            yield return new WaitForSeconds(fractureSpawnDelay);
            Vector2 initialPos = NextLocation().position;
            Quaternion rot = AlignToPosition(initialPos, lookTarget);
            int fractureIndex = Random.Range(0, 9);
            GameObject tester = Instantiate(fractures[fractureIndex], initialPos, rot, arenaParent);
            float scaleRandomised = Random.Range(0.045f, 0.06f);
            tester.transform.localScale = new Vector3(scaleRandomised,scaleRandomised,scaleRandomised);
            Debug.Log("spawn");
        }
    }
}
