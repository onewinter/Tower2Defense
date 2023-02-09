using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private int firstRoundDelay;
    [SerializeField] private int generalRoundDelay;
    [SerializeField] private List<WaveSetup> WavesList;
    [SerializeField] private WaveSetup CurrentWave;
    [SerializeField] private GameObject spawnEffectPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Events")]
    [SerializeField] private GameEventInt eventEnemies;
    [SerializeField] private GameEvent eventNewGame;

    [Header("Visualize Values Debug")]
    [SerializeField] private int round;
    [SerializeField] private int maxEnemies;
    [SerializeField] private int enemiesSpawned;
    [SerializeField] private int enemiesCount;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float lastSpawn;
    [SerializeField] private List<Enemy> spawnTypes;
    private GameManager gameManager;
    
    // subscribe / unsubscribe to events
    private void OnEnable()
    {
        eventEnemies.RegisterListener(AddToEnemies);
        eventNewGame.RegisterListener(OnNewGame);
    }
    
    private void OnDisable()
    {
        eventEnemies.UnregisterListener(AddToEnemies);
        eventNewGame.UnregisterListener(OnNewGame);
    }

    // on all new games
    private void OnNewGame()
    {
        // stop any coroutines on the manager
        StopAllCoroutines();
        
        // delete all enemies
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        // reset our starting variables
        round = 0;
        enemiesSpawned = 0;
        maxEnemies = 1;
        spawnDelay = .5f;
        spawnTypes = new List<Enemy>();
        
        // start the first round
        StartCoroutine(StartNextRound(firstRoundDelay));
    }

    // change the enemy count and update the UI
    private void AddToEnemies(int value)
    {
        enemiesCount += value;
        enemiesText.text = enemiesCount + " / " + enemiesSpawned;

        // only check for round completion during Attacking phase
        if (gameManager.CurrentGameState != GameState.Attacking) return;

        // if enemy count is zero, start the next round
        if (enemiesCount > 0) return;
        StartCoroutine(StartNextRound(generalRoundDelay));
    }

    private IEnumerator StartNextRound(int delay)
    {
        // change our game state and round number
        gameManager.CurrentGameState = GameState.Waiting;
        AddToRound(1);

        CurrentWave = WavesList.OrderByDescending(x=>x.Number).First(x => x.Number <= round);
        
        // display a countdown timer
        for (int i = 1; i < delay; i++)
        {
            timerText.text = (delay - i).ToString();
            yield return new WaitForSeconds(1f);    
        }

        // update UI and reset spawn count
        timerText.text = string.Empty;
        enemiesSpawned = 0;
        
        // set a new base count of enemies OR choose a random number of enemies to add between min/max
        if (CurrentWave.NewBaseCount != 0) maxEnemies = CurrentWave.NewBaseCount;
        else maxEnemies += Random.Range(CurrentWave.MinNewEnemies, CurrentWave.MaxNewEnemies);

        // if the wave says we are replacing the enemy types, clear it; add wave's enemies and set the spawn delay
        if (CurrentWave.ReplaceEnemies) spawnTypes.Clear();
        spawnTypes.AddRange(CurrentWave.Enemies);
        spawnDelay = CurrentWave.GetSpawnDelay(round);
        
        // change game state and start spawning
        gameManager.CurrentGameState = GameState.Spawning;
    }

    // update round number and UI
    private void AddToRound(int value)
    {
        round += value;
        roundText.text = round.ToString();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        // only run when game state is spawning; keep track of spawn time
        if (gameManager.CurrentGameState != GameState.Spawning) return;
        lastSpawn += Time.deltaTime;

        // only run when spawnDelay has passed since last spawn
        if (lastSpawn < spawnDelay) return;
        SpawnRoundEnemy();
        
        if (!CurrentWave.BossEnemy || enemiesSpawned < maxEnemies) return;
        SpawnEnemy(CurrentWave.BossEnemy, transform.position); // spawn final boss at end of round 25
    }

    // spawn a new enemy, return it if needed
    private Enemy SpawnEnemy(Enemy enemy, Vector3 position)
    {
        lastSpawn = 0;
        enemiesSpawned++;
        AddToEnemies(1);

        if (spawnEffectPrefab) Instantiate(spawnEffectPrefab, position, Quaternion.identity);
        
        // spawn a random enemy from our spawn types list
        var newEnemy = Instantiate(enemy, position, Quaternion.identity);
        newEnemy.transform.parent = enemiesParent;
        newEnemy.ChangeSpeed(CurrentWave.GetRoundSpeed(round));
        newEnemy.ChangeHealth(CurrentWave.GetRoundHealth(round));

        return newEnemy;
    }

    // simple wrapper coroutine
    private IEnumerator SpawnEnemies(List<Enemy> enemies, Vector3 position)
    {
        foreach (var enemy in enemies)
        {
            SpawnEnemy(enemy, position);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // called from spawner enemy action, used to spawn enemies a la carte
    public void SpawnEnemies(Enemy enemy, Vector3 position, int count)
    {
        var enemies = new List<Enemy>();
        for (var i = 0; i < count; i++)
        {
            enemies.Add(enemy);
        }

        StartCoroutine(SpawnEnemies(enemies, position));
    }

    // spawn an enemy during the spawning phase
    private void SpawnRoundEnemy()
    {
        // spawn a random enemy from our spawn types list
        SpawnEnemy(spawnTypes[Random.Range(0, spawnTypes.Count)], transform.position);
        
        // check to see if we've spawned our max enemies for the round and switch the game mode if so
        if (enemiesSpawned >= maxEnemies) gameManager.CurrentGameState = GameState.Attacking;
    }
}
