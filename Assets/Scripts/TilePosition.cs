using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;



public class TilePosition : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    public Tilemap tilemap;
    private List<Node> nodes = new List<Node>();
    private List<Node> neighbors = new List<Node>();
    // private Node[,] nodes;
    public TileBase tile, obstacleTile, pathTile;
    // public int x, y;
    public int gridWidth = 18; 
    public int gridHeight = 10; 
    private InputMaster _Pathfinding; // Reference to Input System 
    private Camera mainCamera;

    private Node startNode, endNode, lastClickedNode;


    private void Awake()
    {
        _Pathfinding = new InputMaster();
    }

    private void OnEnable()
    {
        _Pathfinding.Enable();

        // Subscribe to the actions
        _Pathfinding.Pathfinding.RightClick.performed += CreateObstacle; 
        _Pathfinding.Pathfinding.LeftClick.performed += SetTarget; 
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        _Pathfinding.Pathfinding.RightClick.performed -= CreateObstacle;
        _Pathfinding.Pathfinding.LeftClick.performed -= SetTarget; 
        _Pathfinding.Disable();
    }
    void Start()
    {
        CreateGrid();
        mainCamera = Camera.main;
        startNode = GetNodeAtPosition(0, 0);
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
    private void SetTarget(InputAction.CallbackContext context)
    {
        Vector2 position = _Pathfinding.Pathfinding.LeftClickPosition.ReadValue<Vector2>();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);
        
        Debug.Log("Target Node is Set");

        Node clickedNode = GetNodeAtPosition(gridPosition.x, gridPosition.y);
        if (clickedNode != null)
        {
            // Set the endNode to the clicked node
            endNode = clickedNode;
            Debug.Log($"Target Node Position: x = {clickedNode.x}, y = {clickedNode.y}");

            // Automatically set the startNode to the last clicked node if it exists
            if (lastClickedNode != null)
            {
                startNode = lastClickedNode;
                Debug.Log($"Start Node Position: x = {startNode.x}, y = {startNode.y}");
            }  

            // Find the path if both nodes are set
            List<Node> path = FindPath(startNode, endNode);
            if (path != null)
            {
                Debug.Log("Path found!");
                foreach (Node node in path)
                {
                    Vector3Int pathPosition = new Vector3Int(node.x, node.y, 0);
                    tilemap.SetTile(pathPosition, pathTile); // Set the specified tile at the path position
                }
            }
            else
            {
                Debug.Log("No path found.");
            }
            
        }
        else
        {
            Debug.Log("Node not found at the clicked position.");
        }

        // Update the last clicked node to the current clicked node
        lastClickedNode = clickedNode;
    }


    private Node GetNodeAtPosition(int x, int y)
    {
        return nodes.Find(node => node.x == x && node.y == y);
    }


    private List<Node> GetNeighbors(Node node)
    {
        neighbors.Clear();
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
        
        return neighbors;
    }


    private int CalculateDistanceCost(Node a, Node b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }


    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                // Compare fCost and choose the lowest
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Check if we reached the target node
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue; // Skip if not walkable or already evaluated
                }

                int newCostToNeighbor = currentNode.gCost + CalculateDistanceCost(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = CalculateDistanceCost(neighbor, targetNode);
                    neighbor.cameFromNode = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // No path found
    }

    // Method to retrace the path from targetNode to startNode
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.cameFromNode;
        }
        
        path.Reverse(); // Reverse the path to get it from start to end
        return path;
    }

}