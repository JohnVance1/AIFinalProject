using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the nodes
/// </summary>
public class NodeSpawner : MonoBehaviour
{
    private List<List<AINode>> nodes = new List<List<AINode>>();
    private List<GameObject> objects = new List<GameObject>();
    public List<AINode> largest;

    [SerializeField]
    private PathFinding path;
    [SerializeField]
    private LevelGenerator level;
    [SerializeField]
    private GameObject cubePrefab;

    
    /// <summary>
    /// Gets the largest open area
    /// </summary>
    public void ShowNodes()
    {
        //Clear();
        largest = new List<AINode>();
        foreach (List<AINode> element in nodes)
        {
            if(element.Count > largest.Count)
            {
                largest = element;
            }
            

        }
        //Clear();
        Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //DisplayAllRegions(largest);

        GetPoints(largest);
    }

    public void DisplayAllRegions(List<AINode> element)
    {
        //Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Color newColor = Color.red;
        foreach (AINode node in element)
        {
            if (level.meshGen.grid.squares[node.tileX, node.tileY].configuration == 0)
            {
                Vector3 pos = new Vector3(-level.map.GetLength(0) / 2 + node.tileX * 1 + 1 / 2, 0, -level.map.GetLength(0) / 2 + node.tileY * 1 + 1 / 2);

                GameObject gO = Instantiate(cubePrefab, node.pos, Quaternion.identity);
                gO.GetComponent<MeshRenderer>().material.color = newColor;
                objects.Add(gO);
            }
        }
    }

    /// <summary>
    /// Gets the highest and lowest points in the largest area
    /// </summary>
    /// <param name="largest"></param>
    public void GetPoints(List<AINode> largest)
    {
        List<AINode> highestList = new List<AINode>();
        List<AINode> lowestList = new List<AINode>();

        AINode[,] largestMap = new AINode[level.width * level.resolution, level.height * level.resolution];

        float highest = -1000f;
        float lowest = 1000f;
        highestList.Clear();
        lowestList.Clear();

        foreach (AINode node in largest)
        {
            largestMap[node.tileX, node.tileY] = node;

            if (node.pos.z > highest)
            {
                highest = node.pos.z;
            }
            if (node.pos.z < lowest)
            {
                lowest = node.pos.z;
            }
        }

        foreach (AINode node in largest)
        {
            if (node.pos.z == highest)
            {
                highestList.Add(node);
            }
            if (node.pos.z == lowest)
            {
                lowestList.Add(node);
            }
        }

        if (highestList.Count != 0 && lowestList.Count != 0)
        {
            //path.AStar(highestList[0], lowestList[0], largestMap);

        }
        else
        {
            path.Clear();

        }
    }


    /// <summary>
    /// Processes the entire map and gets the areas
    /// </summary>
    public void ProcessMap()
    {
        List<int> defaultConfig = new List<int>(){ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        List<int> bottomConfig = new List<int>() { 1, 2, 3, 7, 11 };

        List<List<AINode>> wallRegions = GetRegions(defaultConfig, 1);
        int wallThresholdSize = 50;

        foreach (List<AINode> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (AINode tile in wallRegion)
                {
                    level.map[tile.tileX, tile.tileY].active = 0;
                }
            }
        }

        List<List<AINode>> roomRegions = GetRegions(defaultConfig, 0);
        nodes = roomRegions;

        int roomThresholdSize = 50;

        foreach (List<AINode> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (AINode tile in roomRegion)
                {
                    level.map[tile.tileX, tile.tileY].active = 1;
                }
            }
        }

        ShowNodes();
    }

    /// <summary>
    /// Gets a region of open area or the walls
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    List<List<AINode>> GetRegions(List<int> configurations, int tileType = 3)
    {
        List<List<AINode>> regions = new List<List<AINode>>();
        int width = level.width * level.resolution;
        int height = level.height * level.resolution;

        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && CheckTileType(tileType, level.map[x, y]) && configurations.Contains(level.map[x, y].config))
                {
                    List<AINode> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (AINode tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                 
                    }
                }
            }
        }

        return regions;
    }


    public bool CheckTileType(int tiletype, MapNodes current)
    {
        if(tiletype == 3)
        {
            return true;
        }
        else
        {
            return current.active == tiletype;
        }



    }

    /// <summary>
    /// Gets the individual regions
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <returns></returns>
    List<AINode> GetRegionTiles(int startX, int startY)
    {
        int width = level.width * level.resolution;
        int height = level.height * level.resolution;

        List<AINode> tiles = new List<AINode>();
        int[,] mapFlags = new int[width, height];
        int tileType = (int)level.map[startX, startY].active;

        Queue<AINode> queue = new Queue<AINode>();
        queue.Enqueue(new AINode(level.meshGen.grid.nodes[startX, startY].pos, startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            AINode tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= (int)tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == (int)tile.tileY || x == (int)tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && level.map[x, y].active == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new AINode(level.meshGen.grid.nodes[x, y].pos, x, y));
                        }
                    }
                }
            }
        }



        return tiles;
    }

    /// <summary>
    /// Checks to see if it is in the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < level.width * level.resolution && y >= 0 && y < level.height * level.resolution;
    }


}
