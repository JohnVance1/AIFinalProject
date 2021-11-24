using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;


public enum Generation
{
    RandomFill,
    PerlinNoise = 1,
    FractalBrownianNoise = 2
}

public class LevelGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool randomSeed;
    private float increment;
    private int hardSeed;

    [Range(0, 100)]
    public int fillPercent;

    private float[,] map;
    private float[,] tempMap;
    MeshGenerator meshGen;

    [SerializeField]
    private float squareSize;

    [Header("Generation Method")]
    [SerializeField] private bool randomFill;
    [SerializeField] private bool perlinNoise;
    [SerializeField] private bool fractalBrownianNoise;

    private string[] toolbarSettings = { "Random", "Perlin Noise", "Fractal Brownian Noise" };
    private int toolbarInt = 0;

    public int octaves = 1;

    private void OnGUI()
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarSettings);
    }

    void Start()
    {
        increment = 0.03f;
        meshGen = GetComponent<MeshGenerator>();
        hardSeed = 1000;


        GenerateMap();
    }

    private void Update()
    {
        GenerateMap();
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
        map = new float[width, height];
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

        // Start generating a mesh
        meshGen.GenerateMesh(map, squareSize);
    }

    /// <summary>
    /// Creates the actual pockets for the cave
    /// </summary>
    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetNearbyWalls(x, y);

                if (neighbourWallTiles > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    map[x, y] = 0;
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
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    // If its not the current square
                    if (neighbourX != x || neighbourY != y)
                    {
                        if (tempMap[neighbourX, neighbourY] >= (fillPercent / 100f))
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // If they are along the borders of the map then make them walls
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    if (seed.Next(0, 100) < fillPercent)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        map[x, y] = 0;

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

        for (int x = 0; x < width; x++)
        {
            float yCoord = 0;
            xCoord += increment;
            for (int y = 0; y < height; y++)
            {
                yCoord += increment;

                //float noise = GetNoiseAt(xCoord, yCoord, 1, 1, 0.3f, 0.5f);
                //float noise = Mathf.PerlinNoise(x / (float)width, y / (float)height) + increment;
                //Debug.Log(increment);
                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                //
                // If they are along the borders of the map then make them walls
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else if (noise < (fillPercent / 100f))
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = 0;

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
        for (int i = 0; i < width; i++)
        {
            float yCoord = 0;
            xCoord += next;

            for (int j = 0; j < height; j++)
            {
                yCoord += next;
                //heightArray[i, j] = Mathf.PerlinNoise(xCoord / (float)resolution, yCoord / (float)resolution);
                float noise = GetNoiseAt(xCoord, yCoord, 0.5f, octaves, 0.9f, 0.7f);

                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    map[i, j] = 1;
                }
                else if (noise < (fillPercent / 100f))
                {
                    map[i, j] = 1;
                }
                else
                {
                    map[i, j] = 0;

                }

            }
        }

    }

}
