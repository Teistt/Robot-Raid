using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : NavMeshManager
{

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float slowRate = .5f;
    [SerializeField] private float slowedTime = 2f;
    [SerializeField] private int obstacleLayerID=7;
    [SerializeField] private string unitLayerName = "Unit";
    [SerializeField] private int enemyLayerID = 6;
    //[SerializeField] private int unitLayerID = 3;

    private float detectionRange = 25f;
    private bool facingRight = true;
    [SerializeField] private float attackRange = 1f;
    private bool _isNavMesh = false;
    private bool _isKnockdBack = false;

    private GameObject targetUnit;
    private EnemyAttack attackScript;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    LayerMask mask;

    #region Actions
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
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        //agent.enabled = false;
        //this.enabled = false;
    }
    #endregion


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == obstacleLayerID)
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            rb.velocity = new Vector2(0, 0);
            rb.isKinematic = true;
            _isNavMesh = true;
            gameObject.layer =enemyLayerID;

            //Debug.Log("navmesh enable");
        }
    }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        attackScript = gameObject.GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>();
        //initVelocity = agent.velocity;
        agent.speed = walkSpeed;
        mask = LayerMask.GetMask(unitLayerName);
    }


    void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    void FixedUpdate()
    {
        FindNearestUnit();

        if (_isKnockdBack)
        {
            if (rb.velocity.magnitude < 1f)
            {
                agent.enabled = true;
                rb.isKinematic = true;

                _isKnockdBack = false;
            }
            else
            {
                return;
            }
        }

        if (targetUnit == null)
        {
            if (_isNavMesh)
            {
                agent.enabled = false;
            }
            return;
        }

        if (_isNavMesh)
        {
            agent.enabled = true;
            agent.SetDestination(targetUnit.transform.position);
            CanAttack();
        }
        else
        {
            WalkTarget();
        }

        LookSide();
    }


    void FindNearestUnit()
    {

        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, detectionRange, mask);

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

    void CanAttack()
    {
        if (agent.pathPending)
        {
            return;
        }

        //Debug.Log(agent.remainingDistance);
        if (agent.remainingDistance <= attackRange)
        {
            attackScript.SetAttack(targetUnit);
        }
        else
        {
            attackScript.StopAttack();
        }
    }


    void WalkTarget()
    {
        if (targetUnit == null)
        {
            FindNearestUnit();
        }

        Vector3 actualDir = targetUnit.transform.position - transform.position;
        actualDir.x = Mathf.Clamp(actualDir.x, -1f, 1f);
        actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

        rb.velocity = actualDir * walkSpeed;
    }

    public override void SetSlow()
    {
        agent.speed= walkSpeed * slowRate;
        StartCoroutine(SlowMo());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    IEnumerator SlowMo()
    {
        yield return new WaitForSeconds(slowedTime);

        agent.speed = walkSpeed;
    }
}
