using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomController : MonoBehaviour
{
  public Tilemap floorTilemaps;
  public Tilemap wallTilemaps;

  public TileBase floorTiles;

  public TileBase topWallTiles;
  public TileBase leftWallTiles;
  public TileBase rightWallTiles;
  public TileBase bottomWallTiles;
  public TileBase topLeftCornerWalltiles;
  public TileBase topRightCornerWalltiles;
  public TileBase bottomLeftCornerWalltiles;
  public TileBase bottomRightCornerWalltiles;

  public int roomWidth;
  public int roomHeight;


  public Vector3Int roomOrigin;


  private HashSet<Vector3Int> floorCells = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> wallCells = new HashSet<Vector3Int>();

void Start()
    {

   

        GenerateFloor();
        GenerateWalls();
    }


  void GenerateFloor()
    {
        floorTilemaps.ClearAllTiles();
        floorCells.Clear();

        for (int y = 0; y < roomHeight; y++)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                Vector3Int cellPosition = roomOrigin + new Vector3Int(x, y, 0);

                floorCells.Add(cellPosition);
                floorTilemaps.SetTile(cellPosition, floorTiles);
            }
        }
    }

    void GenerateWalls()
    {
        wallTilemaps.ClearAllTiles();

        foreach (Vector3Int floorCell in floorCells)
        {
            Vector3Int topCell = floorCell + Vector3Int.up;
            Vector3Int bottomCell = floorCell + Vector3Int.down;
            Vector3Int leftCell = floorCell + Vector3Int.left;
            Vector3Int rightCell = floorCell + Vector3Int.right;


              if (!floorCells.Contains(topCell))
                wallTilemaps.SetTile(topCell, topWallTiles);

             if (!floorCells.Contains(bottomCell))
               wallTilemaps.SetTile(bottomCell, bottomWallTiles);

               if (!floorCells.Contains(leftCell))
               wallTilemaps.SetTile(leftCell, leftWallTiles);

               if (!floorCells.Contains(rightCell))
               wallTilemaps.SetTile(rightCell, rightWallTiles);

        
 
        }
    }
}
