using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    [SerializeField] private Texture2D mouseCursor;

    public List<UnitsSelection> SELECTED_UNITS = new List<UnitsSelection>();
    public List<UnitsSelection> PRESENT_UNITS = new List<UnitsSelection>();

    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;
    private Vector3 targetMovePosition;

    [SerializeField] private float deltaX = 0.6f;
    [SerializeField] private float deltaY = 0.6f;

    private void Awake()
    {
        if(Instance!=null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Physics2D.gravity = Vector2.zero;
        Physics.gravity = Vector3.zero;
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);


        GameObject[] initialUnits = GameObject.FindGameObjectsWithTag("Unit");
        foreach(var unit in initialUnits)
        {
            PRESENT_UNITS.Add(unit.GetComponent<UnitsSelection>());
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingMouseBox = false;
        }


        if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
        {
            _SelectUnitsInDraggingBox();
        }

        if (Input.GetMouseButtonDown(1))
        {
            targetMovePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Position returned is based on camera height
            targetMovePosition.z -= Camera.main.transform.position.z;

            if (SELECTED_UNITS.Count == 1)
            {
                SELECTED_UNITS[0].gameObject.GetComponent<UnitNavMeshMovement>().SetDestination(targetMovePosition);
            }
            else
            {
                Vector3 gridPos;
                int countX = 0;
                int countY = 0;
                int squadSizeXMax = (int) Mathf.Sqrt((float)SELECTED_UNITS.Count);

                foreach (var unit in SELECTED_UNITS)
                {
                    gridPos = targetMovePosition+ new Vector3(countX* deltaX, countY* deltaY, 0);
                    unit.gameObject.GetComponent<UnitNavMeshMovement>().SetDestination(gridPos);
                    countX++;

                    if (countX >= squadSizeXMax)
                    {
                        countX = 0;
                        countY++;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("All units selected");
            SELECTED_UNITS.Clear();
            foreach (UnitsSelection unit in PRESENT_UNITS)
            {
                unit.GetComponent<UnitsSelection>().Select();
            }
        }
    }


    private void _SelectUnitsInDraggingBox()
    {
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            _dragStartPosition,
            Input.mousePosition
        );
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("Unit");
        bool inBounds;
        foreach (GameObject unit in selectableUnits)
        {
            inBounds = selectionBounds.Contains(
                Camera.main.WorldToViewportPoint(unit.transform.position)
            );
            if (inBounds)
                unit.GetComponent<UnitsSelection>().Select();
            else
                unit.GetComponent<UnitsSelection>().Deselect();
        }
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            //Create a rect from mouse start postion to mouse actual position
            var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1f, new Color(0.5f, 1f, 0.4f));
        }
    }
}