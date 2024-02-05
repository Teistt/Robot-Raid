using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    public virtual void Knocked(Vector3 knockback) { }

    public virtual void SetSlow() { }
}
