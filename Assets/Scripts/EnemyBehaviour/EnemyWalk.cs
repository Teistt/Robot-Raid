using UnityEngine;

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


    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        attackScript = gameObject.GetComponent<EnemyAttack>();
    }

    void Start()
    {
        FindNearestUnit();
        //Permet de lancer la fonction FindNearestUnit toutes les 0.5s à partir de 0s
        InvokeRepeating("FindNearestUnit", 0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (targetUnit == null)
        {
            Debug.LogError("no target unit found!");
            return;
        }

        LookSide();
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
            //transform.Translate(actualDir.normalized * walkSpeed * Time.deltaTime, Space.World);
            //rb.MovePosition(walkDestination);
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
}
