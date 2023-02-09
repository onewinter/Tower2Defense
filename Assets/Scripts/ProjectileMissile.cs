using UnityEngine;

public class ProjectileMissile : Projectile
{
    [SerializeField] private float impactRadius = 2f;
    
    // splash damage on impact    
    protected override void OnImpact()
    {
        base.OnImpact();
        
        // get all enemy colliders in range of impact
        var enemies = Physics2D.OverlapCircleAll(transform.position, impactRadius, LayerMask.GetMask("Enemy"));
        foreach (var enemyCollider in enemies)
        {
            // deal damage to each
            var enemy = enemyCollider.GetComponent<Enemy>();
            if(!enemy) continue;
            enemy.TakeDamage(damage);
        }
    }
}
