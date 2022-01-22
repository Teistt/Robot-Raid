using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] robotsPrefab;
    [SerializeField] private int[] robotsCost;
    [SerializeField] private GameObject[] pickUps;

    [SerializeField] private List<GameObject> unitsList;
    //private List<GameObject> enemiesList;
    private int enemiesCount =0;
    public int score=0;
    [SerializeField] private int probability=50;
    private int enemiesSpawning=1;
    private int enemiesUnlocked=1;

    [SerializeField] private bool _isDebugNoSpawn=false;

    private int waveCounter = 0;
    private int availableTokens = 0;
    [SerializeField] private float spawnerTimer = 10f;
    private float spawnerCtdw = 0f;

    private Vector3 spawnPos;
    [SerializeField] private Rect gameScene;

    public static event Action OnGameOver;

    //Choice for tokens calculation over time
    public enum myEnum // your custom enumeration
    {
        Linear,
        Exponential,
        Polynom
    };
    public myEnum dropDown = myEnum.Linear;  // this public var should appear as a drop down

    //Linear coefficient linA*x +linB
    private float linA = 1.3871f;
    private float linB = 0.4516f;

    //Exponential coefficient expA *e^(expB*x)
    private float expA = 1.1014f;
    private float expB = 0.2785f;

    //Polynomial coefficient polA * x^2 + polB *x + polC
    private float polA = -0.0135f;
    private float polB = 1.529f;
    private float polC = -0.6643f;


    void Awake()
    {
        DetermineSpawn();
        Spawner();
    }

    void Update()
    {
        if (_isDebugNoSpawn)
        {
            return;
        }

        if (spawnerCtdw <= 0f || enemiesCount==0)
        {
            Spawner();
            /*
            for (int i = 0; i < enemiesSpawning; i++)
            {
                DetermineSpawn();
                Instantiate(robotsPrefab[Random.Range(0, enemiesUnlocked)], spawnPos, Quaternion.identity);
                enemiesCount++;
            }*/

            spawnerCtdw = spawnerTimer;
        }
        //Debug.Log(spawnerCtdw);
        spawnerCtdw -= Time.deltaTime;

    }

    void Spawner()
    {
        waveCounter++;
        //Each 5 waves, timer goes 10% faster
        if (waveCounter % 6 == 0)
        {
            spawnerTimer = spawnerTimer * 0.9f;
        }

        if (waveCounter % 3 == 0 && enemiesUnlocked < robotsPrefab.Length)
        {
            enemiesUnlocked++;
        }

        //tokens count based on choosen calcul in enum
        if (dropDown == myEnum.Linear)
        {
            availableTokens = Mathf.RoundToInt(linA * waveCounter + linB);
            if (availableTokens == 0) availableTokens++;
        }
        else if (dropDown == myEnum.Exponential)
        {
            availableTokens = Mathf.RoundToInt(expA * Mathf.Exp(expB * availableTokens));
            if (availableTokens == 0) availableTokens++;
        }
        else
        {
            availableTokens = Mathf.RoundToInt((polA * availableTokens * availableTokens) + (polB * availableTokens) + polC);
            if (availableTokens == 0) availableTokens++;
        }

        while (availableTokens > 0)
        {
            int indexToSpawn;
            switch (availableTokens)
            {
                case 5:
                    indexToSpawn = 3;
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;

                case 4:
                    indexToSpawn = 2;
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;

                case 3:
                    indexToSpawn = 0;
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;

                case 2:
                    indexToSpawn = 1;
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;

                case 1:
                    indexToSpawn = 0;
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;

                default:
                    indexToSpawn = Random.Range(0, enemiesUnlocked);
                    availableTokens -= robotsCost[indexToSpawn];
                    DetermineSpawn();
                    Instantiate(robotsPrefab[indexToSpawn], spawnPos, Quaternion.identity);
                    enemiesCount++;
                    break;
            }

            //Debug.Log("enemies count " + enemiesCount);
        }
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
