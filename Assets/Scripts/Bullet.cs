using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private int damage = 1;

    [SerializeField] private float propulsionForce = 50f;
    [SerializeField] private float explosionRadius = 0f;
    
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
        if(collision.gameObject.tag != "Enemy")
        {
            return;
        }

        else
        {
            if (explosionRadius <= 0)
            {
                Vector3 knockback = ((collision.transform.position - rb.transform.position).normalized);
                collision.GetComponent<EnemyLife>().GetHit(damage);
                //collision.attachedRigidbody.AddForce(knockback.normalized * knockbackForce);
            }
            else
            {
                LayerMask mask = LayerMask.GetMask("Enemy");
                Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, explosionRadius, mask);
                foreach (var item in en)
                {

                    Vector3 knockback = ((item.transform.position - rb.transform.position).normalized);
                    item.GetComponent<EnemyLife>().GetHit(damage);
                }
            }
        }
        
        OnEnd();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
