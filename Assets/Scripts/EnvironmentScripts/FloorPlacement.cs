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

     [Header("Special Wall Tiles")]
     [SerializeField] private TileBase bottomCenterTile;

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

    // =====================================
    // PHASE 1: STRAIGHT WALLS
    // =====================================

    foreach (Vector3Int floorCell in bounds.allPositionsWithin)
    {
        if (!floorTilemap.HasTile(floorCell))
            continue;

    //  top wall
        if (!floorTilemap.HasTile(floorCell + Vector3Int.up))
        {
            TryPlaceWall(
                floorCell + Vector3Int.up,
                topWallTile
            );
        }

    //  bottom wall
        if (!floorTilemap.HasTile(floorCell + Vector3Int.down))
        {
            TryPlaceWall(
                floorCell + Vector3Int.down,
                bottomWallTile
            );
        }
            
            // left wall
        if (!floorTilemap.HasTile(floorCell + Vector3Int.left))
        {
            TryPlaceWall(
                floorCell + Vector3Int.left,
                leftWallTile
            );
        }
       
        // right wall
        if (!floorTilemap.HasTile(floorCell + Vector3Int.right))
        {
            TryPlaceWall(
                floorCell + Vector3Int.right,
                rightWallTile
            );
        }
    }

    // =====================================
    // PHASE 2: OUTER + INNER CORNERS
    // =====================================

    foreach (Vector3Int floorCell in bounds.allPositionsWithin)
    {
        if (!floorTilemap.HasTile(floorCell))
            continue;

        Vector3Int up = floorCell + Vector3Int.up;
        Vector3Int down = floorCell + Vector3Int.down;
        Vector3Int left = floorCell + Vector3Int.left;
        Vector3Int right = floorCell + Vector3Int.right;

        Vector3Int topLeft = floorCell + new Vector3Int(-1, 1, 0);
        Vector3Int topRight = floorCell + new Vector3Int(1, 1, 0);
        Vector3Int bottomLeft = floorCell + new Vector3Int(-1, -1, 0);
        Vector3Int bottomRight = floorCell + new Vector3Int(1, -1, 0);

        // =================================
        // OUTER CORNERS
        // =================================

        //  top left outer corner
        if (!floorTilemap.HasTile(up) &&
            !floorTilemap.HasTile(left))
        {
            wallTilemap.SetTile(topLeft, topLeftCorner);
        }
        // top right outer corner
        if (!floorTilemap.HasTile(up) &&
            !floorTilemap.HasTile(right))
        {
            wallTilemap.SetTile(topRight, topRightCorner);
        }
        // bottom left outer corner
        if (!floorTilemap.HasTile(down) &&
            !floorTilemap.HasTile(left))
        {
            wallTilemap.SetTile(bottomLeft, bottomLeftCorner);
        }
        // bottom right outer corner
        if (!floorTilemap.HasTile(down) &&
            !floorTilemap.HasTile(right))
        {
            wallTilemap.SetTile(bottomRight, bottomRightCorner);
        }

        // =================================
        // INNER CORNERS
        // =================================

        //  top left inner corner
        if (floorTilemap.HasTile(up) &&
            floorTilemap.HasTile(left) &&
            !floorTilemap.HasTile(topLeft))
        {
            wallTilemap.SetTile(topLeft, innerTopLeftCorner);
        }
        //  top right inner corner
        if (floorTilemap.HasTile(up) &&
            floorTilemap.HasTile(right) &&
            !floorTilemap.HasTile(topRight))
        {
            wallTilemap.SetTile(topRight, innerTopRightCorner);
        }
        // bottom left inner corner
        if (floorTilemap.HasTile(down) &&
            floorTilemap.HasTile(left) &&
            !floorTilemap.HasTile(bottomLeft))
        {
            wallTilemap.SetTile(bottomLeft, innerBottomLeftCorner);
        }
        // bottom right inner corner
        if (floorTilemap.HasTile(down) &&
            floorTilemap.HasTile(right) &&
            !floorTilemap.HasTile(bottomRight))
        {
            wallTilemap.SetTile(bottomRight, innerBottomRightCorner);
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

