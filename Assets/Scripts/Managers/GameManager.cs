using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] robotsPrefab;
    [SerializeField] private GameObject[] pickUps;

    [SerializeField] private List<GameObject> unitsList;
    //private List<GameObject> enemiesList;
    private int enemiesCount =0;
    public int score=0;
    [SerializeField] private int probability=50;
    private int enemiesSpawning=1;
    private int enemiesUnlocked=1;

    

    private int waveCounter = 0;
    private int availableTokens = 0;
    [SerializeField] private float spawnerTimer = 10f;
    private float spawnerCtdw = 0f;

    private Vector3 spawnPos;
    [SerializeField] private Rect gameScene;

    public static event Action OnGameOver;


    void Awake()
    {

        DetermineSpawn();
    }

    void Update()
    {
        if (spawnerCtdw <= 0f)
        {
            waveCounter++;
            //Each 5 waves, timer goes 10% faster
            if (waveCounter % 5 == 0)
            {
                spawnerTimer = spawnerTimer * 0.9f;
            }

            if (waveCounter % 7 == 0 )
            {
                enemiesSpawning++;
            }
            if (waveCounter % 10 == 0 && enemiesUnlocked < robotsPrefab.Length)
            {
                enemiesUnlocked++;
            }
            for (int i = 0; i < enemiesSpawning; i++)
            {
                DetermineSpawn();
                Instantiate(robotsPrefab[Random.Range(0, enemiesUnlocked)], spawnPos, Quaternion.identity);
                enemiesCount++;
            }

            spawnerCtdw = spawnerTimer;
        }

        spawnerCtdw -= Time.deltaTime;

    }

    void DetermineSpawn()
    {
        //Improvement: recuperer positions de toutes les unités (avec la list) surla map pour ne pas faire spawn les enemis trops proches
        spawnPos = new Vector3(Random.Range(gameScene.xMin, gameScene.xMax), Random.Range(gameScene.yMin, gameScene.yMax));
    }

    private void OnEnable()
    {
        UnitLife.OnUnitSpawn += OnUnitSpawn;
        UnitLife.OnUnitDie += OnUnitDie;

        EnemyLife.OnEnemyDie += OnEnemyDie;
    }

    private void OnDisable()
    {
        UnitLife.OnUnitSpawn -= OnUnitSpawn;
        UnitLife.OnUnitDie -= OnUnitDie;

        EnemyLife.OnEnemyDie -= OnEnemyDie;
    }

    void OnUnitSpawn(GameObject n_unit)
    {
        unitsList.Add(n_unit);
    }

    void OnUnitDie(GameObject n_unit)
    {
        unitsList.Remove(n_unit);
        

        if (unitsList.Count <= 0)
        {
            OnGameOver?.Invoke();

            Debug.Log("GAME LOST");
            //Time.timeScale = 0f;

            //ici fire action pour uimanager game screen
        }
    }

    void OnEnemyDie(Vector3 pos, int reward)
    {
        score += reward;
        enemiesCount--;
        //enemiesList.Remove(n_enemy);


        //random spawn of pickup bonus
        if (Random.Range(0, 100) < probability)
        {
            Instantiate(pickUps[Random.Range(0,4)], pos, Quaternion.identity);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gameScene.center, gameScene.size);
    }
}
