using System;
using UnityEngine;

public class UnitLife : MonoBehaviour
{
    [SerializeField] private int maxLife = 4;
    [SerializeField] private int life;
    [SerializeField] private GameObject deadSprite;
    [SerializeField] private GameObject hitVFX;
    private UnitNavMeshMovement unitNavigation;


    private Animator anim;


    public static event Action<GameObject> OnUnitDie;
    public static event Action<GameObject> OnUnitSpawn;

    void Awake()
    {
        life = maxLife;
        OnUnitSpawn?.Invoke(gameObject);
        anim = GetComponent<Animator>();
        unitNavigation = GetComponent<UnitNavMeshMovement>();
    }


    public void GetHeal(int amount)
    {
        if (life < maxLife)
        {
            //Instantiate(healVFX, transform);
            life += amount;
        }
    }

    public void GetHit(int damage, Vector3 knockback)
    {
        Instantiate(hitVFX, transform);
        life -= damage;
        unitNavigation.Knocked(knockback);

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
