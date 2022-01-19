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

    private Collider2D[] col;
    private List<Collider2D> en = new List<Collider2D>();
    [SerializeField] private GameObject targetEnemy;
    private float nearDist = 1000f;

    private ObjectPool<GameObject> BulletPool;

    private void Awake()
    {
        BulletPool = new ObjectPool<GameObject>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 20, 100);
    }


    private GameObject CreatePooledObject()
    {
        GameObject instance = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        instance.GetComponent<Bullet>().Disable += ReturnObjectToPool;
        instance.SetActive(false);

        return instance;
    }
    
    private void ReturnObjectToPool(GameObject Instance)
    {
        BulletPool.Release(Instance);
    }

    private void OnTakeFromPool(GameObject Instance)
    {

        Instance.SetActive(true);
        PooledShoot(Instance);
        Instance.transform.SetParent(transform, true);
    }


    private void OnReturnToPool(GameObject Instance)
    {
        Instance.SetActive(false);
    }


    private void OnDestroyObject(GameObject Instance)
    {
        Destroy(Instance);
    }

    private void Start()
    {
        //Permet de lancer la fonction UpdateTarget toutes les 0.5s à partir de 0s
        InvokeRepeating("FindNearestEnemy", 0f, 0.5f);
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            Look(targetEnemy.transform.position); ;
        }

        AttackNearestEnemy();
    }

    private void FindNearestEnemy()
    {
        col = Physics2D.OverlapCircleAll(transform.position, shootRange);
        en.Clear();
        foreach (var item in col)
        {
            if (item.gameObject.tag == "Enemy")
            {
                en.Add(item);
            }
        }

        if (en.Count == 0)
        {
            targetEnemy = null;
            nearDist = 1000f;
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
            nearDist = 1000f;
        }
    }


    private void Look(Vector3 target)
    {
        float angle = 0;

        Vector3 relative = transform.InverseTransformPoint(target);
        angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);
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
            BulletPool.Get();

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
        bulletGO.transform.rotation = transform.rotation;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
