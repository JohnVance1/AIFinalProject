using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSpawner : MonoBehaviour
{
    private List<List<AINode>> nodes = new List<List<AINode>>();
    private List<GameObject> objects = new List<GameObject>();
    
    [SerializeField]
    private LevelGenerator level;
    [SerializeField]
    private GameObject cubePrefab;

    private void Start()
    {
        //AddNodes();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //AddNodes();

        }
    }

    private void OnDrawGizmos()
    {
        



    }

    //public void AddNodes()
    //{
    //    nodes.Clear();
    //    foreach (Square element in level.meshGen.grid.squares)
    //    {
    //        if (!element.topLeft.active)
    //        {
    //            nodes.Add(new AINode(element.topLeft.pos));
    //        }
    //        if (!element.topRight.active)
    //        {
    //            nodes.Add(new AINode(element.topRight.pos));

    //        }
    //        if (!element.bottomLeft.active)
    //        {
    //            nodes.Add(new AINode(element.bottomLeft.pos));

    //        }
    //        if (!element.bottomRight.active)
    //        {
    //            nodes.Add(new AINode(element.bottomRight.pos));

    //        }

    //    }

    //}

    public void ShowNodes()
    {
        Clear();
        foreach (List<AINode> element in nodes)
        {
            List<AINode> region = element;
            //Gizmos.color = new Color(Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
            Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            foreach (AINode node in region)
            {
                //Gizmos.DrawCube(node.pos, Vector3.one * 0.4f);
                GameObject gO = Instantiate(cubePrefab, node.pos, Quaternion.identity);
                gO.GetComponent<MeshRenderer>().material.color = newColor;
                objects.Add(gO);
            }

        }
    }

    public void Clear()
    {
        if(objects.Count <= 0)
        {
            return;
        }
        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }
        objects.Clear();
    }

    public void ProcessMap()
    {
        List<List<AINode>> wallRegions = GetRegions(1);
        int wallThresholdSize = 50;

        foreach (List<AINode> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (AINode tile in wallRegion)
                {
                    level.map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<AINode>> roomRegions = GetRegions(0);
        nodes = roomRegions;

        int roomThresholdSize = 50;

        foreach (List<AINode> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (AINode tile in roomRegion)
                {
                    level.map[tile.tileX, tile.tileY] = 1;
                }
            }
        }

        ShowNodes();
    }

    List<List<AINode>> GetRegions(int tileType)
    {
        List<List<AINode>> regions = new List<List<AINode>>();
        int width = level.width;
        int height = level.height;

        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && level.map[x, y] == tileType)
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

    List<AINode> GetRegionTiles(int startX, int startY)
    {
        int width = level.width;
        int height = level.height;

        List<AINode> tiles = new List<AINode>();
        int[,] mapFlags = new int[width, height];
        int tileType = (int)level.map[startX, startY];

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
                        if (mapFlags[x, y] == 0 && level.map[x, y] == tileType)
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

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < level.width && y >= 0 && y < level.height;
    }


}
