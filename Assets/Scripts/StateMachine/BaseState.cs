using UnityEngine;

public abstract class BaseState
{
    protected EnemyBehaviourStateMachine activeSM;
    public abstract void EnterState(EnemyBehaviourStateMachine activeSM);
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void KnockbackHandler(Vector3 knockback);
    public abstract bool SetTarget(GameObject target);
    public abstract void OnTriggerExit2D(Collider2D collision);
}
