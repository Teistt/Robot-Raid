using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{

    [SerializeField] private float walkSpeed = 3f;
    private float detectionRange = 19f;
    private float attackRange = 1f;
    private Vector3 actualDir;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private GameObject targetUnit;
    private EnemyAttack attackScript;
    private bool _isBlocked = false;


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
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        attackScript = gameObject.GetComponent<EnemyAttack>();
    }

    void Start()
    {
        FindNearestUnit();
        //Permet de lancer la fonction FindNearestUnit toutes les 0.2s à partir de 0s
        //Game at 30fps so every 6 frames
        InvokeRepeating("FindNearestUnit", 0f, 0.2f);
    }

    void FixedUpdate()
    {
        if (targetUnit == null)
        {
            return;
        }

        LookSide();
        if (_isBlocked)
        {
            return;
        }
        WalkTarget();
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
        if (horizontalValue < 0 && !facingRight || horizontalValue>0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0,180,0);
        }
    }

    void WalkTarget()
    {
        Vector3 walkDestination = targetUnit.transform.position;
        if (Vector3.Distance(transform.position, walkDestination) >= attackRange)
        {
            attackScript.StopAttack();
            actualDir = walkDestination - transform.position;
            actualDir.x = Mathf.Clamp(actualDir.x, -1f, 1f);
            actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

            rb.velocity = actualDir * walkSpeed;
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            attackScript.SetAttack(targetUnit);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("enemy " + gameObject.name + " entered collision");
        if (collision.gameObject.layer == 7)
        {
            _isBlocked = true;
            Vector3 t = transform.position - collision.transform.position;
            MinMax(t.x);
            MinMax(t.y);
            if (t.x == 0) t.x = 1f;
            if (t.y == 0) t.y = 1f;
            rb.velocity = t * walkSpeed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger=false;
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            gameObject.GetComponent<EnemyNavMesh>().enabled = true;
            this.enabled = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log("enemy " + gameObject.name + " still on collision");
        if (collision.gameObject.layer == 7)
        {
            Vector3 t = transform.position - collision.transform.position;
            MinMax(t.x);
            MinMax(t.y);
            if (t.x == 0) t.x = 1f;
            if (t.y == 0) t.y = 1f;
            rb.velocity = t * walkSpeed;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _isBlocked = false;
        }
    }

    float MinMax(float input, float min=1, float max=1)
    {
        if (input == 0) return 0f;
        if (input > 0) return max;
        else return min;
    }
}
