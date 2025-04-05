using UnityEngine;

public class spawnantbabys : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;     // Assign in Inspector
    public float spawnInterval = 2f;     // Seconds between spawns
    public Vector2 spawnOffset = Vector2.zero;

    [Header("Optional Random Positioning")]
    public bool randomizePosition = false;
    public Vector2 randomRangeMin = new Vector2(-5f, -5f);
    public Vector2 randomRangeMax = new Vector2(5f, 5f);

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;

        if (randomizePosition)
        {
            spawnPos = new Vector2(
                Random.Range(randomRangeMin.x, randomRangeMax.x),
                Random.Range(randomRangeMin.y, randomRangeMax.y)
            );
        }

        Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
    }
}
