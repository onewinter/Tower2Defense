using UnityEngine;

public class TowerLookout : Tower
{
    // how much to add to the enemy's dmg mult
    [SerializeField] private float damageModifier; 
    
    // no fire mechanism for this tower
    protected override void Fire() { }

    // adjust enemy damage modifiers as they enter/exit tower range
    protected override void OnEnemyEnter(Enemy enemy)
    {
        base.OnEnemyEnter(enemy);
        enemy.AdjustDamageModifier(damageModifier);
    }

    protected override void OnEnemyExit(Enemy enemy)
    {
        base.OnEnemyExit(enemy);
        enemy.AdjustDamageModifier(-damageModifier);
    }
}
