using System;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] private int life = 4;
    [SerializeField] private int reward = 10;
    [SerializeField] private GameObject deadSprite;
    private bool _isDead = false;
    private EnemyNavMesh enemyNav;

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
        enemyNav = GetComponent<EnemyNavMesh>();
    }

    private void Update()
    {
        if (!_isDead)
        {
            return;
        }
        Die();
    }

    public void GetHit(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            _isDead = true;
            OnEnemyDie?.Invoke(transform.position, reward);
            if (deadSprite != null)
            {
                Instantiate(deadSprite, transform.position, Quaternion.identity);
            }
            return;
        }
        enemyNav.SetSlow();
    }

    void Die()
    {
        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        Destroy(gameObject);
    }
}
