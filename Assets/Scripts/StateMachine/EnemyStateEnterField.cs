using UnityEngine;

public class EnemyStateEnterField : BaseState
{
    [SerializeField] private float moveSpeed = .5f;
    [SerializeField] private int boundariesLayerID = 9;

    private Vector3 targetPos;

    public override void EnterState(EnemyBehaviourStateMachine sm)
    {
        activeSM = sm;
    }

    public override void UpdateState()
    {
        Vector3 actualDir = targetPos - activeSM.transform.position;

        activeSM.transform.Translate(actualDir * moveSpeed * Time.deltaTime);
    }

    public override void ExitState()
    {
    }

    public override void KnockbackHandler(Vector3 knockback) { }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == boundariesLayerID)
        {
            activeSM.SwitchState(activeSM.enemyStateNavMesh);
        }
    }
    public override bool SetTarget(GameObject target)
    {
        //we dont want to use this method here
        return false;
    }
}
