using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitNavMeshMovement : NavMeshManager
{
    //[SerializeField] private float walkSpeed = 3f;
    //[SerializeField] private float walkTargetPrecision = .2f;
    //private Vector3 walkDestination;

    private bool isMoving=false;
    //private bool _isNavMesh = false;
    
    //int countFrame = 0;
    public static event Action<bool> OnMoving;

    private Animator m_anim;

    protected override void Init()
    {
        m_anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!m_agent.hasPath)
        {
            SetMoving(false);
        }


        if (_isKnockdBack)
        {
            if (m_rb.velocity.magnitude < 1f)
            {
                m_rb.velocity = Vector2.zero;
                m_rb.isKinematic = true;
                m_agent.enabled = true;

                _isKnockdBack = false;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        //When the player set destination for selected units the Mouse Manager Class
        //call each selected unit's SetDestination Method
        NavMeshPath path = new NavMeshPath();

        if (m_agent.CalculatePath(destination, path))
        {
            m_agent.SetPath(path);
        }
        SetMoving(true);
    }


    private void SetMoving(bool b)
    {
        // Do this instead of agent.SetDestination(destination) to force waiting path is fully calculated before moving
        isMoving = b;
        //We set unit's movement animation and we launch the isMoving Action for UnitShoot and MedicHeal classes
        //That way, these classes stop firing or heal
        m_anim.SetBool("_isMoving", isMoving);
        OnMoving?.Invoke(isMoving);
    }


    //public override void Knocked(Vector3 knockback)
    //{
    //    if(knockback==null || knockback==Vector3.zero)
    //    { return; }

    //    _isKnockedBack = true;
    //    m_agent.enabled = false;
    //    m_rb.isKinematic = false;
    //    m_rb.AddForce(knockback, ForceMode2D.Impulse);
    //}
}