using UnityEngine;

public class EnemyStateIdle : BaseState
{
    public override void EnterState(EnemyBehaviourStateMachine sm)
    {
        activeSM = sm;
        Debug.Log("Enemy now Idleing...");
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
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
