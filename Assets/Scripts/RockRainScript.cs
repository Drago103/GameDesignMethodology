using UnityEngine;

public class RockRainSpawner : MonoBehaviour
{
    [Header("Rock Prefabs")]
    public GameObject[] rockPrefabs;

    [Header("Spawn Area")]
    public Vector3 areaCenter = new Vector3(200f, 80f, 100f);
    public Vector3 areaSize = new Vector3(200f, 20f, 120f);

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public int rocksPerWave = 2;

    [Header("Lifetime")]
    public float destroyAfterSeconds = 15f;

    [Header("Initial Force")]
    public float extraDownwardForce = 0f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        for (int i = 0; i < rocksPerWave; i++)
        {
            Vector3 spawnPos = GetRandomPointInArea();

            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            GameObject rock = Instantiate(
                prefab,
                spawnPos,
                Random.rotation,
                null
            );

            // Add RockHazard script automatically if missing
            if (rock.GetComponent<RockHazard>() == null)
            {
                rock.AddComponent<RockHazard>();
            }

            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb != null && extraDownwardForce > 0f)
            {
                rb.AddForce(Vector3.down * extraDownwardForce, ForceMode.Impulse);
            }

            Destroy(rock, destroyAfterSeconds);
        }
    }

    Vector3 GetRandomPointInArea()
    {
        float x = Random.Range(
            areaCenter.x - areaSize.x * 0.5f,
            areaCenter.x + areaSize.x * 0.5f
        );

        float y = Random.Range(
            areaCenter.y - areaSize.y * 0.5f,
            areaCenter.y + areaSize.y * 0.5f
        );

        float z = Random.Range(
            areaCenter.z - areaSize.z * 0.5f,
            areaCenter.z + areaSize.z * 0.5f
        );

        return new Vector3(x, y, z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}