using UnityEngine;

public class ProjectileGlue : Projectile
{
    [SerializeField] private float percentageImpact = .9f;
    
    // slow the enemy's movement speed on impact
    protected override void OnImpact()
    {
        base.OnImpact();
        target.ChangeSpeed(percentageImpact);
    }
}
