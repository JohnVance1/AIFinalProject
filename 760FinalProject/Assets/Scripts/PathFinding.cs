using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

/// <summary>
/// Gets the path between the highest and lowest nodes
/// </summary>
public class PathFinding : SerializedMonoBehaviour
{
    private AINode highNode;
    private AINode lowNode;

    private List<GameObject> gOPath = new List<GameObject>();

    [SerializeField]
    private GameObject pathPrefab;

    private List<AINode> closed = new List<AINode>();
    private List<AINode> open = new List<AINode>();

    [OdinSerialize]
    private List<AINode> path = new List<AINode>();
    private AINode[,] graph;

    private Dictionary<AINode, AINode> visited = new Dictionary<AINode, AINode>();
    private Dictionary<AINode, float> moveCost = new Dictionary<AINode, float>();
    private float priority;
    private Dictionary<string, AINode> visit = new Dictionary<string, AINode>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(AINode node in path)
        {
            Gizmos.DrawCube(node.pos, Vector3.one * 0.8f);
        }
    }

    /// <summary>
    /// Clears all of the lists
    /// </summary>
    public void Clear()
    {
        path.Clear();
        open.Clear();
        closed.Clear();
    }

    /// <summary>
    /// The actual algorithm for A*
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="map"></param>
    public void AStar(AINode start, AINode goal, AINode[,] map)
    {
        path.Clear();
        closed.Clear();
        open.Clear();
        graph = map;
        
        open.Add(start);

        while (open.Count > 0)
        {
            AINode current = open[0]; 

            for(int i = 0; i < open.Count; i++)
            {
                if((open[i].F < current.F ||
                    open[i].F == current.F) &&
                    open[i].H < current.H)
                {
                    current = open[i];
                }

            }

            open.Remove(current);
            closed.Add(current);

            if (current == goal)
            {
                ConstructPath(start, goal);
                break;
            }

            foreach (AINode element in Neighbors(current))
            {
                if(closed.Contains(element))
                {
                    continue;
                }

                float newCost = current.G + GetDistance(current, element);

                if (newCost < element.G || 
                    !open.Contains(element))
                {
                    element.G = newCost;
                    element.H = GetDistance(element, goal);
                    element.parent = current;
                    open.Add(element);

                }

            }

        }
        
        // Should allow for the path to be built
        AINode key = goal;
        for (int i = 0; i < visited.Count; i++)
        {
            if (visited.ContainsValue(key))
            {
                path.Add(visited[key]);
                key = visited[key];

            }

        }


    }

    /// <summary>
    /// Constructs the path for the A* algorithm
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    public void ConstructPath(AINode start, AINode goal)
    {
        foreach(GameObject gO in gOPath)
        {
            Destroy(gO);
        }
        gOPath.Clear();

        AINode current = goal;

        while(current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(start);

        path.Reverse();

        foreach(AINode node in path)
        {
            gOPath.Add(Instantiate(pathPrefab, node.pos, Quaternion.identity));
        }

    }

    /// <summary>
    /// Gets the distance between two nodes
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int GetDistance(AINode a, AINode b)
    {
        int x = Mathf.Abs(a.tileX - b.tileX);
        int y = Mathf.Abs(a.tileY - b.tileY);

        if(x > y)
        {
            return 14 * y + 10 * (x - y);
        }
        return 14 * x + 10 * (y - x);

    }

    /// <summary>
    /// Gets all of the neighbors around a certain point
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public List<AINode> Neighbors(AINode current)
    {
        AINode next;
        List<AINode> neighborList = new List<AINode>();

        #region Cardinal Directions
        next = graph[current.tileX - 1, current.tileY];
        if (next != null)
        {
            neighborList.Add(next);
        }

        next = graph[current.tileX + 1, current.tileY];
        if (next != null)
        {
            neighborList.Add(next);

        }

        next = graph[current.tileX, current.tileY - 1];
        if (next != null)
        {
            neighborList.Add(next);

        }

        next = graph[current.tileX, current.tileY + 1];
        if (next != null)
        {
            neighborList.Add(next);

        }
        #endregion

        #region Diagonals
        next = graph[current.tileX - 1, current.tileY - 1];
        if (next != null)
        {
            neighborList.Add(next);
        }

        next = graph[current.tileX + 1, current.tileY - 1];
        if (next != null)
        {
            neighborList.Add(next);

        }

        next = graph[current.tileX - 1, current.tileY + 1];
        if (next != null)
        {
            neighborList.Add(next);

        }

        next = graph[current.tileX + 1, current.tileY + 1];
        if (next != null)
        {
            neighborList.Add(next);

        }
        #endregion

        return neighborList;

    }



}
