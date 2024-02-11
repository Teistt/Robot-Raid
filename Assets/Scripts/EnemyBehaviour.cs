using UnityEngine;

//on enemmy spawn, move towards center of map until ontriggerexit2d
public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int boundariesLayerID = 9;

    private Vector3 targetPos;

    //when enemies start, they are on a wall. they move until collider trigger exit, then we activate navmesh
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == boundariesLayerID)
        {
            Debug.Log("NOW NAVMESH");
        }

        targetPos=Vector3.zero;
    }

    void FixedUpdate()
    {
        WalkTarget();
    }


    void WalkTarget()
    {
        Vector3 actualDir = targetPos - transform.position;

        transform.Translate(actualDir* moveSpeed*Time.deltaTime);
    }
}
