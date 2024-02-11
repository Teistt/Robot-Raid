using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 3f;

    protected Rigidbody2D m_rb;
    protected NavMeshAgent m_agent;

    // Start is called before the first frame update
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.gravityScale = 0f;
        m_rb.isKinematic = true;

        m_agent = GetComponent<NavMeshAgent>();
        m_agent.speed = moveSpeed;

        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;

        Init();
    }

    protected virtual void Init() { }



    public void SetDestination(Vector3 destination)
    {
        //When the player set destination for selected units the Mouse Manager Class
        //call each selected unit's SetDestination Method
        NavMeshPath path = new NavMeshPath();

        if (m_agent.CalculatePath(destination, path))
        {
            m_agent.SetPath(path);
        }

        SetDestinationPostAction();
    }


    public virtual void SetDestinationPostAction() { }


    public virtual void SetSlow() { }
}
