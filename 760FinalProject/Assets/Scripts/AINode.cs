using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class of the nodes for pathfinding
/// </summary>
public class AINode
{
    public Vector3 pos;
    public int tileX;
    public int tileY;
    public float G;
    public float H;
    public float F { get { return G + H; } }
    public AINode parent;

    /// <summary>
    /// Nodes for pathfinding
    /// </summary>
    /// <param name="position"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public AINode(Vector3 position, int x, int y)
    {
        pos = position;
        tileX = x;
        tileY = y;

    }
}
