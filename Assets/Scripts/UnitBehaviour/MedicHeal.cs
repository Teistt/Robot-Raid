using UnityEngine;

public class MedicHeal : MonoBehaviour
{
    //Heal rate, .25 means 1 heal every 4 seconds
    [SerializeField] private float healRate = .25f;
    [SerializeField] private int healAmount=1;
    //Heal zone
    [SerializeField] private float healRadius = 1f;
    [SerializeField] private GameObject healCircle;
    private float healCtwn = 0f; //cooldown
    //Only heal when not moving
    private bool _isMoving = false;

    #region Actions
    private void OnEnable()
    {
        UnitMovement.OnMoving += OnUnitMoving;
    }

    private void OnDisable()
    {
        UnitMovement.OnMoving -= OnUnitMoving;
    }
    #endregion


    void OnUnitMoving(bool i)
    {
        //Actions called by UnitMovement class
        //If we are moving we are not going to heal units
        //so we also hide heal circle
        _isMoving = i;
        healCircle.SetActive(!_isMoving);
    }


    void Start()
    {
        //Adapt healCircle sprite to heal zone radius
        healCircle.gameObject.transform.localScale = healCircle.gameObject.transform.localScale * healRadius;
    }


    void FixedUpdate()
    {
        //If the cooldown is reached, whe checked if we are moving
        //If not, we can heal units around us

        //This way, healCtdw is updated even if we are moving
        if (healCtwn <= 0f)
        {
            if (_isMoving)
            {
                return;
            }
            HealUnit();

            healCtwn = 1 / healRate;
        }
        //If countdown not finished, we update it since the last frame time
        else
        {
            healCtwn -= Time.deltaTime;
        }
    }

    void HealUnit()
    {
        //Physic raycast to get all GO with "Unit" mask and in radius healRadius in en Collider2D array
        LayerMask mask = LayerMask.GetMask("Unit");
        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, healRadius, mask);
        foreach (var item in en)
        {
            //GetHeal() Method of each unit's inside the healRadius is called
            item.GetComponent<UnitLife>().GetHeal(healAmount);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
