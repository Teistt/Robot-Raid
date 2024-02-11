using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitNavMeshMovement : NavMeshManager
{
    private bool isMoving=false;
    
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
            SetAnimationMoving(false);
        }
    }

    public override void SetDestinationPostAction()
    {
        SetAnimationMoving(true);
    }

    private void SetAnimationMoving(bool b)
    {
        // Do this instead of agent.SetDestination(destination) to force waiting path is fully calculated before moving
        isMoving = b;
        //We set unit's movement animation
        m_anim.SetBool("_isMoving", isMoving);

        //Action for UnitShoot and MedicHeal classes that way, these classes stop firing or heal
        OnMoving?.Invoke(isMoving);
    }
}