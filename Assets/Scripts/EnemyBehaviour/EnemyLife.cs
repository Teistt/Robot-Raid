using System;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 4;
    [SerializeField] private int reward = 10;
    [SerializeField] private GameObject deadSprite;
    private EnemyNavMesh enemyNavigation;


    public static event Action<Vector3,int> OnEnemyDie;

    #region Actions
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
    #endregion

    private void Awake()
    {
        enemyNavigation = GetComponent<EnemyNavMesh>();
    }

    public void GetHit(int damage,Vector3 knockback)
    {
        life -= damage;
        enemyNavigation.Knocked(knockback);

        if (life <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        OnEnemyDie?.Invoke(transform.position, reward);
        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
