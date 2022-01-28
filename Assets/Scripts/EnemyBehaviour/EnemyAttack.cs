using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //[SerializeField] private float attackSpeed=1f;
    [SerializeField] private int damage=1;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float knockbackForce = 1000f;
    private float fireCtdw = 0f; //cooldown

    private bool isAttacking = false;

    private Animator anim;

    private GameObject targetUnit;
    private UnitLife unitLife;


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

    void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        if (isAttacking)
        {
            Attack();
        }
    }

    private void Attack()
    {

        if (targetUnit == null)
        {
            return;
        }


        if (fireCtdw <= 0f)
        {
            Vector3 knockback = ((unitLife.transform.position - transform.position).normalized) * knockbackForce;
            unitLife.GetHit(damage,knockback);
            fireCtdw = 1 / attackSpeed;
            //firerate correspond � nb coup/s; donc le cooldown est l'inverse
            //aka fireRate=2 donc fireCtdw=1/2=.5s
        }

        fireCtdw -= Time.deltaTime;
    }

    public void SetAttack(GameObject target)
    {
        isAttacking = true;
        anim.SetBool("_isAttak", true);
        targetUnit = target;
        unitLife = targetUnit.GetComponent<UnitLife>();
    }

    public void StopAttack()
    {
        isAttacking = false;
        anim.SetBool("_isAttak", false);
    }
}
