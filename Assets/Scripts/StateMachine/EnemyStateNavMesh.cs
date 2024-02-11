using UnityEngine;
using UnityEngine.AI;

public class EnemyStateNavMesh : BaseState
{

    #region Actions

    void OnGameOver()
    {
        activeSM.SwitchState(activeSM.enemyStateIdle);
    }
    #endregion


    public override void EnterState(EnemyBehaviourStateMachine sm)
    {
        //Debug.Log("NOW NAVMESH");

        GameManager.OnGameOver += OnGameOver;

        activeSM = sm;
        //Debug.Log("go name " + activeSM.name);


        activeSM.m_rb.gravityScale = 0f;
        activeSM.m_rb.isKinematic = true;
        
        activeSM.m_agent.speed = activeSM.moveSpeed;
        activeSM.m_agent.updateRotation = false;
        activeSM.m_agent.updateUpAxis = false;
        activeSM.m_agent.enabled = true;
    }

    public override void ExitState()
    {
        GameManager.OnGameOver -= OnGameOver;
    }

    public override void UpdateState()
    {
        //Debug.Log("navmeshing stuff");

        if (activeSM.targetUnit == null)
        {
            //TODO: handling no target found case
            return;
        }

        activeSM.m_agent.SetDestination(activeSM.targetUnit.transform.position);

        CanAttack();
    }

    void CanAttack()
    {
        if (activeSM.m_agent.pathPending)
        {
            return;
        }

        //Debug.Log(agent.remainingDistance);
        if (activeSM.m_agent.remainingDistance <= activeSM.attackRange)
        {
            activeSM.SwitchState(activeSM.enemyStateAttack,activeSM.targetUnit);
        }
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

    public override void OnTriggerExit2D(Collider2D collision)
    {
    }

    public override bool SetTarget(GameObject target)
    {
        //we dont want to use this method here
        return false;
    }
}
