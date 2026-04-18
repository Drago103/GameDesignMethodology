using UnityEngine;
using System.Collections.Generic;

public class DesertFPSLevelGenerator : MonoBehaviour
{
    [Header("Ground")]
    public GameObject groundPrefab; // Floor
    public int groundSizeX = 300;
    public int groundSizeZ = 300;
    public float groundY = 0f;

    [Header("Path Settings")]
    public float pathWidth = 18f;
    public int pathControlPoints = 6;
    public float pathCurveAmount = 35f;

    [Header("Rock Prefabs")]
    public GameObject[] rockPrefabs; // Rocks
    public int rockCount = 120;
    public float minRockScale = 1.5f;
    public float maxRockScale = 6f;
    public float rockClearDistanceFromPath = 10f;

    [Header("Hill / Dune Prefabs")]
    public GameObject[] hillPrefabs; // Hills
    public int hillCount = 30;
    public float minHillScale = 8f;
    public float maxHillScale = 20f;
    public float hillClearDistanceFromPath = 12f;

    [Header("Decor Prefabs")]
    public GameObject[] decorPrefabs; // Stonnes and small rocks
    public int decorCount = 40;
    public float decorClearDistanceFromPath = 6f;

    [Header("Player / Start / End")]
    public Transform player; // Player
    public Transform startMarker;
    public Transform endMarker;

    [Header("Height Variation")]
    public float noiseScale = 0.015f;
    public float duneHeight = 6f;

    private List<Vector3> pathPoints = new List<Vector3>();

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        ClearChildren();

        GenerateGround();
        GeneratePath();
        PlaceHills();
        PlaceRocks();
        PlaceDecor();
        PositionPlayer();
    }

    void GenerateGround()
    {
        if (groundPrefab == null) return;

        GameObject ground = Instantiate(
            groundPrefab,
            new Vector3(groundSizeX * 0.5f, groundY, groundSizeZ * 0.5f),
            Quaternion.identity,
            transform
        );

        ground.name = "DesertGround";
        ground.transform.localScale = new Vector3(groundSizeX / 10f, 1f, groundSizeZ / 10f);
    }

    void GeneratePath()
    {
        pathPoints.Clear();

        float segmentLength = groundSizeZ / (float)(pathControlPoints - 1);

        for (int i = 0; i < pathControlPoints; i++)
        {
            float z = i * segmentLength;
            float x = (groundSizeX * 0.5f);

            if (i != 0 && i != pathControlPoints - 1)
            {
                x += Random.Range(-pathCurveAmount, pathCurveAmount);
            }

            float y = GetHeightAtPosition(x, z);
            pathPoints.Add(new Vector3(x, y, z));
        }

        if (startMarker != null)
            startMarker.position = pathPoints[0] + Vector3.up * 1f;

        if (endMarker != null)
            endMarker.position = pathPoints[pathPoints.Count - 1] + Vector3.up * 1f;
    }

    void PlaceHills()
    {
        if (hillPrefabs == null || hillPrefabs.Length == 0) return;

        int placed = 0;
        int attempts = 0;

        while (placed < hillCount && attempts < hillCount * 20)
        {
            attempts++;

            float x = Random.Range(0f, groundSizeX);
            float z = Random.Range(0f, groundSizeZ);
            Vector3 pos = new Vector3(x, GetHeightAtPosition(x, z), z);

            if (DistanceToPath(pos) < hillClearDistanceFromPath)
                continue;

            GameObject prefab = hillPrefabs[Random.Range(0, hillPrefabs.Length)];
            GameObject hill = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);

            float scale = Random.Range(minHillScale, maxHillScale);
            hill.transform.localScale = new Vector3(scale, scale, scale);

            placed++;
        }
    }

    void PlaceRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        int placed = 0;
        int attempts = 0;

        while (placed < rockCount && attempts < rockCount * 20)
        {
            attempts++;

            float x = Random.Range(0f, groundSizeX);
            float z = Random.Range(0f, groundSizeZ);
            Vector3 pos = new Vector3(x, GetHeightAtPosition(x, z), z);

            if (DistanceToPath(pos) < rockClearDistanceFromPath)
                continue;

            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
            GameObject rock = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);

            float scale = Random.Range(minRockScale, maxRockScale);
            rock.transform.localScale = new Vector3(scale, scale, scale);

            placed++;
        }
    }

    void PlaceDecor()
    {
        if (decorPrefabs == null || decorPrefabs.Length == 0) return;

        int placed = 0;
        int attempts = 0;

        while (placed < decorCount && attempts < decorCount * 20)
        {
            attempts++;

            float x = Random.Range(0f, groundSizeX);
            float z = Random.Range(0f, groundSizeZ);
            Vector3 pos = new Vector3(x, GetHeightAtPosition(x, z), z);

            if (DistanceToPath(pos) < decorClearDistanceFromPath)
                continue;

            GameObject prefab = decorPrefabs[Random.Range(0, decorPrefabs.Length)];
            GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);

            float scale = Random.Range(0.8f, 2.2f);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            placed++;
        }
    }

    void PositionPlayer()
    {
        if (player == null || pathPoints.Count == 0) return;

        Vector3 startPos = pathPoints[0];
        player.position = startPos + Vector3.up * 2f;

        if (pathPoints.Count > 1)
        {
            Vector3 dir = (pathPoints[1] - pathPoints[0]).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
                player.rotation = Quaternion.LookRotation(dir);
        }
    }

    float GetHeightAtPosition(float x, float z)
    {
        float noise = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
        return groundY + noise * duneHeight;
    }

    float DistanceToPath(Vector3 point)
    {
        if (pathPoints.Count < 2) return float.MaxValue;

        float minDist = float.MaxValue;

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 a = pathPoints[i];
            Vector3 b = pathPoints[i + 1];

            Vector3 closest = ClosestPointOnLineSegment(a, b, point);
            float dist = Vector3.Distance(new Vector3(point.x, 0, point.z), new Vector3(closest.x, 0, closest.z));

            if (dist < minDist)
                minDist = dist;
        }

        return minDist;
    }

    Vector3 ClosestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ap = p - a;
        Vector3 ab = b - a;

        float magnitudeAB = ab.sqrMagnitude;
        float abapProduct = Vector3.Dot(ap, ab);
        float distance = abapProduct / magnitudeAB;

        if (distance < 0) return a;
        if (distance > 1) return b;

        return a + ab * distance;
    }

    void ClearChildren()
    {
        List<GameObject> toDelete = new List<GameObject>();

        foreach (Transform child in transform)
        {
            toDelete.Add(child.gameObject);
        }

        for (int i = 0; i < toDelete.Count; i++)
        {
#if UNITY_EDITOR
            DestroyImmediate(toDelete[i]);
#else
            Destroy(toDelete[i]);
#endif
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pathPoints == null || pathPoints.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
        }

        Gizmos.color = Color.cyan;
        foreach (var p in pathPoints)
        {
            Gizmos.DrawSphere(p, 1.2f);
        }
    }
}