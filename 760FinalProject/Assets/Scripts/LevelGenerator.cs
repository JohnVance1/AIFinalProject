using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

public class LevelGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool randomSeed;

    [Header("Generation Method")]
    [SerializeField] private bool randomFill;
    [SerializeField] private bool perlinNoise;
    [SerializeField] private bool fractalBrownianNoise;

    private float increment;

    [Range(0, 100)]
    public int fillPercent;

    private float[,] map;
    private float[,] tempMap;
    MeshGenerator meshGen;

    [SerializeField]
    private float squareSize;

    void Start()
    {
        increment = 0.03f;
        meshGen = GetComponent<MeshGenerator>();

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

        if(randomFill)
        {
            RandomFill(rand);
        }
        else if (perlinNoise)
        {
            PerlinNoise(rand);
        }
        else if (fractalBrownianNoise)
        {
            FractalBrownianNoise(rand);

        }

    }

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

    /// <summary>
    /// Allows for the use of Perlin Noise for generation
    /// </summary>
    /// <param name="seed"></param>
    public void PerlinNoise(System.Random seed)
    {
        float next = (float)seed.Next(0, 100) / 1000f;

        increment = next;

        float xCoord = 0;

        for (int x = 0; x < width; x++)
        {
            float yCoord = 0;
            xCoord += increment;
            for (int y = 0; y < height; y++)
            {
                yCoord += increment;

                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                // If they are along the borders of the map then make them walls
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else if (noise < ((float)fillPercent / 100f))
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


    }

}
