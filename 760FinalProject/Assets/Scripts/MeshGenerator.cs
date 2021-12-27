using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class MeshGenerator : SerializedMonoBehaviour
{
    Mesh mesh;
    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;
    public float fillPercent;

    private Square[,] squareList;

    [SerializeField]
    private bool interpolation;

    /// <summary>
    /// Takes in the map from the LevelGenerator and 
    /// creates a mesh of triangles based on the active points
    /// </summary>
    /// <param name="map"></param>
    /// <param name="squareSize"></param>
    public void GenerateMesh(MapNodes[,] map, float squareSize, float fillPercent)
    {
        this.fillPercent = fillPercent;
        grid = new SquareGrid(map, squareSize, fillPercent);
        squareList = grid.ReturnList();


        vertices = new List<Vector3>();
        triangles = new List<int>();
        if (grid.squares != null)
        {
            for (int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.squares.GetLength(1); y++)
                {
                    if (interpolation)
                    {
                        InterpolationTriangulateSquare(grid.squares[x, y]);
                    }
                    else
                    {
                        TriangulateSquare(grid.squares[x, y]);
                    }
                }
            }
        }


        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void OnDrawGizmos()
    {
        //if (grid != null)
        //{
        //    for (int x = 0; x < grid.squares.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < grid.squares.GetLength(1); y++)
        //        {
        //            Gizmos.color = (grid.squares[x, y].topLeft.active) ? Color.black : Color.white;
        //            Gizmos.DrawCube(grid.squares[x, y].topLeft.pos, Vector3.one * 0.05f);

        //            Gizmos.color = (grid.squares[x, y].topRight.active) ? Color.black : Color.white;
        //            Gizmos.DrawCube(grid.squares[x, y].topRight.pos, Vector3.one * 0.05f);

        //            Gizmos.color = (grid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
        //            Gizmos.DrawCube(grid.squares[x, y].bottomRight.pos, Vector3.one * 0.05f);

        //            Gizmos.color = (grid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
        //            Gizmos.DrawCube(grid.squares[x, y].bottomLeft.pos, Vector3.one * 0.05f);

        //            Gizmos.color = Color.gray;
        //            Gizmos.DrawCube(grid.squares[x, y].centerTop.pos, Vector3.one * 0.01f);
        //            Gizmos.DrawCube(grid.squares[x, y].centerRight.pos, Vector3.one * 0.01f);
        //            Gizmos.DrawCube(grid.squares[x, y].centerBotton.pos, Vector3.one * 0.01f);
        //            Gizmos.DrawCube(grid.squares[x, y].centerLeft.pos, Vector3.one * 0.01f);

        //        }
        //    }
        //}
    }

    public void GenerateGrid(MapNodes[,] map, float squareSize, float fillPercent)
    {
        this.fillPercent = fillPercent;
        grid = new SquareGrid(map, squareSize, fillPercent);

    }

    /// <summary>
    /// Determines what the triangles are going to look like 
    /// based off of which Nodes are active
    /// </summary>
    /// <param name="square"></param>
    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 0001, 0010, 0100, 1000
            case 1:
                MeshFromPoints(square.centerBotton, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBotton);
                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 0011, 0110, 1001, 1100, 0101, 1010
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBotton);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBotton, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBotton, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBotton, square.centerLeft);
                break;

            // 0111, 1011, 1101, 1110
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBotton, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBotton, square.centerLeft);
                break;

            // 1111
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }

    }

    /// <summary>
    /// Determines what the triangles are going to look like 
    /// based off of which Nodes are active
    /// </summary>
    /// <param name="square"></param>
    void InterpolationTriangulateSquare(Square square)
    {
        float t1;
        float t2;

        switch (square.configuration)
        {
            case 0:
                break;
            case 1:
                t1 = GetT(square.topLeft, square.bottomLeft, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.centerBotton, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                t1 = GetT(square.topRight, square.bottomRight, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBotton);
                break;
            case 3:
                t1 = GetT(square.topLeft, square.bottomLeft, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);

                break;
            case 4:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);

                break;

            case 5:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topLeft, square.bottomLeft, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t2);

                t1 = GetT(square.bottomLeft, square.bottomRight, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBotton, square.bottomLeft, square.centerLeft);

                break;

            case 6:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBotton);

                break;

            case 7:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topLeft, square.bottomLeft, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t2);

                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);

                break;

            case 8:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topLeft, square.bottomLeft, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t2);

                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);

                break;
            case 9:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.centerTop, square.centerBotton, square.bottomLeft);

                break;
            case 10:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topLeft, square.bottomLeft, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t2);

                t1 = GetT(square.bottomLeft, square.bottomRight, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBotton, square.centerLeft);

                break;
            case 11:
                t1 = GetT(square.topLeft, square.topRight, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerTop = new Node(Vector3.Lerp(square.topLeft.pos, square.topRight.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerTop = GetInterpolatedNode(square.topLeft, square.topRight, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);

                break;
            case 12:
                t1 = GetT(square.topLeft, square.bottomLeft, fillPercent);
                t2 = GetT(square.topRight, square.bottomRight, fillPercent);

                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t1));
                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t2));

                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t1);
                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);

                break;
            case 13:
                t1 = GetT(square.topRight, square.bottomRight, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerRight = new Node(Vector3.Lerp(square.topRight.pos, square.bottomRight.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerRight = GetInterpolatedNode(square.topRight, square.bottomRight, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBotton, square.bottomLeft);

                break;
            case 14:
                t1 = GetT(square.topLeft, square.bottomLeft, fillPercent);
                t2 = GetT(square.bottomLeft, square.bottomRight, fillPercent);

                square.centerLeft = new Node(Vector3.Lerp(square.topLeft.pos, square.bottomLeft.pos, t1));
                square.centerBotton = new Node(Vector3.Lerp(square.bottomLeft.pos, square.bottomRight.pos, t2));

                //square.centerLeft = GetInterpolatedNode(square.topLeft, square.bottomLeft, t1);
                //square.centerBotton = GetInterpolatedNode(square.bottomLeft, square.bottomRight, t2);

                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBotton, square.centerLeft);

                break;

            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);

                break;

            default:
                break;
        }

    }

    /// <summary>
    /// Depending on how many points are in the "points" it creates a triangle for each one
    /// </summary>
    /// <param name="points"></param>
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if (points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    public float GetT(Node a, Node b, float value)
    {
        float v2 = Mathf.Max(b.v, a.v);
        float v1 = Mathf.Min(b.v, a.v);

        //return (value - a.v) / (a.v - b.v);
        return (a.v + b.v) / 2f;
        //return 0.5f;
    }

    public Node GetInterpolatedNode(Node a, Node b, float t)
    {
        float x = (1 - t) * a.pos.x + t * b.pos.x;
        float z = (1 - t) * a.pos.z + t * b.pos.z;

        return new Node(new Vector3(x, a.pos.y, z));
    }

    /// <summary>
    /// Adds all of the possible vertices to the List<Vector3>
    /// </summary>
    /// <param name="points"></param>
    void AssignVertices(Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].index == -1)
            {
                points[i].index = vertices.Count;
                vertices.Add(points[i].pos);
            }
        }
    }

    /// <summary>
    /// Actually adds the points to the triangle's List<int>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.index);
        triangles.Add(b.index);
        triangles.Add(c.index);

    }
    
}

/// <summary>
/// Populates the new grid with Nodes for each side and corner of every square
/// </summary>
public class SquareGrid
{
    public Square[,] squares;
    public Node[,] nodes;

    /// <summary>
    /// Populates the new grid with Nodes for each side and corner of every square
    /// </summary>
    /// <param name="map">The map from the LevelGenerator class</param>
    /// <param name="squareSize"></param>
    public SquareGrid(MapNodes[,] map, float squareSize, float fillPercent)
    {
        int nodeCountX = map.GetLength(0);
        int nodeCountY = map.GetLength(1);
        float mapWidth = nodeCountX * squareSize;
        float mapHeight = nodeCountY * squareSize;

        nodes = new Node[nodeCountX, nodeCountY];

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int y = 0; y < nodeCountY; y++)
            {
                // Gets the middle point of each square
                Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);

                // Adds it to the new list of nodes
                nodes[x, y] = new Node(pos, map[x, y].active == 1, squareSize, map[x, y].value);
            }
        }

        // Creates the array of squares
        squares = new Square[nodeCountX - 1, nodeCountY - 1];
        for (int x = 0; x < nodeCountX - 1; x++)
        {
            for (int y = 0; y < nodeCountY - 1; y++)
            {
                // Adds a new square for each square in the original map
                squares[x, y] = new Square(nodes[x, y + 1], nodes[x + 1, y + 1], nodes[x + 1, y], nodes[x, y], fillPercent);
            }
        }


    }

    public Square[,] ReturnList()
    {
        return squares;
    }

}


/// <summary>
/// Gets the single square, its four corners, and its four sides
/// </summary>
public class Square
{
    public Node topLeft;
    public Node topRight;
    public Node bottomRight;
    public Node bottomLeft;

    public Node centerTop;
    public Node centerRight;
    public Node centerBotton;
    public Node centerLeft;

    public int configuration;

    /// <summary>
    /// Gets the single square, its four corners, and its four sides
    /// </summary>
    /// <param name="_topLeft"></param>
    /// <param name="_topRight"></param>
    /// <param name="_bottomRight"></param>
    /// <param name="_bottomLeft"></param>
    public Square(Node _topLeft, Node _topRight, Node _bottomRight, Node _bottomLeft, float _fillPercent)
    {
        topLeft = _topLeft;
        topRight = _topRight;
        bottomRight = _bottomRight;
        bottomLeft = _bottomLeft;

        // TODO: add in linear interpolation using the fillpercent, the value, and points

        //centerTop = GetInterpolatedNode(topLeft, topRight, GetT(topLeft, topRight, _fillPercent));
        //centerRight = GetInterpolatedNode(topRight, bottomRight, GetT(topRight, bottomRight, _fillPercent));
        //centerBotton = GetInterpolatedNode(bottomRight, bottomLeft, GetT(bottomRight, bottomLeft, _fillPercent));
        //centerLeft = GetInterpolatedNode(topLeft, bottomLeft, GetT(topLeft, bottomLeft, _fillPercent));

        centerTop = topLeft.right;
        centerRight = bottomRight.above;
        centerBotton = bottomLeft.right;
        centerLeft = bottomLeft.above;

        if (topLeft.active)
        {
            configuration += 8;
        }
        if (topRight.active)
        {
            configuration += 4;
        }
        if (bottomRight.active)
        {
            configuration += 2;
        }
        if (bottomLeft.active)
        {
            configuration += 1;
        }

    }

    

}

/// <summary>
/// The basic point for each corner and side
/// </summary>
public class Node
{
    public Vector3 pos;
    public int index = -1;
    public bool active;
    public Node above;
    public Node right;
    public float v;

    public Node(Vector3 position, bool act, float squareSize, float val = 0)
    {
        active = act;
        above = new Node(position + Vector3.forward * squareSize / 2);
        right = new Node(position + Vector3.right * squareSize / 2);
        pos = position;
        v = val;
    }

    public Node(Vector3 position)
    {
        pos = position;
    }

}
