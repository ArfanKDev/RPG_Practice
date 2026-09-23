using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorPlacement : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] private Tilemap backWallTilemap;
    [SerializeField] private Tilemap frontWallTilemap;
    [SerializeField] private Tilemap boundaryWallTilemap;

    [SerializeField] private TileBase boundaryWallTile;
    [SerializeField] private TileBase topWallTile;
    [SerializeField] private TileBase bottomWallTile;
    [SerializeField] private TileBase leftWallTile;
    [SerializeField] private TileBase rightWallTile;
    [SerializeField] private TileBase topInnerWallTile;

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
        if (backWallTilemap == null ||
            floorTilemap == null ||
            boundaryWallTilemap == null ||
            frontWallTilemap == null)
            return;

        backWallTilemap.ClearAllTiles();
        frontWallTilemap.ClearAllTiles();
        boundaryWallTilemap.ClearAllTiles();


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
     // Floor ke bilkul upar wali wall
    Vector3Int wallCell1 =
        floorCell + Vector3Int.up;

    // Uske upar
    Vector3Int wallCell2 =
        floorCell + new Vector3Int(0, 2, 0);

    // Sab se upar
    Vector3Int wallCell3 =
        floorCell + new Vector3Int(0, 3, 0);


    // Wall ki 3 cells
    backWallTilemap.SetTile(
        wallCell1,
        topInnerWallTile
    );

    backWallTilemap.SetTile(
        wallCell2,
        topInnerWallTile
    );

    backWallTilemap.SetTile(
        wallCell3,
        topWallTile
    );
}

            // BOTTOM
            if (!floorTilemap.HasTile(
                floorCell + Vector3Int.down))
            {
                frontWallTilemap.SetTile(
                    floorCell + Vector3Int.down,
                    bottomWallTile
                );
                // Boundary wall
                boundaryWallTilemap.SetTile(
                    floorCell + Vector3Int.down,
                    boundaryWallTile
                );
            }
        }
       
// =====================================================
// PHASE 1B
// =====================================================

foreach (Vector3Int floorCell in bounds.allPositionsWithin)
{
    if (!floorTilemap.HasTile(floorCell))
        continue;

    // LEFT
    Vector3Int leftCell = floorCell + Vector3Int.left;

    if (!floorTilemap.HasTile(leftCell) &&
        !frontWallTilemap.HasTile(leftCell))
    {
        backWallTilemap.SetTile(leftCell, leftWallTile);
    }

    // RIGHT
    Vector3Int rightCell = floorCell + Vector3Int.right;

    if (!floorTilemap.HasTile(rightCell) &&
        !frontWallTilemap.HasTile(rightCell))
    {
        backWallTilemap.SetTile(rightCell, rightWallTile);
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
                new Vector3Int(-1, 3, 0);

            Vector3Int topRight =
                floorCell +
                new Vector3Int(1, 3, 0);

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
   
    backWallTilemap.SetTile(topLeft, topLeftCorner);

    Vector3Int gapCell1 = floorCell + new Vector3Int(-1, 2, 0);
    Vector3Int gapCell2 = floorCell + new Vector3Int(-1, 1, 0);

    Vector3Int gapCell1Left = gapCell1 + Vector3Int.left;   // khaali check
    bool gapCell1LeftIsEmpty = !floorTilemap.HasTile(gapCell1Left) && !backWallTilemap.HasTile(gapCell1Left);

    if (gapCell1LeftIsEmpty)
    {
        backWallTilemap.SetTile(gapCell1, leftWallTile);
    }

    Vector3Int gapCell2Left = gapCell2 + Vector3Int.left;   // khaali check
    bool leftIsEmpty = !floorTilemap.HasTile(gapCell2Left) && !backWallTilemap.HasTile(gapCell2Left);

    if (leftIsEmpty)
    {
        backWallTilemap.SetTile(gapCell2, leftWallTile);
    }
    

    
 }


     // =================================================
// OUTER TOP RIGHT
// =================================================

if (!floorTilemap.HasTile(up) &&
    !floorTilemap.HasTile(right))
{
    backWallTilemap.SetTile(topRight, topRightCorner);

    Vector3Int gapCell1 = floorCell + new Vector3Int(1, 2, 0);
    Vector3Int gapCell2 = floorCell + new Vector3Int(1, 1, 0);

    Vector3Int gapCell1Right = gapCell1 + Vector3Int.right;   // khaali check
    bool gapCell1RightIsEmpty = !floorTilemap.HasTile(gapCell1Right) && !backWallTilemap.HasTile(gapCell1Right);

    if (gapCell1RightIsEmpty)
    {
        backWallTilemap.SetTile(gapCell1, rightWallTile);
    }

    Vector3Int gapCell2Right = gapCell2 + Vector3Int.right;   // khaali check
    bool rightIsEmpty = !floorTilemap.HasTile(gapCell2Right) && !backWallTilemap.HasTile(gapCell2Right);

    if (rightIsEmpty)
    {
        backWallTilemap.SetTile(gapCell2, rightWallTile);
    }
}


            // =================================================
            // OUTER BOTTOM LEFT
            // =================================================

            if (!floorTilemap.HasTile(down) &&
                !floorTilemap.HasTile(left))
            {
                backWallTilemap.SetTile(
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
                backWallTilemap.SetTile(
                    bottomRight,
                    bottomRightCorner
                );
            }
// =================================================
// INNER TOP LEFT
// =================================================

Vector3Int diagTopLeft = floorCell + new Vector3Int(-1, 1, 0);

if (floorTilemap.HasTile(up) &&
    floorTilemap.HasTile(left) &&
    !floorTilemap.HasTile(diagTopLeft))
{
    Vector3Int cornerCell = floorCell + new Vector3Int(-1, 3, 0);
    Vector3Int innerWallCell1 = floorCell + new Vector3Int(-1, 1, 0);
    Vector3Int innerWallCell2 = floorCell + new Vector3Int(-1, 2, 0);

    backWallTilemap.SetTile(cornerCell, innerTopLeftCorner);
    backWallTilemap.SetTile(innerWallCell1, topInnerWallTile);
    backWallTilemap.SetTile(innerWallCell2, topInnerWallTile);
}


// =================================================
// INNER TOP RIGHT
// =================================================

Vector3Int diagTopRight = floorCell + new Vector3Int(1, 1, 0);

if (floorTilemap.HasTile(up) &&
    floorTilemap.HasTile(right) &&
    !floorTilemap.HasTile(diagTopRight))
{
    Vector3Int cornerCell = floorCell + new Vector3Int(1, 3, 0);
    Vector3Int innerWallCell1 = floorCell + new Vector3Int(1, 1, 0);
    Vector3Int innerWallCell2 = floorCell + new Vector3Int(1, 2, 0);

    backWallTilemap.SetTile(cornerCell, innerTopRightCorner);
    backWallTilemap.SetTile(innerWallCell1, topInnerWallTile);
    backWallTilemap.SetTile(innerWallCell2, topInnerWallTile);
}


            // =================================================
            // INNER BOTTOM LEFT
            // =================================================

            if (floorTilemap.HasTile(down) &&
                floorTilemap.HasTile(left) &&
                !floorTilemap.HasTile(bottomLeft))
            {
                frontWallTilemap.SetTile(bottomLeft, innerBottomLeftCorner
                    );
            }


            // =================================================
            // INNER BOTTOM RIGHT
            // =================================================

            if (floorTilemap.HasTile(down) &&
                floorTilemap.HasTile(right) &&
                !floorTilemap.HasTile(bottomRight))
            {
                frontWallTilemap.SetTile(
                    bottomRight,
                    innerBottomRightCorner
                );
            }
        }


    }

}