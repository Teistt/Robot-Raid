using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }


    public List<UnitsSelection> SELECTED_UNITS = new List<UnitsSelection>();
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;
    private Vector3 targetMovePosition;


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
            foreach (var unit in SELECTED_UNITS)
            {
                unit.gameObject.GetComponent<UnitMovement>().SetDestination(targetMovePosition);
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