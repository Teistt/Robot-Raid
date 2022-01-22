using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackSpeed=1f;
    [SerializeField] private int damage=1;
    [SerializeField] private float fireRate = 1f;
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
            unitLife.GetHit(damage);
            fireCtdw = 1 / fireRate;
            //firerate correspond à nb coup/s; donc le cooldown est l'inverse
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
