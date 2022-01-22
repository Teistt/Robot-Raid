using System;
using UnityEngine;

public class UnitLife : MonoBehaviour
{
    [SerializeField] private int life = 4;
    [SerializeField] private GameObject deadSprite;
    [SerializeField] private GameObject hitVFX;


    private Animator anim;


    public static event Action<GameObject> OnUnitDie;
    public static event Action<GameObject> OnUnitSpawn;

    void Awake()
    {
        OnUnitSpawn?.Invoke(gameObject);
        anim = GetComponent<Animator>();
    }


    public void GetHit(int damage)
    {
        Instantiate(hitVFX, transform);
        life -= damage;
        if (life <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("_getHit");
        }
    }


    void Die()
    {
        OnUnitDie?.Invoke(gameObject);
        GetComponent<UnitsSelection>().Deselect();
        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
