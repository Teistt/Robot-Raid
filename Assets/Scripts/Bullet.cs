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
        rb.AddForce(transform.forward*propulsionForce);
    }


    void OnEnable()
    {
        rb.AddForce(new Vector2(propulsionForce, 0f));
    }

    void FixedUpdate()
    {

    }

    private void hit()
    {
        Vector3 dir = creationPoint - target.position;
        //Debug.Log("firepoint: " + creationPoint+ "target: " + target.position);
        //Debug.Log(dir);
        //Debug.Log("rot: "+Quaternion.LookRotation(dir));
        //Quand on hit on fait spawn des particules
        //GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, Quaternion.LookRotation(dir));
        

        if (explosionRadius > 0)
        {
            Explode();
        }
        else
        {
            //effectIns.transform.position = target.position + dir.normalized * 1.5f;
            Damage(target);
        }

        //Destroy(target.gameObject);

        //on détruit la boulette

        //Destroy(gameObject);
        OnEnd();

        //gameObject.SetActive(false);
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
        Disable?.Invoke(gameObject);
    }

    private void OnBecameInvisible()
    {
        Disable?.Invoke(gameObject);
    }

    void Damage(Transform enemy)
    {/*
        Enemy e = enemy.GetComponent<Enemy>();
        if (e == null)
        {
            Debug.LogError("pas de script ennemi trouvé");
        }
        else
        {
            e.TakeDamage(damage);
        }*/

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
        }
        
        OnEnd();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
