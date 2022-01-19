using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;


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
