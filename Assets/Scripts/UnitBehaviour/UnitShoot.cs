using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnitShoot : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootRange=1f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int damage = 1;
    private float fireCtdw = 0f; //cooldown

    private bool facingRight = true;

    private Collider2D[] col;
    //private List<Collider2D> en = new List<Collider2D>();
    [SerializeField] private GameObject targetEnemy;

    private ObjectPool<GameObject> BulletPool;

    private void Awake()
    {
        //BulletPool = new ObjectPool<GameObject>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, true, 20, 100);
    }

    private void Start()
    {
        //Permet de lancer la fonction UpdateTarget toutes les 0.5s à partir de 0s
        InvokeRepeating("FindNearestEnemy", 0f, 0.5f);
    }

    void FixedUpdate()
    {
        AttackNearestEnemy();

        if (targetEnemy == null)
        {
            return;
        }

        LookSide();
    }

    private void FindNearestEnemy()
    {
        LayerMask mask = LayerMask.GetMask("Enemy");
        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, shootRange, mask);

        float nearDist = 1000f;
        if (en.Length == 0)
        {
            targetEnemy = null;
        }
        else
        {
            foreach (var item in en)
            {
                float actDistance = Vector2.Distance(item.gameObject.transform.position, transform.position);
                if (actDistance <= nearDist)
                {
                    nearDist = actDistance;
                    targetEnemy = item.gameObject;
                }
            }
        }
    }


    void LookSide()
    {
        float horizontalValue = transform.position.x - targetEnemy.transform.position.x;
        if (horizontalValue < 0 && !facingRight || horizontalValue > 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }


    private void AttackNearestEnemy()
    {
        if (targetEnemy == null)
        {
            return;
        }


        if (fireCtdw <= 0f)
        {
            //On tire
            //BulletPool.Get();

            //float rot_z = Mathf.Atan2( targetEnemy.transform.position.y- transform.position.y ,  targetEnemy.transform.position.x- transform.position.x);

            Vector3 diff = targetEnemy.transform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y,diff.x) * Mathf.Rad2Deg;

            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, rot_z));
            fireCtdw = 1 / fireRate;
            //firerate correspond à nb coup/s; donc le cooldown est l'inverse
            //aka fireRate=2 donc fireCtdw=1/2=.5s
        }

        fireCtdw -= Time.deltaTime;
    }


    void PooledShoot(GameObject bulletGO)
    {
        //On instantie la boullette a la position firePoint
        bulletGO.transform.position = firePoint.position;
        //bulletGO.transform.rotation = transform.rotation;
        float rot_z = Mathf.Atan2(transform.position.y-targetEnemy.transform.position.y, transform.position.x - targetEnemy.transform.position.x);
        rot_z = rot_z * Mathf.Rad2Deg;


        bulletGO.transform.rotation = Quaternion.Euler(0, 0, rot_z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
