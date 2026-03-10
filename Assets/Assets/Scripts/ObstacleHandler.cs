using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    [SerializeField]private GameObject[] obstaclePrefabs;

    [SerializeField] float spawnInterval =2f;
    [SerializeField] float spawnIntervalSpacing = 0.5f;

    [SerializeField] Vector2 spawnAreaPoint = Vector2.zero;
    [SerializeField] Vector2 spawnAreaOuter = new Vector2(5f, 5f);

    [SerializeField] bool spawnOnStart = true;
    [SerializeField] int maxObstaclesSpawned = 10;

    [SerializeField] float _timer;
    [SerializeField] float _nextSpawnTime;

    void Start ()
    {
        if (obstaclePrefabs == null 
            || obstaclePrefabs.Length == 0)
        {
            enabled = false;
            return;
        }

        _nextSpawnTime = spawnOnStart ? 0f : GetNextInterval();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _nextSpawnTime)
        {
            TrySpawn();
            _timer = 0f;
            _nextSpawnTime = GetNextInterval();
        }
    }

    void TrySpawn ()
    {
        if (transform.childCount >= maxObstaclesSpawned) return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        Vector2 spawnPos = spawnAreaPoint + new Vector2(Random.Range(-spawnAreaOuter.x, spawnAreaOuter.x),
            Random.Range(-spawnAreaOuter.y, spawnAreaOuter.y));

        GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawned.transform.parent = transform;

        // spawns object prefabs within a set range and random intervals and distances 
    }

    float GetNextInterval ()
    {
        return spawnInterval + Random.Range(-spawnIntervalSpacing, spawnIntervalSpacing);

    }

    public void StartSpawning() => enabled = true;
    public void StopSpawning() => enabled = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaPoint, spawnAreaOuter * 2f); // just lets me visualise the spawn area
    }
}
