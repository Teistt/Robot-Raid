using System;
using UnityEngine;

public class UnitLife : MonoBehaviour
{
    [SerializeField] private int life = 4;

    public static event Action OnUnitDie;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit(int damage)
    {
        life -= damage;
        Debug.Log("unit hit");
        if (life <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        OnUnitDie?.Invoke();
        //Destroy(gameObject);
    }
}
