using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;



public class TilePosition : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14
    public Tilemap tilemap;
    private List<Node> nodes = new List<Node>();
    private List<Node> neighbors = new List<Node>();
    // private Node[,] nodes;
    public TileBase tile, obstacleTile;
    // public int x, y;
    public int gridWidth = 18; 
    public int gridHeight = 10; 
    private InputMaster _Pathfinding; // Reference to Input System 
    private Camera mainCamera;


    private void Awake()
    {
        _Pathfinding = new InputMaster();
    }

    private void OnEnable()
    {
        _Pathfinding.Enable();

        // Subscribe to the actions
        _Pathfinding.Pathfinding.RightClick.performed += CreateObstacle; 
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        _Pathfinding.Pathfinding.RightClick.performed -= CreateObstacle;
        _Pathfinding.Disable();
    }
    void Start()
    {
        CreateGrid();
        mainCamera = Camera.main;
    }

    private void CreateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                Node newNode = new Node(x, y, true);
                nodes.Add(newNode);
            }
        }
    }

    private void CreateObstacle(InputAction.CallbackContext context)
    {
        Vector2 position = _Pathfinding.Pathfinding.RightClickPosition.ReadValue<Vector2>();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);
        tilemap.SetTile(gridPosition, obstacleTile);
        Debug.Log("Placed tile via Touch");

        Node clickedNode = GetNodeAtPosition(gridPosition.x, gridPosition.y);
        if (clickedNode != null)
        {
            Debug.Log($"Clicked Node Position: x = {clickedNode.x}, y = {clickedNode.y}");
        }
        else
        {
            Debug.Log("Node not found at the clicked position.");
        }

        clickedNode.ChangeWalkableStatus();
    }

    private Node GetNodeAtPosition(int x, int y)
    {
        return nodes.Find(node => node.x == x && node.y == y);
    }


    private List<Node> GetNeighbors(Node node)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                Node neighbor = GetNodeAtPosition(checkX, checkY);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        
        foreach (Node neighbor in neighbors)
        {
            Debug.Log($"Neighbor at: ({neighbor.x}, {neighbor.y})");
        }

        return neighbors;
    }


    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);
        return Mathf.Max(distX, distY);  // Chebyshev distance
    }
}