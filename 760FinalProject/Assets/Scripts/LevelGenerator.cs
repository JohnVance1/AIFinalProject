using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

public struct MapNodes
{
    public int active;
    public float value;
    public int config;

}

public enum Generation
{
    RandomFill,
    PerlinNoise = 1,
    FractalBrownianNoise = 2
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private NodeSpawner spawner;

    public int width;
    public int height;

    public string seed;
    public bool randomSeed;
    private float increment;
    private int hardSeed;

    [Range(0, 100)]
    public int fillPercent;

    public MapNodes[,] map;
    private MapNodes[,] tempMap;
    public MeshGenerator meshGen;

    [SerializeField]
    private float squareSize;
    [SerializeField] [Range(1, 10)]
    public int resolution;

    [Header("Generation Method")]
    [SerializeField] private bool randomFill;
    [SerializeField] private bool perlinNoise;
    [SerializeField] private bool fractalBrownianNoise;

    private string[] toolbarSettings = { "Random", "Perlin Noise", "Fractal Brownian Noise" };
    private int toolbarInt = 0;

    public int octaves = 1;

    private void OnGUI()
    {
        // Genneration Type
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarSettings);

        // Fill Percent
        GUI.Label(new Rect(20f, 30f, 100f, 20f), "Fill Percent");
        GUI.Label(new Rect(0f, 45f, 20f, 20f), "0%");
        GUI.Label(new Rect(120f, 45f, 40f, 20f), "100%");
        fillPercent = (int)GUI.HorizontalSlider(new Rect(20f, 50f, 100f, 10f), fillPercent, 0, 100);

        // Width
        GUI.Label(new Rect(20f, 60f, 100f, 20f), "Width");
        GUI.Label(new Rect(0f, 75f, 20f, 20f), "8");
        GUI.Label(new Rect(120f, 75f, 40f, 20f), "100");
        width = (int)GUI.HorizontalSlider(new Rect(20f, 80f, 100f, 10f), width, 8, 100 * resolution);

        // Height
        GUI.Label(new Rect(20f, 90f, 100f, 20f), "Height");
        GUI.Label(new Rect(0f, 105f, 20f, 20f), "8");
        GUI.Label(new Rect(120f, 105f, 40f, 20f), "100");
        height = (int)GUI.HorizontalSlider(new Rect(20f, 110f, 100f, 10f), height, 8, 100 * resolution);

        // Random Seed
        randomSeed = GUI.Toggle(new Rect(20f, 140f, 100f, 20f), randomSeed, "Random Seed");

        // Seed
        GUI.Label(new Rect(20f, 160f, 100f, 20f), "Seed");
        seed = GUI.TextField(new Rect(20f, 180f, 50f, 20f), seed);

        if(toolbarInt == 2)
        {
            GUI.Label(new Rect(20f, 200f, 100f, 20f), "Octaves");
            GUI.Label(new Rect(0f, 220f, 20f, 20f), "1");
            GUI.Label(new Rect(120f, 220f, 40f, 20f), "10");
            octaves = (int)GUI.HorizontalSlider(new Rect(20f, 220f, 100f, 10f), octaves, 1, 10);

        }

        #region Switch
        //switch (toolbarInt)
        //{
        //    case 0:

        //        GUI.Label(new Rect(20f, 20f, 100f, 20f), "Fill Percent");
        //        GUI.Label(new Rect(0f, 35f, 20f, 20f), "30%");
        //        GUI.Label(new Rect(110f, 35f, 40f, 20f), "70%");

        //        fillPercent = (int)GUI.HorizontalSlider(new Rect(20f, 40f, 100f, 10f), fillPercent, 30, 70);
        //        break;

        //    case 1:
        //        GUI.Label(new Rect(20f, 20f, 100f, 20f), "Fill Percent");
        //        GUI.Label(new Rect(0f, 35f, 20f, 20f), "15%");
        //        GUI.Label(new Rect(110f, 35f, 40f, 20f), "55%");

        //        fillPercent = (int)GUI.HorizontalSlider(new Rect(20f, 40f, 100f, 10f), fillPercent, 15, 55);
        //        break;

        //    case 2:
        //        GUI.Label(new Rect(20f, 20f, 100f, 20f), "Fill Percent");
        //        GUI.Label(new Rect(0f, 35f, 20f, 20f), "50%");
        //        GUI.Label(new Rect(110f, 35f, 40f, 20f), "90%");

        //        fillPercent = (int)GUI.HorizontalSlider(new Rect(20f, 40f, 100f, 10f), fillPercent, 50, 90);
        //        break;

        //}
        #endregion

    }

    void OnDrawGizmos()
    {
        //if (map != null)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            Gizmos.color = (map[x, y].active == 1) ? Color.black : Color.white;
        //            Vector3 pos = new Vector3(-width * squareSize / 2 + x * squareSize + squareSize / 2, 0, -height * squareSize / 2 + y * squareSize + squareSize / 2);
        //            Gizmos.DrawCube(pos, Vector3.one * squareSize);
        //        }
        //    }
        //}
    }

    void OnEnable()
    {
        increment = 0.03f;
        meshGen = GetComponent<MeshGenerator>();
        hardSeed = 1000;

        GenerateMap();
    }

    private void Update()
    {
        //GenerateMap();
        // On left mouse click generate a new map
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();

        }
    }

    /// <summary>
    /// Generate a new map
    /// </summary>
    private void GenerateMap()
    {
        //width *= resolution;
        //height *= resolution;
        map = new MapNodes[width * resolution, height * resolution];
        tempMap = map;

        // Fill in the map
        FillInMap();
        tempMap = map;

        // Smooth out the map
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
            map = tempMap;

        }

        meshGen.GenerateGrid(map, squareSize, fillPercent / 100f);

        spawner.ProcessMap();

        // Start generating a mesh
        meshGen.GenerateMesh(map, squareSize, fillPercent / 100f);
    }

    /// <summary>
    /// Creates the actual pockets for the cave
    /// </summary>
    private void SmoothMap()
    {
        for (int x = 0; x < width * resolution; x++)
        {
            for (int y = 0; y < height * resolution; y++)
            {
                int neighbourWallTiles = GetNearbyWalls(x, y);

                if (neighbourWallTiles > 4)
                {
                    map[x, y].active = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    map[x, y].active = 0;
                }

            }
        }

    }

    /// <summary>
    /// Gets the number of walls around the current square
    /// Gets the 8 around 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int GetNearbyWalls(int x, int y)
    {
        int wallCount = 0;
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width * resolution && neighbourY >= 0 && neighbourY < height * resolution)
                {
                    // If its not the current square
                    if (neighbourX != x || neighbourY != y)
                    {
                        if (tempMap[neighbourX, neighbourY].active >= (fillPercent / 100f))
                        {
                            wallCount++;
                        }
                        
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    /// <summary>
    /// Populates the map based off of a seed
    /// </summary>
    private void FillInMap()
    {
        // Gets a random seed
        if (randomSeed)
        {
            seed = UnityEngine.Random.Range(0, 1000).ToString();
        }

        System.Random rand = new System.Random(seed.GetHashCode());

        switch(toolbarInt)
        {
            case 0:
                RandomFill(rand);
                break;

            case 1: 
                PerlinNoise(rand);
                break;

            case 2:
                FractalBrownianNoise(rand);
                break;

        }

    }

    /// <summary>
    /// Randomly fills in the map
    /// Also White Noise
    /// </summary>
    /// <param name="seed"></param>
    public void RandomFill(System.Random seed)
    {
        // Actually populates the map
        for (int x = 0; x < width * resolution; x++)
        {
            for (int y = 0; y < height * resolution; y++)
            {
                // If they are along the borders of the map then make them walls
                if (x == 0 || x == width * resolution - 1 || y == 0 || y == height * resolution - 1)
                {
                    map[x, y].active = 1;
                    map[x, y].value = 1;
                }
                else
                {
                    float randPercent = seed.Next(0, 100);
                    map[x, y].value = randPercent / 100f;

                    if (randPercent < fillPercent)
                    {
                        map[x, y].active = 1;
                    }
                    else
                    {
                        map[x, y].active = 0;

                    }
                }


            }
        }
    }

    public float GetNoiseAt(float x, float z, float heightMultiplier, int octaves, float persistance, float lacunarity)
    {
        float PerlinValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            // Get the perlin value at that octave and add it to the sum
            PerlinValue += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value
        return PerlinValue * heightMultiplier;
    }

    /// <summary>
    /// Allows for the use of Perlin Noise for generation
    /// </summary>
    /// <param name="seed"></param>
    public void PerlinNoise(System.Random seed)
    {
        float next = (float)seed.Next(0, 100) / 1000f;

        increment = next;

        hardSeed--;
        increment = (float)hardSeed / 1000;
        Debug.Log(increment);
        float xCoord = 0;

        for (int x = 0; x < width * resolution; x++)
        {
            float yCoord = 0;
            xCoord += increment;
            for (int y = 0; y < height * resolution; y++)
            {
                yCoord += increment;

                //float noise = GetNoiseAt(xCoord, yCoord, 1, 1, 0.3f, 0.5f);
                //float noise = Mathf.PerlinNoise(x / (float)width, y / (float)height) + increment;
                //Debug.Log(increment);
                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y].value = noise;

                // If they are along the borders of the map then make them walls
                if (x == 0 || x == width * resolution - 1 || y == 0 || y == height * resolution - 1)
                {
                    map[x, y].active = 1;
                }
                else if (noise < (fillPercent / 100f))
                {
                    map[x, y].active = 1;
                }
                else
                {
                    map[x, y].active = 0;

                }
                //else
                //{
                //    map[x, y] = noise;
                //}
                


            }
        }

    }

    public void FractalBrownianNoise(System.Random seed)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        float next = (float)rand.Next(10, 20) / 10;

        hardSeed--;
        next = (float)hardSeed / 1000;
        Debug.Log(next);

        float xCoord = 0;
        for (int i = 0; i < width * resolution; i++)
        {
            float yCoord = 0;
            xCoord += next;

            for (int j = 0; j < height * resolution; j++)
            {
                yCoord += next;
                //heightArray[i, j] = Mathf.PerlinNoise(xCoord / (float)resolution, yCoord / (float)resolution);
                float noise = GetNoiseAt(xCoord, yCoord, 0.5f, octaves, 0.9f, 0.7f);
                map[i, j].value = noise;

                if (i == 0 || i == width * resolution - 1 || j == 0 || j == height * resolution - 1)
                {
                    map[i, j].active = 1;
                }
                else if (noise < (fillPercent / 100f))
                {
                    map[i, j].active = 1;
                }
                else
                {
                    map[i, j].active = 0;

                }

            }
        }

    }

}
