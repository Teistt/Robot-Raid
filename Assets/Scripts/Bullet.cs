using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private int damage = 1;

    [SerializeField] private float propulsionForce = 50f;
    [SerializeField] private float explosionRadius = 0f;
    [SerializeField] private float knockbackForce = 1000f;
    
    private Vector3 creationPoint;
    private Rigidbody2D rb;

    public delegate void OnDisableCallback(GameObject Instance);
    public OnDisableCallback Disable;



    private void Awake()
    {
        rb=GetComponent<Rigidbody2D>();

        
        float alpha = gameObject.transform.eulerAngles.z *Mathf.Deg2Rad;
        Vector2 xy = new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha));
        xy.Normalize();
        rb.AddForce(xy*propulsionForce,ForceMode2D.Impulse);
    }


    void OnEnable()
    {
    }

    void FixedUpdate()
    {

    }

    void Explode()
    {/*

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }*/
    }

    private void OnEnd()
    {
        Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Unit")
        {
            return;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<EnemyLife>().GetHit(damage);
            Vector3 knockback =  collision.transform.position-rb.transform.position ;
            collision.attachedRigidbody.AddForce(knockback.normalized * knockbackForce);
        }
        
        OnEnd();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
