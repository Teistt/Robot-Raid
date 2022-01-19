using UnityEngine;

public class UnitsSelection : MonoBehaviour
{
    [SerializeField] private GameObject selectionCircle;

    public void Select()
    {
        if (MouseManager.Instance.SELECTED_UNITS.Contains(this)) return;
        MouseManager.Instance.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
    }

    public void Deselect()
    {
        if (!MouseManager.Instance.SELECTED_UNITS.Contains(this)) return;
        MouseManager.Instance.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
    }
}
