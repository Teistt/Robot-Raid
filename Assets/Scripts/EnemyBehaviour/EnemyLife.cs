using System;
using UnityEngine;

public class EnemyLife : LifeManager
{
    [SerializeField] private int reward = 10;

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


    protected override void AdditionalHit()
    {
        if(m_navMesh != null)
        {
            m_navMesh.SetSlow();
        }
    }

    protected override void Die()
    {
        OnEnemyDie?.Invoke(transform.position, reward);

        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }

        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        
        //TODO: OBJECT POOLING
        Destroy(gameObject);
    }
}
