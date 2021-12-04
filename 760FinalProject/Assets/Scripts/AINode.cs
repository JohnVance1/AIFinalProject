using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode
{
    public Vector3 pos;
    public int tileX;
    public int tileY;

    public AINode(Vector3 position, int x, int y)
    {
        pos = position;
        tileX = x;
        tileY = y;
    }


}
