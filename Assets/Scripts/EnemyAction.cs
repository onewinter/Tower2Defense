using UnityEngine;

public abstract class EnemyAction : ScriptableObject
{
    [SerializeField] protected float actionDelay;
    
    public abstract void DoAction(Enemy enemy);
    public float TickAction(Enemy enemy, float lastAction, float deltaTime)
    {
        lastAction += deltaTime;
        if (lastAction < actionDelay) return lastAction;

        DoAction(enemy);
        return 0;
    }
}
