using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private GameObject unitToSpawn;
    private bool hasBeenUsed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            if (hasBeenUsed)
            {
                return;
            }
            hasBeenUsed = true;
            Instantiate(unitToSpawn, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
