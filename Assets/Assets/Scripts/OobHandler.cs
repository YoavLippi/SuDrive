using System;
using UnityEngine;

public class OobHandler : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
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
            Debug.Log(other.gameObject);
            _gameController.KillPlayer(other.transform.parent.gameObject);
        }
    }
}
