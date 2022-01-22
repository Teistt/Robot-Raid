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
    private bool _isMoving=false;


    [SerializeField] private GameObject targetEnemy;

    private Animator anim;

    private void OnEnable()
    {
        UnitMovement.OnMoving += OnUnitMoving;
    }

    private void OnDisable()
    {
        UnitMovement.OnMoving -= OnUnitMoving;
    }

    private void Awake()
    {

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //Permet de lancer la fonction UpdateTarget toutes les 0.5s à partir de 0s
        InvokeRepeating("FindNearestEnemy", 0f, 0.5f);
    }

    void OnUnitMoving(bool i)
    {
        _isMoving = i;
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            return;
        }

        if (targetEnemy == null)
        {
            return;
        }

        AttackNearestEnemy();
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
        if (fireCtdw <= 0f)
        {
            Vector3 diff = targetEnemy.transform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y,diff.x) * Mathf.Rad2Deg;

            anim.SetTrigger("_isFire");
            
            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, rot_z));
            fireCtdw = 1 / fireRate;
            //firerate correspond à nb coup/s; donc le cooldown est l'inverse
            //aka fireRate=2 donc fireCtdw=1/2=.5s
        }

        fireCtdw -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
