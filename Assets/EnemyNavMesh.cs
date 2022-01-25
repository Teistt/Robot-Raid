using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private float walkSpeed = 3f;
    private float detectionRange = 19f;
    private bool facingRight = true;
    private float attackRange = 1f;
    private bool _isNavMesh = false;
    private bool _isKnockdBack = false;

    private GameObject targetUnit;
    private EnemyAttack attackScript;
    private Rigidbody2D rb;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

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
        if (collision.gameObject.layer == 7)
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            rb.velocity = new Vector2(0, 0);
            rb.isKinematic = true;
            _isNavMesh = true;
        }
    }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        attackScript = gameObject.GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>();
    }


    void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        //FindNearestUnit();
        //Permet de lancer la fonction FindNearestUnit toutes les 0.2s à partir de 0s
        //Game at 30fps so every 6 frames
        //InvokeRepeating("FindNearestUnit", 0f, 0.2f);
    }

    // Update is called once per frame
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

    public void Knocked(Vector3 knockback)
    {
        _isKnockdBack = true;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    void FindNearestUnit()
    {
        LayerMask mask = LayerMask.GetMask("Unit");
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
        }
    }

    void CanAttack()
    {
        if (agent.pathPending)
        {
            return;
        }

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
        Vector3 actualDir = targetUnit.transform.position - transform.position;
        actualDir.x = Mathf.Clamp(actualDir.x, -1f, 1f);
        actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

        rb.velocity = actualDir * walkSpeed;
    }
}
