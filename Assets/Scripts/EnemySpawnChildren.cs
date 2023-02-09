using UnityEngine;

[CreateAssetMenu (fileName = "EnemySpawnChildren", menuName = "Enemy Actions/Spawn Children", order = 51)]
public class EnemySpawnChildren : EnemyAction
{
    [SerializeField] private int countToSpawn;
    [SerializeField] private Enemy childPrefab;

    private SpawnManager spawnManager;
    
    public override void DoAction(Enemy enemy)
    {
        if (!spawnManager) spawnManager = FindObjectOfType<SpawnManager>();
        if(!spawnManager) return;
        
        spawnManager.SpawnEnemies(childPrefab, enemy.transform.position, countToSpawn);
    }

}