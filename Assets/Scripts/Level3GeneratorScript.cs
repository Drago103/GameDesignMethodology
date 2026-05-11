using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandcraftedDesertMapGenerator : MonoBehaviour
{
    [Header("Level Scale")]
    public float levelScale = 2.5f;

    [Header("Floor")]
    public GameObject sandTilePrefab;
    public int floorTilesX = 46;
    public int floorTilesZ = 22;
    public float tileSize = 10f;
    public float floorY = -0.1f;

    [Header("Floor Texture")]
    public Vector2 sandTextureTiling = new Vector2(6f, 6f);

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
    public Vector3 playerStartPosition = new Vector3(10f, 0.5f, 259f);
    public Vector3 playerStartRotation = new Vector3(0f, 90f, 0f);

    [Header("Level End")]
    public GameObject levelEndPrefab;
    public Vector3 levelEndPosition = new Vector3(480f, 3f, 50f);
    public Vector3 levelEndRotation = new Vector3(0f, 0f, 0f);
    public Vector3 levelEndScale = new Vector3(3f, 3f, 3f);
    public string nextLevelSceneName = "Level 4";
    
    void Start()
    {
        GenerateMap();

        // Force spawn again after other scripts and physics initialise.
        StartCoroutine(ForcePlayerSpawnNextFrame());
    }

    IEnumerator ForcePlayerSpawnNextFrame()
    {
        yield return null;
        PositionPlayer();

        yield return new WaitForFixedUpdate();
        PositionPlayer();
    }

    public void GenerateMap()
    {
        ClearChildren();

        // GenerateFloor();
        GenerateWalls();
        GenerateTrees();
        GenerateRocks();
        GenerateDecorations();

        // Initial spawn attempt.
        PositionPlayer();
    }

    // void GenerateFloor()
    // {
    //     if (sandTilePrefab == null) return;
    //
    //     for (int x = 0; x < floorTilesX; x++)
    //     {
    //         for (int z = 0; z < floorTilesZ; z++)
    //         {
    //             Vector3 pos = new Vector3(x * tileSize, floorY, z * tileSize);
    //
    //             GameObject tile = Instantiate(
    //                 sandTilePrefab,
    //                 pos,
    //                 Quaternion.identity,
    //                 transform
    //             );
    //
    //             tile.name = $"SandTile_{x}_{z}";
    //             tile.transform.localScale = new Vector3(tileSize, 0.2f, tileSize);
    //
    //             Renderer rend = tile.GetComponent<Renderer>();
    //             if (rend != null)
    //             {
    //                 Material mat = rend.material;
    //
    //                 if (mat.HasProperty("_BaseMap"))
    //                     mat.SetTextureScale("_BaseMap", sandTextureTiling);
    //
    //                 if (mat.HasProperty("_MainTex"))
    //                     mat.SetTextureScale("_MainTex", sandTextureTiling);
    //             }
    //         }
    //     }
    // }

    void GenerateWalls()
    {
        if (wallPrefab == null) return;
        
        //Start Back Wall
        CreateWallChain(new Vector3[]
        {
            new Vector3(0, 0, 90),
            new Vector3(0, 0, 115)
        });

        // START CORRIDOR
        CreateWallChain(new Vector3[]
        {
            new Vector3(0, 0, 115),
            new Vector3(90, 0, 115)
        });

        CreateWallChain(new Vector3[]
        {
            new Vector3(0, 0, 90),
            new Vector3(85, 0, 90),
            new Vector3(110, 0, 75)
        });

        // OUTER TOP WALL
        CreateWallChain(new Vector3[]
        {
            new Vector3(90, 0, 115),
            new Vector3(135, 0, 130),
            new Vector3(175, 0, 110),
            new Vector3(220, 0, 118),
            new Vector3(265, 0, 105),
            new Vector3(310, 0, 90),
            new Vector3(300, 0, 140),
            new Vector3(470, 0, 90)
        });

        // OUTER BOTTOM WALL
        CreateWallChain(new Vector3[]
        {
            new Vector3(110, 0, 75),
            new Vector3(135, 0, 10),
            new Vector3(200, 0, 0),
            new Vector3(260, 0, 15),
            new Vector3(300, 0, 30),
            new Vector3(360, 0, 10),
            new Vector3(430, 0, 25)
        });

        // INNER TOP CORRIDOR
        CreateWallChain(new Vector3[]
        {
            new Vector3(170, 0, 85),
            new Vector3(215, 0, 95),
            new Vector3(260, 0, 80),
            new Vector3(255, 0, 60)
        });

        // INNER LOWER CORRIDOR
        CreateWallChain(new Vector3[]
        {
            new Vector3(160, 0, 55),
            new Vector3(205, 0, 65),
            new Vector3(255, 0, 55),
            new Vector3(330, 0, 35)
        });

        // LEFT DIAGONAL WALL
        CreateWallChain(new Vector3[]
        {
            new Vector3(120, 0, 80),
            new Vector3(150, 0, 20),
            new Vector3(200, 0, 10)
        });

        // RIGHT TRIANGLE FUNNEL
        CreateWallChain(new Vector3[]
        {
            new Vector3(320, 0, 80),
            new Vector3(430, 0, 50),
            new Vector3(320, 0, 35),
            new Vector3(340, 0, 55)
        });

        // RIGHT INNER LINE
        CreateWallChain(new Vector3[]
        {
            new Vector3(300, 0, 50),
            new Vector3(380, 0, 50)
        });

        // FINAL END CORRIDOR
        CreateWallChain(new Vector3[]
        {
            new Vector3(430, 0, 25),
            new Vector3(480, 0, 40)
        });

        CreateWallChain(new Vector3[]
        {
            new Vector3(470, 0, 90),
            new Vector3(480, 0, 60)
        });
    }

    void GenerateTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        Vector3[] treePositions = new Vector3[]
        {
            new Vector3(110, treeY, 108),
            new Vector3(120, treeY, 98),
            new Vector3(130, treeY, 92),

            new Vector3(150, treeY, 78),
            new Vector3(165, treeY, 70),
            new Vector3(180, treeY, 62),

            new Vector3(200, treeY, 85),
            new Vector3(215, treeY, 80),
            new Vector3(230, treeY, 76),

            new Vector3(210, treeY, 55),
            new Vector3(230, treeY, 52),
            new Vector3(250, treeY, 48),

            new Vector3(260, treeY, 60),
            new Vector3(275, treeY, 55),
            new Vector3(290, treeY, 50),

            new Vector3(305, treeY, 65),
            new Vector3(320, treeY, 60),
            new Vector3(335, treeY, 55),

            new Vector3(350, treeY, 65),
            new Vector3(360, treeY, 55),

            new Vector3(375, treeY, 50),
            new Vector3(390, treeY, 45),
            new Vector3(405, treeY, 42),

            new Vector3(420, treeY, 50),
            new Vector3(435, treeY, 45),
            new Vector3(450, treeY, 40)
        };

        for (int i = 0; i < treePositions.Length; i++)
        {
            GameObject prefab = treePrefabs[i % treePrefabs.Length];
            Vector3 pos = treePositions[i] * levelScale;
            pos.y = treeY;

            GameObject obj = Instantiate(
                prefab,
                pos,
                Quaternion.Euler(0f, i * 23f, 0f),
                transform
            );

            float scale = Random.Range(1.5f, 2.2f) * levelScale;
            obj.transform.localScale = Vector3.one * scale;
        }
    }

    void GenerateRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        int rockCount = 240;

        float minX = 5f;
        float maxX = 460f;

        float minZ = 20f;
        float maxZ = 120f;

        for (int i = 0; i < rockCount; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            Vector3 pos = new Vector3(x, rockY, z) * levelScale;
            pos.y = rockY;

            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            GameObject rock = Instantiate(
                prefab,
                pos,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f),
                transform
            );
            
            if (rock.GetComponent<RockHazard>() == null)
            {
                rock.AddComponent<RockHazard>();
            }

            float scale = Random.Range(0.8f, 2.2f) * levelScale * 0.6f;
            rock.transform.localScale = Vector3.one * scale;
        }
    }

    void GenerateDecorations()
    {
        if (decorationPrefabs == null || decorationPrefabs.Length == 0) return;

        Vector3[] decorationPositions = new Vector3[]
        {
            new Vector3(115, decorationY, 112),

            new Vector3(150, decorationY, 85),
            new Vector3(170, decorationY, 75),

            new Vector3(200, decorationY, 70),
            new Vector3(225, decorationY, 60),

            new Vector3(260, decorationY, 60),
            new Vector3(285, decorationY, 55),

            new Vector3(310, decorationY, 60),
            new Vector3(340, decorationY, 52),

            new Vector3(370, decorationY, 50),
            new Vector3(395, decorationY, 45),

            new Vector3(420, decorationY, 45),
            new Vector3(450, decorationY, 38)
        };

        for (int i = 0; i < decorationPositions.Length; i++)
        {
            GameObject prefab = decorationPrefabs[i % decorationPrefabs.Length];
            Vector3 pos = decorationPositions[i] * levelScale;
            pos.y = decorationY;

            Instantiate(
                prefab,
                pos,
                Quaternion.Euler(0f, i * 18f, 0f),
                transform
            );
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
        start *= levelScale;
        end *= levelScale;

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
        wall.transform.localScale = new Vector3(
            wallThickness,
            wallHeight,
            length
        );
    }

    void PositionPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("[SPAWN] Player Transform is not assigned in the level generator.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Exact coordinates. No levelScale applied.
        player.position = playerStartPosition;
        player.rotation = Quaternion.Euler(playerStartRotation);

        Physics.SyncTransforms();

        if (cc != null)
            cc.enabled = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        Debug.Log("[SPAWN] Player positioned at: " + player.position);
    }

    void ClearChildren()
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            // Safety check: do not destroy the player if it is accidentally a child
            // of the map generator.
            if (player != null && child == player)
                continue;

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