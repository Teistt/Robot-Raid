using System;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float walkTargetPrecision = .2f;
    private Vector3 walkDestination;
    private Vector3 actualDir;

    private Rigidbody2D rb;
    private bool isMoving=false;

    public static event Action<bool> OnMoving;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb =gameObject.GetComponent<Rigidbody2D>();

        //rb.gravityScale = 0f;
        walkDestination = transform.position;
    }

    void FixedUpdate()
    {
        walkTargetPrecision = .2f * MouseManager.Instance.SELECTED_UNITS.Count;

        //If we are not arrived at destiantion
        if (Vector3.Distance(transform.position, walkDestination) >= walkTargetPrecision)
        {
            //Recalculate the vector toward the destination
            //And normalized it in order to have constant speed
            actualDir = walkDestination - transform.position;
            actualDir.x = Mathf.Clamp(actualDir.x, -1f, 1f);
            actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

            rb.velocity = actualDir * walkSpeed;
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

                //Launch isMoving Action for UnitShoot and MedicHeal classes to start firing/healing
                OnMoving?.Invoke(isMoving);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the unit collide with an obstacle, we say his arrived at destination
        //That way we prevent it from getting stucked by obstacle
        if (collision.gameObject.layer == 7)
        {
            walkDestination = transform.position;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        //When the player set destination for selected units the Mouse Manager Class
        //call each selected unit's SetDestination Method
        walkDestination = destination;
        isMoving = true;
        //We set unit's movement animation and we launch the isMoving Action for UnitShoot and MedicHeal classes
        //That way, these classes stop firing or heal
        anim.SetBool("_isMoving", isMoving);
        OnMoving?.Invoke(isMoving);
    }
}