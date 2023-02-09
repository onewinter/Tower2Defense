using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public string Name;
    public int Cost;
    public float Range;
    
    [SerializeField] private float fireDelay;
    [SerializeField] protected List<Enemy> enemiesInRange;
    [SerializeField] private GameObject bulletPrefab;
    protected Enemy currentTarget;
    private float lastFired = 99f;
    
    private void Update()
    {
        // get target and fire if we have one and we can
        CheckTarget();
        if (lastFired > fireDelay) Fire();
        lastFired += Time.deltaTime;
    }

    protected virtual void CheckTarget()
    {
        // remove old targets from the list
        if (!currentTarget || !currentTarget.isActiveAndEnabled) enemiesInRange.Remove(currentTarget);
        // pick the first target in the list
        currentTarget = enemiesInRange.FirstOrDefault();
    }

    protected virtual void Fire()
    {
        // don't do anything if we don't have a valid target
        if (!currentTarget || !currentTarget.isActiveAndEnabled) return;

        // reset the fire time and shoot at the enemy
        lastFired = 0;
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        bullet.SetTarget(currentTarget);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // if a collider enters our range, see if its an enemy and then act accordingly
        if (!col.CompareTag("Enemy")) return;
        var enemy = col.GetComponent<Enemy>();
        if (!enemy) return;
     
        OnEnemyEnter(enemy);
    }

    // add enemies to the list as they enter; if we don't have a target, set the first enemy to enter to target
    protected virtual void OnEnemyEnter(Enemy enemy)
    {
        enemiesInRange.Add(enemy);
        if (!currentTarget) currentTarget = enemy;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // if a collider exits our range, see if its an enemy and then act accordingly
        if (!col.CompareTag("Enemy")) return;
        var enemy = col.GetComponent<Enemy>();
        if (!enemy) return;

        OnEnemyExit(enemy);
    }

    // remove enemies from the list as they exit; if the list is empty, null our current target
    protected virtual void OnEnemyExit(Enemy enemy)
    {
        if (enemiesInRange.Contains(enemy)) enemiesInRange.Remove(enemy);
        if (enemiesInRange.Count == 0) currentTarget = null;
    }
}
