using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviourStateMachine : MonoBehaviour
{
    LayerMask targetLayerMask;
    [Header("Targetting settings")]
    [SerializeField] private string unitLayerName = "Unit";
    [SerializeField] private float detectionRange = 25f;
    [SerializeField] private int searchNearestUnitRate = 4;


    [field: Header("Movement settings")]
    [field: SerializeField] public float moveSpeed { get; private set; } = 3f;


    [field: Header("Attack settings")]
    public GameObject targetUnit { get; private set; }
    [field: SerializeField] public int damage { get; private set; } = 1;
    [field: SerializeField] public float attackRange { get; private set; } = 1f;
    [field: SerializeField] public float attackSpeed { get; private set; } = 1f;
    [field: SerializeField] public float knockbackForce { get; private set; } = 1000f;
    private int snuRateCnt;


    BaseState currentState;
    public EnemyStateEnterField enemyStateEnterField=new EnemyStateEnterField();
    public EnemyStateNavMesh enemyStateNavMesh =new EnemyStateNavMesh();
    public EnemyStateAttack enemyStateAttack=new EnemyStateAttack();
    public EnemyStateIdle enemyStateIdle=new EnemyStateIdle();


    public Rigidbody2D m_rb { get; private set; }
    public NavMeshAgent m_agent { get; private set; }
    public Animator m_anim { get; private set; }


    private bool facingRight = true;


    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();

        currentState = enemyStateEnterField;

        targetLayerMask = LayerMask.GetMask(unitLayerName);

        //at init, we want first fixedupdate to search nearest unit
        snuRateCnt = searchNearestUnitRate;

        currentState.EnterState(this);
    }

    private void FixedUpdate()
    {
        //avoiding too much scans for performance
        if (snuRateCnt >= searchNearestUnitRate)
        {
            snuRateCnt = 0;
            FindNearestUnit();
        }
        else
            snuRateCnt++;

        if (targetUnit != null)
        {
            LookSide();
        }

        currentState.UpdateState();
    }

    public void SwitchState(BaseState nextState, GameObject targetUnit=null)
    {
        currentState.ExitState();

        currentState= nextState;
        currentState.EnterState(this);
        if (targetUnit != null)
        {
            currentState.SetTarget(targetUnit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentState.OnTriggerExit2D(collision);
    }


    public void KnockbackHandler(Vector3 knockback)
    {
        currentState.KnockbackHandler(knockback);
    }

    void FindNearestUnit()
    {
        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayerMask);

        float nearDist = 1000f;
        if (en.Length == 0)
        {
            targetUnit = null;
        }
        else
        {
            foreach (var item in en)
            {
                float actDistance = Vector2.Distance(item.gameObject.transform.position, transform.position);
                if (actDistance <= nearDist)
                {
                    nearDist = actDistance;
                    targetUnit = item.gameObject;
                }
            }
        }

        //Debug.Log("nearest unit is "+targetUnit.name+" at "+targetUnit.transform.position);
    }

    void LookSide()
    {
        
        float horizontalValue = transform.position.x - targetUnit.transform.position.x;
        if (horizontalValue < 0 && !facingRight || horizontalValue > 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
            GetComponent<EnemyLife>().FlipLifeSliderSprite(facingRight);
        }
    }
}
