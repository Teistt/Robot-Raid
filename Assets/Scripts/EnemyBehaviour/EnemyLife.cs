using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 4;

    public void GetHit(int damage)
    {
        life -= damage;
        Debug.Log("hit " + life);
    }
}
