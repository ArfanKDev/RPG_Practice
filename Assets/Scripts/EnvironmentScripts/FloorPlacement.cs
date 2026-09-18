using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorPlacement : MonoBehaviour
{
     [Header("walls")]
     [SerializeField] private Tilemap wallTilemap;
     [SerializeField] private TileBase topWallTile;
     [SerializeField] private TileBase bottomWallTile;
     [SerializeField] private TileBase leftWallTile;
     [SerializeField] private TileBase rightWallTile;
     
     [Header("Wall Corners")]
     [SerializeField] private TileBase topLeftCorner;
     [SerializeField] private TileBase topRightCorner;
     [SerializeField] private TileBase bottomLeftCorner;
     [SerializeField] private TileBase bottomRightCorner;

     [Header("Inner Corners")]
     [SerializeField] private TileBase innerTopLeftCorner;
     [SerializeField] private TileBase innerTopRightCorner;
     [SerializeField] private TileBase innerBottomLeftCorner;
     [SerializeField] private TileBase innerBottomRightCorner;

     [Header("Preview")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private Tilemap floorTilemap;

    private Vector3Int currentCell;
    private Vector3Int floorCell;

    private void Update()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

       currentCell = grid.WorldToCell(mouseWorldPosition);

        ShowFourCellPreviewGrid();

        // Left-click par actual floor place karo
        if (Input.GetMouseButtonDown(0))
        {
            PlaceFourCellFloor();
        }
    }
     private void ShowFourCellPreviewGrid()
    {
        previewTilemap.ClearAllTiles();

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int cell = new Vector3Int(currentCell.x + x, currentCell.y + y, 0);

                previewTilemap.SetTile(cell, floorTile);
            }
        }
    }

    private void PlaceFourCellFloor()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int cell = new Vector3Int( currentCell.x + x, currentCell.y + y, 0);

                floorTilemap.SetTile(cell, floorTile);
            }
        }
        PlaceWall();
    }

 private void PlaceWall()
{
    wallTilemap.ClearAllTiles();

    
    BoundsInt bounds = floorTilemap.cellBounds;

    foreach (Vector3Int floorCell in bounds.allPositionsWithin)
    {
        if (!floorTilemap.HasTile(floorCell))
            continue;

        // Walls
        TryPlaceWall(floorCell + Vector3Int.up, topWallTile);
        TryPlaceWall(floorCell + Vector3Int.down, bottomWallTile);
        TryPlaceWall(floorCell + Vector3Int.left, leftWallTile);
        TryPlaceWall(floorCell + Vector3Int.right, rightWallTile);

        // Corners
         if (!floorTilemap.HasTile(floorCell + Vector3Int.up) && !floorTilemap.HasTile(floorCell + Vector3Int.left))
        {
            PlaceCorner( floorCell + new Vector3Int(-1, 1, 0), topLeftCorner);
        }

        if (!floorTilemap.HasTile(floorCell + Vector3Int.up) && !floorTilemap.HasTile(floorCell + Vector3Int.right))
        {
            PlaceCorner(floorCell + new Vector3Int(1, 1, 0),topRightCorner);
        }

        if (!floorTilemap.HasTile(floorCell + Vector3Int.down) && !floorTilemap.HasTile(floorCell + Vector3Int.left))
        {
            PlaceCorner(floorCell + new Vector3Int(-1, -1, 0), bottomLeftCorner );
        }

        if (!floorTilemap.HasTile(floorCell + Vector3Int.down) && !floorTilemap.HasTile(floorCell + Vector3Int.right))
        {
            PlaceCorner(floorCell + new Vector3Int(1, -1, 0),bottomRightCorner);
        }

    }
}

    private void TryPlaceWall(Vector3Int cell, TileBase Wall)
    {
        if (!floorTilemap.HasTile(cell))
        {
            wallTilemap.SetTile(cell, Wall);
        }
    }
    private void PlaceCorner(Vector3Int cell, TileBase corner)
{
    if (!floorTilemap.HasTile(cell))
    {
        wallTilemap.SetTile(cell, corner);
    }
}
}

