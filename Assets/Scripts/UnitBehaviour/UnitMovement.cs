using System;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float walkTargetPrecision = .2f;
    private Vector3 walkDestination;
    private Vector3 actualDir;

    //If unit is blocked by another collider
    private Vector3 prevPosition;
    private Rigidbody2D rb;
    private bool isMoving=false;

    public static event Action<bool> OnMoving;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb =gameObject.GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        walkDestination = transform.position;
        prevPosition = transform.position;
    }

    void FixedUpdate()
    {
        walkTargetPrecision = .2f * MouseManager.Instance.SELECTED_UNITS.Count;

        //If we are not arrived at destiantion
        if (Vector3.Distance(transform.position, walkDestination) >= walkTargetPrecision)
        {
            /*
            //If the actual position is the same as the previous one, our unit is blocked by another rigidbody so we say his arrived to destination
            //That way he can start shooting
            if (isMoving && transform.position == prevPosition)
            {
                rb.velocity = new Vector2(0, 0);
                walkDestination = transform.position;
                isMoving = false;
                anim.SetBool("_isMoving", isMoving);
                OnMoving?.Invoke(isMoving);
                return;
            }
            */


            //Recalculate the vector toward the destination
            //And normalized it in order to have constant speed
            actualDir = walkDestination - transform.position;
            actualDir.x = Mathf.Clamp(actualDir.x, -1f, 1f);
            actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

            rb.velocity = actualDir * walkSpeed;

            //register new position for next fixed update
            prevPosition = transform.position;
        }

        //If we are arrived, we don't need to move
        else
        {
            rb.velocity = new Vector2(0, 0);

            //If we were moving, we send action to tell we stopped
            //We also change the animation to idle
            if (isMoving)
            {
                isMoving = false;
                anim.SetBool("_isMoving", isMoving);
                OnMoving?.Invoke(isMoving);
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        walkDestination = destination;
        isMoving = true;
        anim.SetBool("_isMoving", isMoving);
        OnMoving?.Invoke(isMoving);
    }
}