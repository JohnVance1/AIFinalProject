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

    [Range(0, 100)]
    public int fillPercent;

    private int[,] map;
    private int[,] tempMap;
    MeshGenerator meshGen;


    void Start()
    {
        meshGen = GetComponent<MeshGenerator>();

        GenerateMap();
    }

    private void Update()
    {
        // On left mouse click generate a new map
        if(Input.GetMouseButtonDown(0))
        {
            GenerateMap();

        }
    }

    /// <summary>
    /// Generate a new map
    /// </summary>
    private void GenerateMap()
    {
        map = new int[width, height];
        tempMap = new int[width, height];

        // Fill in the map
        RandomFillMap();
        tempMap = map;

        // Smooth out the map
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
            map = tempMap;
        }

        // Start generating a mesh
        meshGen.GenerateMesh(map, 1);
    }

    /// <summary>
    /// Populates the map based off of a seed
    /// </summary>
    private void RandomFillMap()
    {
        // Gets a random seed
        if (randomSeed)
        {
            seed = UnityEngine.Random.Range(0, 1000).ToString();
        }

        System.Random rand = new System.Random(seed.GetHashCode());

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
                    if(rand.Next(0, 100) < fillPercent)
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
    /// Creates the actual pockets for the cave
    /// </summary>
    private void SmoothMap()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                // Gets the number of walls around the current square
                int closeWalls = GetNearbyWalls(x, y);

                if(closeWalls > 4)
                {
                    tempMap[x, y] = 1;
                }
                else if(closeWalls <= 4)
                {
                    tempMap[x, y] = 0;
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
                        wallCount += map[neighbourX, neighbourY];
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


    void OnDrawGizmos()
    {
        //if (map != null)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
        //            Vector3 pos = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
        //            Gizmos.DrawCube(pos, Vector3.one);
        //        }
        //    }
        //}
    }


}
