using UnityEngine;
using System.Collections.Generic;

public class HandcraftedDesertMapGenerator : MonoBehaviour
{
    [Header("Floor")]
    public GameObject sandTilePrefab;
    public int floorTilesX = 46;
    public int floorTilesZ = 22;
    public float tileSize = 10f;
    public float floorY = 0f;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] decorationPrefabs;

    [Header("Wall Shape")]
    public float wallHeight = 10f;
    public float wallThickness = 1f;
    public float wallBaseY = 0f;

    [Header("Object Heights")]
    public float treeY = 0f;
    public float rockY = 0f;
    public float decorationY = 0f;

    [Header("Player")]
    public Transform player;
    public Vector3 playerStartPosition = new Vector3(10f, 2f, 102f);

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        ClearChildren();
        GenerateFloor();
        GenerateWalls();
        GenerateTrees();
        GenerateRocks();
        GenerateDecorations();
        PositionPlayer();
    }

    void GenerateFloor()
    {
        if (sandTilePrefab == null) return;

        for (int x = 0; x < floorTilesX; x++)
        {
            for (int z = 0; z < floorTilesZ; z++)
            {
                Vector3 pos = new Vector3(x * tileSize, floorY, z * tileSize);

                GameObject tile = Instantiate(sandTilePrefab, pos, Quaternion.identity, transform);
                tile.name = $"SandTile_{x}_{z}";
                tile.transform.localScale = Vector3.one;
            }
        }
    }

    void GenerateWalls()
{
    if (wallPrefab == null) return;

    // ===== START CORRIDOR (two parallel lines) =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(0,0,115),
        new Vector3(90,0,115)
    });

    CreateWallChain(new Vector3[]
    {
        new Vector3(0,0,90),
        new Vector3(85,0,90),
        new Vector3(110,0,75)
    });

    // ===== OUTER TOP WALL (zig-zag) =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(90,0,115),
        new Vector3(135,0,130),
        new Vector3(175,0,110),
        new Vector3(220,0,118),
        new Vector3(265,0,105),
        new Vector3(310,0,90),
        new Vector3(300,0,140),
        new Vector3(470,0,90)
    });

    // ===== OUTER BOTTOM WALL =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(110,0,75),
        new Vector3(135,0,10),
        new Vector3(200,0,0),
        new Vector3(260,0,15),
        new Vector3(300,0,30),
        new Vector3(360,0,10),
        new Vector3(430,0,25)
    });

    // ===== INNER TOP CORRIDOR =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(170,0,85),
        new Vector3(215,0,95),
        new Vector3(260,0,80),
        new Vector3(255,0,60)
    });

    // ===== INNER LOWER CORRIDOR =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(160,0,55),
        new Vector3(205,0,65),
        new Vector3(255,0,55),
        new Vector3(330,0,35)
    });

    // ===== LEFT DIAGONAL WALL =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(120,0,80),
        new Vector3(150,0,20),
        new Vector3(200,0,10)
    });

    // ===== RIGHT TRIANGLE FUNNEL =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(320,0,80),
        new Vector3(430,0,50),
        new Vector3(320,0,35),
        new Vector3(340,0,55)
    });

    // ===== RIGHT INNER LINE =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(300,0,50),
        new Vector3(380,0,50)
    });

    // ===== FINAL END CORRIDOR =====
    CreateWallChain(new Vector3[]
    {
        new Vector3(430,0,25),
        new Vector3(480,0,40)
    });

    CreateWallChain(new Vector3[]
    {
        new Vector3(470,0,90),
        new Vector3(480,0,60)
    });
}

    void GenerateTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        Vector3[] treePositions = new Vector3[]
        {
            new Vector3(115, treeY, 106),
            new Vector3(135, treeY, 92),
            new Vector3(138, treeY, 88),
            new Vector3(162, treeY, 62),
            new Vector3(272, treeY, 48),
            new Vector3(306, treeY, 100),
            new Vector3(300, treeY, 10),
            new Vector3(382, treeY, 76),
            new Vector3(402, treeY, 18),
            new Vector3(360, treeY, -2),
            new Vector3(208, treeY, 0),
            new Vector3(160, treeY, -10)
        };

        for (int i = 0; i < treePositions.Length; i++)
        {
            GameObject prefab = treePrefabs[i % treePrefabs.Length];
            GameObject obj = Instantiate(prefab, treePositions[i], Quaternion.Euler(0f, i * 23f, 0f), transform);
            obj.transform.localScale = Vector3.one * 1.3f;
        }
    }

    void GenerateRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        Vector3[] rockPositions = new Vector3[]
        {
            new Vector3(152, rockY, 90),
            new Vector3(228, rockY, 38),
            new Vector3(185, rockY, -8),
            new Vector3(318, rockY, 42)
        };

        for (int i = 0; i < rockPositions.Length; i++)
        {
            GameObject prefab = rockPrefabs[i % rockPrefabs.Length];
            GameObject obj = Instantiate(prefab, rockPositions[i], Quaternion.Euler(0f, i * 31f, 0f), transform);
            obj.transform.localScale = Vector3.one * 1.6f;
        }
    }

    void GenerateDecorations()
    {
        if (decorationPrefabs == null || decorationPrefabs.Length == 0) return;

        Vector3[] decorationPositions = new Vector3[]
        {
            new Vector3(110, decorationY, 117),
            new Vector3(210, decorationY, 56),
            new Vector3(300, decorationY, 58),
            new Vector3(425, decorationY, 20)
        };

        for (int i = 0; i < decorationPositions.Length; i++)
        {
            GameObject prefab = decorationPrefabs[i % decorationPrefabs.Length];
            Instantiate(prefab, decorationPositions[i], Quaternion.Euler(0f, i * 18f, 0f), transform);
        }
    }

    void CreateWallChain(Vector3[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            CreateWallSegment(points[i], points[i + 1]);
        }
    }

    void CreateWallSegment(Vector3 start, Vector3 end)
    {
        Vector3 flatDir = end - start;
        flatDir.y = 0f;

        float length = flatDir.magnitude;
        if (length < 0.01f) return;

        Vector3 midpoint = (start + end) * 0.5f;
        midpoint.y = wallBaseY + wallHeight * 0.5f;

        float angleY = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, angleY, 0f);

        GameObject wall = Instantiate(wallPrefab, midpoint, rotation, transform);

        wall.transform.position = midpoint;
        wall.transform.rotation = rotation;
        wall.transform.localScale = new Vector3(wallThickness, wallHeight, length);
    }

    void PositionPlayer()
    {
        if (player == null) return;

        player.position = playerStartPosition;
        player.rotation = Quaternion.identity;
    }

    void ClearChildren()
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        for (int i = 0; i < children.Count; i++)
        {
#if UNITY_EDITOR
            DestroyImmediate(children[i]);
#else
            Destroy(children[i]);
#endif
        }
    }
}