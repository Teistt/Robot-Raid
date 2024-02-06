using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavMesh : NavMeshManager
{
    [SerializeField] private float slowRate = .5f;
    [SerializeField] private float slowedTime = 2f;
    [SerializeField] private int obstacleLayerID=7;
    [SerializeField] private string unitLayerName = "Unit";
    [SerializeField] private int enemyLayerID = 6;
    //[SerializeField] private int unitLayerID = 3;

    [SerializeField] private int searchNearestUnitRate = 4;
    private int snuRateCnt;
    private float detectionRange = 25f;
    private bool facingRight = true;
    [SerializeField] private float attackRange = 1f;
    private bool _isNavMesh = false;
    

    private GameObject targetUnit;
    private EnemyAttack m_attackScript;

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


    //when enemies start, they are on a wall. they move until collider trigger exit, then we activate navmesh
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == obstacleLayerID)
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            m_rb.velocity = new Vector2(0, 0);

            _isNavMesh = true;
            gameObject.layer =enemyLayerID;

            //Debug.Log("navmesh enable");
        }
    }

    protected override void Init()
    {
        //initVelocity = m_agent.velocity;

        m_attackScript = gameObject.GetComponent<EnemyAttack>();
        mask = LayerMask.GetMask(unitLayerName);

        //at init, we want first fixedupdate to search nearest unit
        snuRateCnt = searchNearestUnitRate;
    }

    void FixedUpdate()
    {
        //avoiding too much scans for performance
        if(snuRateCnt>=searchNearestUnitRate)
        {
            snuRateCnt = 0;
            FindNearestUnit();
        }
        else
            snuRateCnt++;

        if (_isKnockdBack)
        {
            if (m_rb.velocity.magnitude < 1f)
            {
                m_agent.enabled = true;
                m_rb.isKinematic = true;

                _isKnockdBack = false;
            }
        }

        if (targetUnit == null)
        {
            if (_isNavMesh)
            {
                m_agent.enabled = false;
            }
            return;
        }

        if (_isNavMesh)
        {
            m_agent.enabled = true;
            m_agent.SetDestination(targetUnit.transform.position);
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
        if (m_agent.pathPending)
        {
            return;
        }

        //Debug.Log(agent.remainingDistance);
        if (m_agent.remainingDistance <= attackRange)
        {
            m_attackScript.SetAttack(targetUnit);
        }
        else
        {
            m_attackScript.StopAttack();
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

        m_rb.velocity = actualDir * walkSpeed;
    }

    public override void SetSlow()
    {
        m_agent.speed= walkSpeed * slowRate;
        StartCoroutine(SlowMo());
    }

    IEnumerator SlowMo()
    {
        yield return new WaitForSeconds(slowedTime);

        m_agent.speed = walkSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
