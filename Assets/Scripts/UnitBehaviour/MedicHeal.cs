using UnityEngine;

public class MedicHeal : MonoBehaviour
{
    [SerializeField] private float healRate = .25f;
    [SerializeField] private int healAmount=1;
    [SerializeField] private float healRadius = 1f;
    [SerializeField] private GameObject healCircle;
    private float fireCtdw = 0f; //cooldown
    private bool _isMoving = false;


    private void OnEnable()
    {
        UnitMovement.OnMoving += OnUnitMoving;
    }

    private void OnDisable()
    {
        UnitMovement.OnMoving -= OnUnitMoving;
    }

    // Start is called before the first frame update
    void Start()
    {
        healCircle.gameObject.transform.localScale = healCircle.gameObject.transform.localScale * healRadius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fireCtdw <= 0f)
        {
            if (_isMoving)
            {
                return;
            }
            HealUnit();
        }
        else
        {
            fireCtdw -= Time.deltaTime;
        }
    }

    void HealUnit()
    {
        LayerMask mask = LayerMask.GetMask("Unit");
        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, healRadius, mask);
        foreach (var item in en)
        {
            item.GetComponent<UnitLife>().GetHeal(healAmount);
        }


        fireCtdw = 1 / healRate;
        //firerate correspond à nb coup/s; donc le cooldown est l'inverse
        //aka fireRate=2 donc fireCtdw=1/2=.5s
    }


    void OnUnitMoving(bool i)
    {
        _isMoving = i;
        healCircle.SetActive(!_isMoving);
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
