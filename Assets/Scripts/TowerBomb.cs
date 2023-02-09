using System.Linq;
using UnityEngine;

public class TowerBomb : Tower
{
    [SerializeField] private EnemyAction bombAction; 
    
    protected override void CheckTarget()
    {
        base.CheckTarget();
        
        // pick the target closest to the home tower that isn't bombed yet
        currentTarget = enemiesInRange.OrderBy(x => x.OnUpdateOverride == bombAction).ThenBy(x => x.DistanceToTower())
            .FirstOrDefault();
    }
}
