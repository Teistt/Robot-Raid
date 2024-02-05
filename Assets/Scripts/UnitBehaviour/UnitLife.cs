using System;
using UnityEngine;

public class UnitLife : LifeManager
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


    protected override void AdditionalHit()
    {
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
