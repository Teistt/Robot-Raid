using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitNavMeshMovement : NavMeshManager
{
//    [SerializeField] private float walkSpeed = 3f;
    //[SerializeField] private float walkTargetPrecision = .2f;
    //private Vector3 walkDestination;

    private bool isMoving=false;
    //private bool _isNavMesh = false;
    private bool _isKnockedBack = false;
    //int countFrame = 0;
    public static event Action<bool> OnMoving;

    private Rigidbody2D rb;
    private Animator anim;
    private NavMeshAgent agent;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb =gameObject.GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        rb.isKinematic=true;

        //SetDestination(transform.position);
    }

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void FixedUpdate()
    {
        if (!agent.hasPath)
        {
            SetMoving(false);
        }


        if (_isKnockedBack)
        {
            if (rb.velocity.magnitude < 1f)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                agent.enabled = true;

                _isKnockedBack = false;
            }
            else
            {
                return;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        //When the player set destination for selected units the Mouse Manager Class
        //call each selected unit's SetDestination Method
        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(destination, path))
        {
            agent.SetPath(path);
        }
        SetMoving(true);
    }


    private void SetMoving(bool b)
    {
        // Do this instead of agent.SetDestination(destination) to force waiting path is fully calculated before moving
        isMoving = b;
        //We set unit's movement animation and we launch the isMoving Action for UnitShoot and MedicHeal classes
        //That way, these classes stop firing or heal
        anim.SetBool("_isMoving", isMoving);
        OnMoving?.Invoke(isMoving);
    }


    public override void Knocked(Vector3 knockback)
    {
        if(knockback==null || knockback==Vector3.zero)
        { return; }

        _isKnockedBack = true;
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }
}