using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitLife : MonoBehaviour
{
    [SerializeField] private int maxLife = 4;
    [SerializeField] private int life;
    [SerializeField] public Slider lifeSlider;
    [SerializeField] private GameObject deadSprite;
    [SerializeField] private GameObject hitVFX;
    private UnitNavMeshMovement unitNavigation;


    private Animator anim;


    public static event Action<GameObject> OnUnitDieGO;
    public static event Action<GameObject> OnUnitSpawn;

    void Start()
    {
        life = maxLife;
        OnUnitSpawn?.Invoke(gameObject);
        lifeSlider.maxValue = maxLife;
        lifeSlider.value = life;

        unitNavigation = GetComponent<UnitNavMeshMovement>();
        MouseManager.Instance.PRESENT_UNITS.Add(GetComponent<UnitsSelection>());
        GetComponent<UnitsSelection>().Select();
        anim = GetComponent<Animator>();
    }


    public void GetHeal(int amount)
    {
        if (life < maxLife)
        {
            //Instantiate(healVFX, transform);
            life += amount;
        }
        
        lifeSlider.value = life;
        if(life==maxLife)
            lifeSlider.gameObject.SetActive(false);
    }

    public void GetHit(int damage, Vector3 knockback)
    {

        lifeSlider.gameObject.SetActive(true);

        Instantiate(hitVFX, transform);
        life -= damage;
        unitNavigation.Knocked(knockback);

        lifeSlider.value = life;
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
        OnUnitDieGO?.Invoke(gameObject);
        GetComponent<UnitsSelection>().Deselect();
        MouseManager.Instance.PRESENT_UNITS.Remove(this.GetComponent<UnitsSelection>());
        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
