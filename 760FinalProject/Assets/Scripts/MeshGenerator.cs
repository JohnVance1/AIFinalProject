using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;
    
    /// <summary>
    /// Takes in the map from the LevelGenerator and 
    /// creates a mesh of triangles based on the active points
    /// </summary>
    /// <param name="map"></param>
    /// <param name="squareSize"></param>
    public void GenerateMesh(float[,] map, float squareSize)
    {
        grid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();
        if (grid.squares != null)
        {
            for (int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(grid.squares[x, y]);
                }
            }
        }
        

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public void GenerateGrid(float[,] map, float squareSize)
    {
        grid = new SquareGrid(map, squareSize);

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
    public SquareGrid(float[,] map, float squareSize)
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
                nodes[x, y] = new Node(pos, map[x, y] == 1, squareSize);
            }
        }

        // Creates the array of squares
        squares = new Square[nodeCountX - 1, nodeCountY - 1];
        for (int x = 0; x < nodeCountX - 1; x++)
        {
            for (int y = 0; y < nodeCountY - 1; y++)
            {
                // Adds a new square for each square in the original map
                squares[x, y] = new Square(nodes[x, y + 1], nodes[x + 1, y + 1], nodes[x + 1, y], nodes[x, y]);
            }
        }

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
    public Square(Node _topLeft, Node _topRight, Node _bottomRight, Node _bottomLeft)
    {
        topLeft = _topLeft;
        topRight = _topRight;
        bottomRight = _bottomRight;
        bottomLeft = _bottomLeft;

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

    public Node(Vector3 position, bool act, float squareSize)
    {
        active = act;
        above = new Node(position + Vector3.forward * squareSize / 2);
        right = new Node(position + Vector3.right * squareSize / 2);
        pos = position;
    }

    public Node(Vector3 position)
    {
        pos = position;
    }

}
