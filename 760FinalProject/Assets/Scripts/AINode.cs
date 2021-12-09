using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode
{
    public Vector3 pos;
    public int tileX;
    public int tileY;
    public float G;
    public float H;
    public float F { get { return G + H; } }
    public AINode parent;

    public AINode(Vector3 position, int x, int y)
    {
        pos = position;
        tileX = x;
        tileY = y;

    }
}
