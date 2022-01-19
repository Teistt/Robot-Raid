using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    private Vector3 walkDestination;
    private Vector3 actualDir;
    private Rigidbody2D rb;


    void Awake()
    {
        rb=gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        walkDestination = transform.position;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, walkDestination) >= 0.2f)
        {
            actualDir = walkDestination - transform.position;
            //transform.Translate(actualDir.normalized * walkSpeed * Time.deltaTime, Space.World);
            //rb.MovePosition(walkDestination);
            actualDir.x = Mathf.Clamp(actualDir.x,-1f,1f);
            actualDir.y = Mathf.Clamp(actualDir.y, -1f, 1f);

            rb.velocity = actualDir * walkSpeed;
        }
        else rb.velocity = new Vector2(0, 0);
    }

    public void SetDestination(Vector3 destination)
    {
        walkDestination = destination;
    }
}
