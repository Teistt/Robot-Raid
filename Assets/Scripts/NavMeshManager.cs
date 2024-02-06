using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] protected float walkSpeed = 3f;

    protected Rigidbody2D m_rb;
    protected NavMeshAgent m_agent;
    protected bool _isKnockdBack = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_rb = gameObject.GetComponent<Rigidbody2D>();
        m_rb.gravityScale = 0f;
        m_rb.isKinematic = true;

        m_agent = GetComponent<NavMeshAgent>();
        m_agent.speed = walkSpeed;

        Init();
    }

    protected virtual void Init() { }

    private void Start()
    {
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }
    public virtual void Knocked(Vector3 knockback)
    {
        if (knockback == null || knockback == Vector3.zero)
        { return; }

        _isKnockdBack = true;
        m_agent.enabled = false;
        m_rb.isKinematic = false;
        m_rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    public virtual void SetSlow() { }
}
