using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase[] tiles; 

    private InputMaster playerInput; // Reference to Input System 
    private TileBase selectedTile; // Currently selected tile
    private Camera mainCamera;
    private Vector2 pointerPosition;

    private void Awake()
    {
        playerInput = new InputMaster();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        // Subscribe to the actions
        playerInput.PlayerInputs.Click.performed += OnClick; 
        playerInput.PlayerInputs.Touch.performed += OnPoint; 
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        playerInput.PlayerInputs.Click.performed -= OnClick;
        playerInput.PlayerInputs.Touch.performed -= OnPoint;
        playerInput.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        pointerPosition = playerInput.PlayerInputs.clickPosition.ReadValue<Vector2>();
        if (selectedTile != null)
        {
            PlaceTileAtPointer(pointerPosition);
            Debug.Log("Clicked screen");
        }
    }

    private void OnPoint(InputAction.CallbackContext context)
    {
        pointerPosition = playerInput.PlayerInputs.touchPosition.ReadValue<Vector2>();
        PlaceTileAtPointer(pointerPosition);
        Debug.Log("Placed tile via Touch");
    }

    private void PlaceTileAtPointer(Vector2 position)
    {
        if (selectedTile != null)
        {
            // Converting screen position to world position
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, Mathf.Abs(mainCamera.transform.position.z)));
            Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);


            tilemap.SetTile(gridPosition, selectedTile);
            Debug.Log("Tile placed at grid position: " + gridPosition);
        }
    }

    public void SelectTile(int tileIndex)
    {
        selectedTile = tiles[tileIndex];
    }
}
