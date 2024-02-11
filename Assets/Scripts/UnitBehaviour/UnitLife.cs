using System;
using UnityEngine;

public class UnitLife : LifeManager, IGetHeal
{
    public static event Action<GameObject> OnUnitDieGO;
    public static event Action<GameObject> OnUnitSpawn;

    protected override void Init()
    {
        OnUnitSpawn?.Invoke(gameObject);
    }

    private void Start()
    {
        MouseManager.Instance.PRESENT_UNITS.Add(GetComponent<UnitsSelection>());
        GetComponent<UnitsSelection>().Select();
    }

    public void GetHeal(int amount)
    {
        if (life < maxLife)
        {
            //Instantiate(healVFX, transform);
            life += amount;
        }
        UpdateSlider();
    }

    protected override void AdditionalHit()
    {
        Debug.Log(gameObject.name + " is hit");
        if (m_anim != null)
        {
            m_anim.SetTrigger("_getHit");
        }
    }

    protected override void Die()
    {
        OnUnitDieGO?.Invoke(gameObject);

        if (deadSprite != null)
        {
            Instantiate(deadSprite, transform.position, Quaternion.identity);
        }

        GetComponent<UnitsSelection>().Deselect();
        MouseManager.Instance.PRESENT_UNITS.Remove(this.GetComponent<UnitsSelection>());

        Destroy(gameObject);
    }
}
