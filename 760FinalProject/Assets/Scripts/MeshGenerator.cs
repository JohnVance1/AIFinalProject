using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid grid;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        grid = new SquareGrid(map, squareSize);

        for (int x = 0; x < grid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < grid.squares.GetLength(1); y++)
            {
                TriangulateSquare(grid.squares[x, y]);
            }
        }
    }

    /// <summary>
    /// Determines what the triangles are going to look like 
    /// based off of which Nodes are active
    /// </summary>
    /// <param name="square"></param>
    void TriangulateSquare(Square square)
    {
        switch(square.configuration)
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

            // 0011, 0110, 1001, 1010, 0101, 1100
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;

            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBotton);
                break;

            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBotton, square.bottomLeft, square.centerLeft);
                break;

            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBotton, square.bottomLeft);
                break;

            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBotton, square.centerLeft);
                break;

            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
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

    void MeshFromPoints(params Node[] points)
    {

    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            for (int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (grid.squares[x, y].topLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(grid.squares[x, y].topLeft.pos, Vector3.one * 0.4f);

                    Gizmos.color = (grid.squares[x, y].topRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(grid.squares[x, y].topRight.pos, Vector3.one * 0.4f);

                    Gizmos.color = (grid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(grid.squares[x, y].bottomRight.pos, Vector3.one * 0.4f);

                    Gizmos.color = (grid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(grid.squares[x, y].bottomLeft.pos, Vector3.one * 0.4f);

                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(grid.squares[x, y].centerTop.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(grid.squares[x, y].centerRight.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(grid.squares[x, y].centerBotton.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(grid.squares[x, y].centerLeft.pos, Vector3.one * 0.15f);

                }
            }
        }
    }

    /// <summary>
    /// Populates the new grid with Nodes for each side and corner of every square
    /// </summary>
    public class SquareGrid
    {
        public Square[,] squares;

        /// <summary>
        /// Populates the new grid with Nodes for each side and corner of every square
        /// </summary>
        /// <param name="map">The map from the LevelGenerator class</param>
        /// <param name="squareSize"></param>
        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    // Gets the middle point of each square
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    // Adds it to the new list of nodes
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            // Creates the array of squares
            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    // Adds a new square for each square in the original map
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }

    /// <summary>
    /// Gets the single square, its four corners, and its four sides
    /// </summary>
    public class Square
    {
        public ControlNode topLeft;
        public ControlNode topRight;
        public ControlNode bottomRight;
        public ControlNode bottomLeft;

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
        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

           centerTop = topLeft.right;
           centerRight = bottomRight.above;
           centerBotton = bottomLeft.right;
           centerLeft = bottomLeft.above;

            if(topLeft.active)
            {
                configuration += 8;
            }
            else if(topRight.active)
            {
                configuration += 4;
            }
            else if (bottomRight.active)
            {
                configuration += 2;
            }
            else if(bottomLeft.active)
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

        public Node(Vector3 position)
        {
            pos = position;
        }

    }

    /// <summary>
    /// Used for each of the square's corners 
    /// which then allows for us to get the middle of each of the square's sides
    /// </summary>
    public class ControlNode : Node
    {
        public bool active;
        public Node above;
        public Node right;

        /// <summary>
        /// Used for each of the square's corners 
        /// which then allows for us to get the middle of each of the square's sides
        /// </summary>
        /// <param name="position"></param>
        /// <param name="act"></param>
        /// <param name="squareSize"></param>
        public ControlNode(Vector3 position, bool act, float squareSize) : base(position)
        {
            active = act;
            above = new Node(position + Vector3.forward * squareSize / 2);
            right = new Node(position + Vector3.right * squareSize / 2);
        }
    }
}
