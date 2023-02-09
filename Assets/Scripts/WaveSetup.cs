using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "WaveSetup_", menuName = "Game Wave Setup", order = 51)]
public class WaveSetup : ScriptableObject
{
    public int Number; // round #
    public List<Enemy> Enemies;
    public Enemy BossEnemy; // spawned at end of round, if defined
    public bool ReplaceEnemies; // replace the spawn manager's list with ours, or add to it?

    public int NewBaseCount; // replace the spawn manager's spawnCount with ours?
    public int MinNewEnemies;
    public int MaxNewEnemies;

    public float GetRoundHealth(int round) => round / 25f + 1; // %, goes up as rounds increase
    public float GetRoundSpeed(int round) => round / 9f + 1; // %, goes up as rounds increase
    public float GetSpawnDelay(int round) => .75f - round / 150f; // %, goes down as rounds increase

}
