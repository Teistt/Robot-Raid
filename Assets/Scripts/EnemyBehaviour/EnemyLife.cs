using System;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 4;
    [SerializeField] private int reward = 10;
    [SerializeField] private GameObject deadSprite;
    //public int enemyCost;


    public static event Action<Vector3,int> OnEnemyDie;


    private void OnEnable()
    {
        GameManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= OnGameOver;
    }

    void OnGameOver()
    {
        this.enabled = false;
    }

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
        OnEnemyDie?.Invoke(transform.position, reward);
        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
