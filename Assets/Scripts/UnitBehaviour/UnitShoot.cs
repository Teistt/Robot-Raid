using UnityEngine;

public class UnitShoot : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootRange=1f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private string enemyLayerName = "Enemy";
    private float fireCtdw = 0f; //cooldown

    private bool facingRight = true;
    private bool _isMoving=false;

    LayerMask mask;
    //[SerializeField] private GameObject targetEnemy;

    private Animator anim;

    #region Actions
    private void OnEnable()
    {
        UnitNavMeshMovement.OnMoving += OnUnitMoving;
    }

    private void OnDisable()
    {
        UnitNavMeshMovement.OnMoving -= OnUnitMoving;
    }
    #endregion

    void OnUnitMoving(bool i)
    {
        _isMoving = i;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        mask = LayerMask.GetMask(enemyLayerName);
    }


    void FixedUpdate()
    {
        //If the cooldown is reached, whe checked if we are moving
        //If not, we can fire nearest enemy around us

        //This way, fireCtw is updated even if we are moving
        if (fireCtdw <= 0f)
        {
            if (_isMoving)
            {
                //Debug.Log("cannot shoot");
                return;
            }

            //We check for the nearest enemy; stored in targetEnemy
            Transform targetEnemy=FindNearestEnemy();

            //If there is no enemy, we return
            if (targetEnemy == null)
            {
                return;
            }

            //We check to look at targetEnemy
            LookHorSide(targetEnemy.position);

            //We attack targetEnemy
            AttackEnemy(targetEnemy.position);

            fireCtdw = 1 / fireRate;
            //firerate correspond à nb coup/s; donc le cooldown est l'inverse
            //aka fireRate=2 donc fireCtdw=1/2=.5s
        }

        else
        {
            //If countdown not finished, we update it since the last frame time
            fireCtdw -= Time.deltaTime;
        }

    }

    private Transform FindNearestEnemy()
    {
        float nearDist = 1000f;

        //Physic raycast to get all GO with "Enemy" mask and in radius shootRange in en Collider2D array
        Collider2D[] en = Physics2D.OverlapCircleAll(transform.position, shootRange, mask);
        Collider2D nearestEn = null;

        if (en.Length == 0)
        {
            return null;
        }
        else
        {
            foreach (Collider2D item in en)
            {
                //For each detected enemy, we check if its the closer
                //If so, we register is current distance in order to compare with remaining enemies
                float actDistance = Vector2.Distance(item.gameObject.transform.position, transform.position);

                if (actDistance <= nearDist)
                {
                    nearDist = actDistance;
                    nearestEn = item;
                }
            }

            return nearestEn.transform;
        }
    }


    void LookHorSide(Vector3 targetEnemy)
    {
        if (targetEnemy == null)
        {
            Debug.Log("Vector3 null");
            return;
        }
        //check if targetted enemy is at our right or left
        float horizontalValue = transform.position.x - targetEnemy.x;
        //If we are not facing it, we turn unit around
        if (horizontalValue < 0 && !facingRight || horizontalValue > 0 && facingRight)
        {
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
            GetComponent<UnitLife>().lifeSlider.transform.Rotate(0, 180, 0);
        }
    }


    private void AttackEnemy(Vector3 target)
    {
        //Calcul of Vector's angle between firepoint's unit and enemy
        Vector3 diff = target - firePoint.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        //Set Unit's Fire animation
        anim.SetTrigger("_isFire");

        //Instantiate bullet at firepoint position and calculated angle
        Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, rot_z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
