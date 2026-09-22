using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorPlacement : MonoBehaviour
{
    [Header("Walls")]
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
    [SerializeField] private TileBase bottomInnerTile;
    [SerializeField] private TileBase topCenterTile;
    [SerializeField] private TileBase topInnerTile;

    [Header("Preview")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private Tilemap floorTilemap;

    [Header("Mode")]
    [SerializeField] private CameraLineController cameraLineController;

    private Vector3Int currentCell;

    private bool isFloorMode = true;


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // B = Floor / Pencil mode switch
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleMode();
        }

        // Pencil mode mein FloorPlacement kuch nahi karega
        if (!isFloorMode)
            return;

        UpdateFloorPreview();

        // Floor place
        if (Input.GetMouseButtonDown(0))
        {
            PlaceFourCellFloor();
        }
    }


    // =========================================================
    // TOGGLE MODE
    // =========================================================

    private void ToggleMode()
    {
        isFloorMode = !isFloorMode;

        if (isFloorMode)
        {
            // -----------------------------------------
            // FLOOR MODE ON
            // -----------------------------------------

            ClearPreview();

            if (cameraLineController != null)
            {
                cameraLineController.SetCameraMode(false);
            }
        }
        else
        {
            // -----------------------------------------
            // PENCIL MODE ON
            // -----------------------------------------

            ClearPreview();

            if (cameraLineController != null)
            {
                cameraLineController.SetCameraMode(true);
            }
        }
    }


    // =========================================================
    // FLOOR PREVIEW
    // =========================================================

    private void UpdateFloorPreview()
    {
        if (mainCamera == null || grid == null)
            return;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

        currentCell =
            grid.WorldToCell(mouseWorldPosition);

        ShowFourCellPreviewGrid();
    }


    private void ShowFourCellPreviewGrid()
    {
        if (previewTilemap == null)
            return;

        previewTilemap.ClearAllTiles();

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int cell =
                    new Vector3Int(
                        currentCell.x + x,
                        currentCell.y + y,
                        0
                    );

                previewTilemap.SetTile(
                    cell,
                    floorTile
                );
            }
        }
    }


    private void ClearPreview()
    {
        if (previewTilemap != null)
        {
            previewTilemap.ClearAllTiles();
        }
    }


    // =========================================================
    // NORMAL FLOOR PLACEMENT
    // =========================================================

    private void PlaceFourCellFloor()
    {
        if (floorTilemap == null)
            return;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int cell =
                    new Vector3Int(
                        currentCell.x + x,
                        currentCell.y + y,
                        0
                    );

                floorTilemap.SetTile(
                    cell,
                    floorTile
                );
            }
        }

        PlaceWall();
    }


    // =========================================================
    // PENCIL SE FLOOR PLACE
    // =========================================================

    public void PlaceFloorAtCell(Vector3Int centerCell)
    {
        if (floorTilemap == null)
            return;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector3Int cell =
                    new Vector3Int(
                        centerCell.x + x,
                        centerCell.y + y,
                        0
                    );

                floorTilemap.SetTile(
                    cell,
                    floorTile
                );
            }
        }
    }


    // =========================================================
    // REBUILD WALLS
    // =========================================================

    public void RebuildWalls()
    {
        PlaceWall();
    }


    // =========================================================
    // WALL SYSTEM
    // =========================================================

    private void PlaceWall()
    {
        if (wallTilemap == null ||
            floorTilemap == null)
            return;

        wallTilemap.ClearAllTiles();

        BoundsInt bounds =
            floorTilemap.cellBounds;


        // =====================================================
        // PHASE 1
        // STRAIGHT WALLS
        // =====================================================

        foreach (Vector3Int floorCell in bounds.allPositionsWithin)
        {
            if (!floorTilemap.HasTile(floorCell))
                continue;


            // TOP
            if (!floorTilemap.HasTile(
                floorCell + Vector3Int.up))
            {
                TryPlaceWall(
                    floorCell + Vector3Int.up,
                    topWallTile
                );
            }


            // BOTTOM
            if (!floorTilemap.HasTile(
                floorCell + Vector3Int.down))
            {
                TryPlaceWall(
                    floorCell + Vector3Int.down,
                    bottomWallTile
                );
            }


            // LEFT
            if (!floorTilemap.HasTile(
                floorCell + Vector3Int.left))
            {
                TryPlaceWall(
                    floorCell + Vector3Int.left,
                    leftWallTile
                );
            }


            // RIGHT
            if (!floorTilemap.HasTile(
                floorCell + Vector3Int.right))
            {
                TryPlaceWall(
                    floorCell + Vector3Int.right,
                    rightWallTile
                );
            }
        }


        // =====================================================
        // PHASE 2
        // CORNERS
        // =====================================================

        foreach (Vector3Int floorCell in bounds.allPositionsWithin)
        {
            if (!floorTilemap.HasTile(floorCell))
                continue;


            Vector3Int up =
                floorCell + Vector3Int.up;

            Vector3Int down =
                floorCell + Vector3Int.down;

            Vector3Int left =
                floorCell + Vector3Int.left;

            Vector3Int right =
                floorCell + Vector3Int.right;


            Vector3Int topLeft =
                floorCell +
                new Vector3Int(-1, 1, 0);

            Vector3Int topRight =
                floorCell +
                new Vector3Int(1, 1, 0);

            Vector3Int bottomLeft =
                floorCell +
                new Vector3Int(-1, -1, 0);

            Vector3Int bottomRight =
                floorCell +
                new Vector3Int(1, -1, 0);


            // =================================================
            // OUTER TOP LEFT
            // =================================================

            if (!floorTilemap.HasTile(up) &&
                !floorTilemap.HasTile(left))
            {
                wallTilemap.SetTile(
                    topLeft,
                    topLeftCorner
                );
            }


            // =================================================
            // OUTER TOP RIGHT
            // =================================================

            if (!floorTilemap.HasTile(up) &&
                !floorTilemap.HasTile(right))
            {
                wallTilemap.SetTile(
                    topRight,
                    topRightCorner
                );
            }


            // =================================================
            // OUTER BOTTOM LEFT
            // =================================================

            if (!floorTilemap.HasTile(down) &&
                !floorTilemap.HasTile(left))
            {
                wallTilemap.SetTile(
                    bottomLeft,
                    bottomLeftCorner
                );
            }


            // =================================================
            // OUTER BOTTOM RIGHT
            // =================================================

            if (!floorTilemap.HasTile(down) &&
                !floorTilemap.HasTile(right))
            {
                wallTilemap.SetTile(
                    bottomRight,
                    bottomRightCorner
                );
            }


            // =================================================
            // INNER TOP LEFT
            // =================================================

            if (floorTilemap.HasTile(up) &&
                floorTilemap.HasTile(left) &&
                !floorTilemap.HasTile(topLeft))
            {
                Vector3Int cornerCell =
                    topLeft;

                bool hasFloorBottom =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.down
                    );

                bool hasFloorLeft =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.left
                    );

                bool hasFloorRight =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.right
                    );


                if (hasFloorBottom &&
                    hasFloorLeft &&
                    hasFloorRight)
                {
                    wallTilemap.SetTile(
                        cornerCell,
                        topCenterTile
                    );
                }
                else
                {
                    wallTilemap.SetTile(
                        cornerCell,
                        innerTopLeftCorner
                    );
                }
            }


            // =================================================
            // INNER TOP RIGHT
            // =================================================

            if (floorTilemap.HasTile(up) &&
                floorTilemap.HasTile(right) &&
                !floorTilemap.HasTile(topRight))
            {
                wallTilemap.SetTile(
                    topRight,
                    innerTopRightCorner
                );
            }


            // =================================================
            // INNER BOTTOM LEFT
            // =================================================

            if (floorTilemap.HasTile(down) &&
                floorTilemap.HasTile(left) &&
                !floorTilemap.HasTile(bottomLeft))
            {
                Vector3Int cornerCell =
                    bottomLeft;

                bool hasFloorTop =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.up
                    );

                bool hasFloorLeft =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.left
                    );

                bool hasFloorRight =
                    floorTilemap.HasTile(
                        cornerCell +
                        Vector3Int.right
                    );


                if (hasFloorTop &&
                    hasFloorLeft &&
                    hasFloorRight)
                {
                    wallTilemap.SetTile(
                        cornerCell,
                        bottomCenterTile
                    );


                    Vector3Int belowCell =
                        cornerCell +
                        Vector3Int.down;


                    while (
                        wallTilemap.HasTile(
                            belowCell))
                    {
                        wallTilemap.SetTile(
                            belowCell,
                            bottomInnerTile
                        );

                        belowCell +=
                            Vector3Int.down;
                    }
                }
                else
                {
                    wallTilemap.SetTile(
                        cornerCell,
                        innerBottomLeftCorner
                    );
                }
            }


            // =================================================
            // INNER BOTTOM RIGHT
            // =================================================

            if (floorTilemap.HasTile(down) &&
                floorTilemap.HasTile(right) &&
                !floorTilemap.HasTile(bottomRight))
            {
                wallTilemap.SetTile(
                    bottomRight,
                    innerBottomRightCorner
                );
            }
        }
    }


    // =========================================================
    // WALL HELPER
    // =========================================================

    private void TryPlaceWall(
        Vector3Int cell,
        TileBase wall)
    {
        if (!floorTilemap.HasTile(cell))
        {
            wallTilemap.SetTile(
                cell,
                wall
            );
        }
    }


    private void PlaceCorner(
        Vector3Int cell,
        TileBase corner)
    {
        if (!floorTilemap.HasTile(cell))
        {
            wallTilemap.SetTile(
                cell,
                corner
            );
        }
    }
}