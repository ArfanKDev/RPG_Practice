using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLineController : MonoBehaviour
{
    [Header("Camera Refs")]
    [SerializeField] private Camera mainCamera;


    // =========================================================
    // ZOOM
    // =========================================================

    [Header("Zoom Settings")]
    [SerializeField] private float normalSize = 5f;

    [SerializeField] private float zoomedOutSize = 8f;

    [SerializeField] private float zoomLerpSpeed = 8f;


    // =========================================================
    // PAN
    // =========================================================

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 1f;

    [SerializeField]
    private int panMouseButton = 1;
    // 0 = Left
    // 1 = Right
    // 2 = Middle


    // =========================================================
    // LINE
    // =========================================================

    [Header("Line Draw Settings")]
    [SerializeField] private LineRenderer linePrefab;

    [SerializeField]
    private int leftClickButton = 0;
    // 0 = Left


    // =========================================================
    // OFFSET
    // =========================================================

    [Header("Line Mouse Offset")]
    [SerializeField] private float lineOffsetX = 0f;

    [SerializeField] private float lineOffsetY = 0f;

    [SerializeField] private float lineOffsetZ = 0f;


    // =========================================================
    // FLOOR
    // =========================================================

    [Header("Floor Placement")]
    [SerializeField] private FloorPlacement floorPlacement;


    // =========================================================
    // DRAW SETTINGS
    // =========================================================

    [Header("Draw Settings")]

    [Tooltip("Chhoti value = zyada accurate path")]
    [SerializeField] private float pointDistance = 0.1f;


    // =========================================================
    // RUNTIME
    // =========================================================

    private bool isCameraMode = false;

    private Vector3 lastMousePos;

    private bool isPanning = false;

    private LineRenderer currentLine;

    private bool isDrawingLine = false;

    private List<Vector3> drawnPoints =
        new List<Vector3>();


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!isCameraMode)
            return;


        HandleCameraPan();

        HandleLineDrawing();
    }


    // =========================================================
    // CAMERA MODE
    // =========================================================

    public void SetCameraMode(bool enabled)
    {
        isCameraMode = enabled;


        if (isCameraMode)
        {
            StopAllCoroutines();


            if (mainCamera != null)
            {
                StartCoroutine(
                    ZoomTo(zoomedOutSize)
                );
            }
        }
        else
        {
            StopDrawing();

            StopAllCoroutines();


            if (mainCamera != null)
            {
                StartCoroutine(
                    ZoomTo(normalSize)
                );
            }


            isPanning = false;
        }
    }


    // =========================================================
    // ZOOM
    // =========================================================

    private IEnumerator ZoomTo(float targetSize)
    {
        if (mainCamera == null)
            yield break;


        while (
            Mathf.Abs(
                mainCamera.orthographicSize -
                targetSize
            ) > 0.01f)
        {
            mainCamera.orthographicSize =
                Mathf.Lerp(
                    mainCamera.orthographicSize,
                    targetSize,
                    Time.deltaTime *
                    zoomLerpSpeed
                );


            yield return null;
        }


        mainCamera.orthographicSize =
            targetSize;
    }


    // =========================================================
    // CAMERA PAN
    // =========================================================

    private void HandleCameraPan()
    {
        if (Input.GetMouseButtonDown(
            panMouseButton))
        {
            isPanning = true;

            lastMousePos =
                Input.mousePosition;
        }


        if (Input.GetMouseButtonUp(
            panMouseButton))
        {
            isPanning = false;
        }


        if (!isPanning)
            return;


        Vector3 delta =
            mainCamera.ScreenToViewportPoint(
                Input.mousePosition -
                lastMousePos
            );


        Vector3 move =
            new Vector3(
                delta.x *
                panSpeed *
                mainCamera.orthographicSize *
                2f,

                delta.y *
                panSpeed *
                mainCamera.orthographicSize *
                2f,

                0f
            );


        mainCamera.transform.position -=
            move;


        lastMousePos =
            Input.mousePosition;
    }


    // =========================================================
    // LINE DRAWING
    // =========================================================

    private void HandleLineDrawing()
    {
        if (Input.GetMouseButtonDown(
            leftClickButton))
        {
            StartDrawing();
        }


        if (
            isDrawingLine &&
            Input.GetMouseButton(
                leftClickButton))
        {
            UpdateDrawing();
        }


        if (Input.GetMouseButtonUp(
            leftClickButton))
        {
            FinishDrawing();
        }
    }


    // =========================================================
    // START DRAWING
    // =========================================================

    private void StartDrawing()
    {
        if (linePrefab == null)
        {
            Debug.LogWarning(
                "Line Prefab assign nahi kiya!"
            );

            return;
        }


        if (floorPlacement == null)
        {
            Debug.LogWarning(
                "Floor Placement assign nahi kiya!"
            );

            return;
        }


        isDrawingLine = true;


        drawnPoints.Clear();


        Vector3 startPosition =
            GetMouseWorldPos();


        drawnPoints.Add(
            startPosition
        );


        currentLine =
            Instantiate(linePrefab);


        currentLine.positionCount = 1;


        currentLine.SetPosition(
            0,
            startPosition
        );
    }


    // =========================================================
    // UPDATE DRAWING
    // =========================================================

    private void UpdateDrawing()
    {
        if (currentLine == null)
            return;


        Vector3 currentPosition =
            GetMouseWorldPos();


        Vector3 lastPoint =
            drawnPoints[
                drawnPoints.Count - 1
            ];


        float distance =
            Vector3.Distance(
                lastPoint,
                currentPosition
            );


        if (distance >= pointDistance)
        {
            drawnPoints.Add(
                currentPosition
            );


            currentLine.positionCount =
                drawnPoints.Count;


            currentLine.SetPosition(
                drawnPoints.Count - 1,
                currentPosition
            );
        }
    }


    // =========================================================
    // FINISH DRAWING
    // =========================================================

    private void FinishDrawing()
    {
        if (!isDrawingLine)
            return;


        isDrawingLine = false;


        // Final mouse position
        Vector3 finalPosition =
            GetMouseWorldPos();


        Vector3 lastPoint =
            drawnPoints[
                drawnPoints.Count - 1
            ];


        if (
            Vector3.Distance(
                lastPoint,
                finalPosition
            ) > 0.01f)
        {
            drawnPoints.Add(
                finalPosition
            );
        }


        // -----------------------------------------
        // PATH → FLOOR
        // -----------------------------------------

        ConvertPathToFloor();


        // -----------------------------------------
        // LINE DESTROY
        // -----------------------------------------

        if (currentLine != null)
        {
            Destroy(
                currentLine.gameObject
            );
        }


        currentLine = null;


        drawnPoints.Clear();
    }


    // =========================================================
    // CONVERT DRAWN PATH TO FLOOR
    // =========================================================

    private void ConvertPathToFloor()
    {
        if (floorPlacement == null)
            return;


        if (drawnPoints.Count == 0)
            return;


        // -----------------------------------------
        // Har 2 consecutive points ke beech
        // grid cells fill karo
        // -----------------------------------------

        for (int i = 0; i < drawnPoints.Count; i++)
        {
            Vector3 start =
                drawnPoints[i];


            // Current point
            PlaceFloorAtWorldPosition(
                start
            );


            // Next point available hai
            if (i < drawnPoints.Count - 1)
            {
                Vector3 end =
                    drawnPoints[i + 1];


                FillBetweenPoints(
                    start,
                    end
                );
            }
        }


        // -----------------------------------------
        // WALLS REBUILD
        // -----------------------------------------

        floorPlacement.RebuildWalls();
    }


    // =========================================================
    // PLACE FLOOR AT ONE POSITION
    // =========================================================

    private void PlaceFloorAtWorldPosition(
        Vector3 worldPosition)
    {
        Vector3Int cell =
            WorldToCell(worldPosition);


        floorPlacement.PlaceFloorAtCell(
            cell
        );
    }


    // =========================================================
    // FILL BETWEEN TWO POINTS
    // =========================================================

    private void FillBetweenPoints(
        Vector3 start,
        Vector3 end)
    {
        Vector3 direction =
            end - start;


        float distance =
            direction.magnitude;


        if (distance <= 0.01f)
            return;


        // Grid ke hisaab se enough steps
        int steps =
            Mathf.CeilToInt(
                distance * 2f
            );


        steps =
            Mathf.Max(
                steps,
                1
            );


        for (int i = 1; i <= steps; i++)
        {
            float t =
                (float)i / steps;


            Vector3 position =
                Vector3.Lerp(
                    start,
                    end,
                    t
                );


            PlaceFloorAtWorldPosition(
                position
            );
        }
    }


    // =========================================================
    // WORLD → GRID CELL
    // =========================================================

    private Vector3Int WorldToCell(
        Vector3 worldPosition)
    {
        // FloorPlacement ke Grid ko directly
        // use karna possible nahi hai kyun ke
        // Grid private hai.
        //
        // Isliye line controller ke paas
        // apna Grid reference hai.
        
        return Vector3Int.RoundToInt(
            worldPosition
        );
    }


    // =========================================================
    // STOP DRAWING
    // =========================================================

    private void StopDrawing()
    {
        isDrawingLine = false;


        if (currentLine != null)
        {
            Destroy(
                currentLine.gameObject
            );
        }


        currentLine = null;


        drawnPoints.Clear();
    }


    // =========================================================
    // GET MOUSE WORLD POSITION
    // =========================================================

    private Vector3 GetMouseWorldPos()
    {
        if (mainCamera == null)
            return Vector3.zero;


        Vector3 mousePosition =
            Input.mousePosition;


        mousePosition.z =
            Mathf.Abs(
                mainCamera.transform.position.z
            );


        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                mousePosition
            );


        worldPosition.z = 0f;


        // Inspector offset
        worldPosition.x +=
            lineOffsetX;


        worldPosition.y +=
            lineOffsetY;


        worldPosition.z +=
            lineOffsetZ;


        return worldPosition;
    }
}