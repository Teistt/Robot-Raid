using UnityEngine;

public class EnemyStateAttack : BaseState
{

    private float fireCtdw = 0f; //cooldown

    private UnitLife unitLife;


    #region Actions
    void OnGameOver()
    {
        activeSM.SwitchState(activeSM.enemyStateIdle);
    }
    #endregion


    public override void EnterState(EnemyBehaviourStateMachine sm)
    {
        activeSM = sm;
        GameManager.OnGameOver += OnGameOver;

        Debug.Log("NOW ATTACKING!");
    }

    public override void ExitState()
    {
        GameManager.OnGameOver -= OnGameOver;

        activeSM.m_anim.SetBool("_isAttak", false);
    }

    public override void UpdateState()
    {
        if (activeSM.targetUnit == null)
        {
            Debug.Log("target to attack is null");
            //TODO: handling no target found case
            return;
        }

        CanAttack();

        if (fireCtdw <= 0f)
        {
            Vector3 knockback = ((unitLife.transform.position - activeSM.transform.position).normalized) * activeSM.knockbackForce;
            unitLife.TakeDamage(activeSM.damage, knockback);
            fireCtdw = 1 / activeSM.attackSpeed;
            //firerate correspond à nb coup/s; donc le cooldown est l'inverse
            //aka fireRate=2 donc fireCtdw=1/2=.5s
        }

        fireCtdw -= Time.deltaTime;
    }
    public override void KnockbackHandler(Vector3 knockback)
    {
        if (knockback == null || knockback == Vector3.zero)
        { return; }

        activeSM.m_agent.enabled = false;
        activeSM.m_rb.isKinematic = false;
        activeSM.m_rb.AddForce(knockback, ForceMode2D.Impulse);

        activeSM.m_rb.velocity = Vector2.zero;
        activeSM.m_rb.isKinematic = true;
        activeSM.m_agent.enabled = true;
    }

    public override bool SetTarget(GameObject target)
    {
        unitLife = target.GetComponent<UnitLife>();

        activeSM.m_anim.SetBool("_isAttak", true);
        return true;
    }


    void CanAttack()
    {
        if (activeSM.m_agent.pathPending)
        {
            return;
        }

        //Debug.Log(agent.remainingDistance);
        if (activeSM.m_agent.remainingDistance <=activeSM.attackRange)
        {
            activeSM.SwitchState(activeSM.enemyStateNavMesh, activeSM.targetUnit);
        }
    }



    public override void OnTriggerExit2D(Collider2D collision)
    {
    }
}
