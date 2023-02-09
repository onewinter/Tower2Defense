using System.Linq;

public class TowerDamage : Tower
{
    protected override void CheckTarget()
    {
        base.CheckTarget();
        // pick the target closest to the home tower
        currentTarget = enemiesInRange.OrderBy(x => x.DistanceToTower()).FirstOrDefault();
    }
}
