using UnityEngine;

public class Node
{
    public int x, y;
    public bool isWalkable;
    public int gCost, hCost, fCost;
    public Node cameFromNode;

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void ChangeWalkableStatus()
    {
        this.isWalkable = false;
        Debug.Log("Walkable status of this node is : " + this.isWalkable);
    }

}

