using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


public class PathFinding : SerializedMonoBehaviour
{
    private AINode highNode;
    private AINode lowNode;

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

    public void ConstructPath(AINode start, AINode goal)
    {
        AINode current = goal;

        while(current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(start);

        path.Reverse();
    }

    public int GetDistance(AINode a, AINode b)
    {
        int X = Mathf.Abs(a.tileX - b.tileX);
        int Y = Mathf.Abs(a.tileY - b.tileY);

        if(X > Y)
        {
            return 14 * Y + 10 * (X - Y);
        }
        return 14 * X + 10 * (Y - X);

    }

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
