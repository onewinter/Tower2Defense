using UnityEngine;

public class ProjectileBomb : Projectile
{
    [SerializeField] private EnemyAction bombAction;
    
    // slow the enemy's movement speed on impact
    protected override void OnImpact()
    {
        base.OnImpact();
        
        // tell the target to turn around and strap a bomb to them
        target.ReversePath();
        target.OnUpdateOverride = bombAction;
    }
}
