using System;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 4;
    [SerializeField] private int reward = 10;

    public static event Action<int,Vector3> OnEnemyDie;

    
    public void GetHit(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnEnemyDie?.Invoke(reward,transform.position);
        Destroy(gameObject);
    }
}
