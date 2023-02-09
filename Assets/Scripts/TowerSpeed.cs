using System.Linq;

public class TowerSpeed : Tower
{
    protected override void CheckTarget()
    {
        base.CheckTarget();
        // pick the target with the highest speed that's closest to the tower
        currentTarget = enemiesInRange.OrderByDescending(x => x.GetSpeed()).ThenBy(x => x.DistanceToTower()).FirstOrDefault();
    }
}
