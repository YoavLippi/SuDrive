using UnityEngine;

public class KillboxHandler : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    
    void Start()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("This was the killbox");
            if (!_gameController.IsPlayerDead(other.transform.parent.gameObject))
            {
                Debug.LogWarning("Killed player");
                _gameController.KillPlayer(other.transform.parent.gameObject);
            }
        }
    }
}
