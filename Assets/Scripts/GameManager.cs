using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] robotsPrefab;
    [SerializeField] private GameObject[] pickUps;


    [SerializeField] private int score=0;
    [SerializeField] private int probability=50;

    [SerializeField] private int unitsAlive = 0;
    [SerializeField] private int enemiesAlive = 0;


    private void OnEnable()
    {
        UnitLife.OnUnitDie+=OnUnitDie;
        EnemyLife.OnEnemyDie += OnEnemyDie;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnUnitDie()
    {
        unitsAlive--;
    }

    void OnEnemyDie(int reward, Vector3 position)
    {
        score += reward;
        enemiesAlive--;

        if (enemiesAlive < 0)
        {
            Debug.LogWarning("enemies alive shouldn't be negative");
            enemiesAlive = 0;
        }

        //random spawn of pickup bonus
        if (Random.Range(0, 100) > probability)
        {
            
            Instantiate(pickUps[Random.Range(0,4)], position, Quaternion.identity);
        }


    }
}
