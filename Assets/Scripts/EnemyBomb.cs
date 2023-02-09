using UnityEngine;

[CreateAssetMenu (fileName = "EnemyBomb", menuName = "Enemy Actions/Bomb", order = 51)]
public class EnemyBomb : EnemyAction
{
    [SerializeField] private float bombRadius;
    [SerializeField] private int bombAmount;
    [SerializeField] private GameObject explosionEffect;
    
    public override void DoAction(Enemy enemy)
    {
        if(explosionEffect) Instantiate(explosionEffect, enemy.transform.position, Quaternion.identity);

        var col = Physics2D.OverlapCircleAll(enemy.transform.position, bombRadius, LayerMask.GetMask("Enemy"));
        foreach (var collider2D in col)
        {
            var colEnemy = collider2D.GetComponent<Enemy>();
            if (!colEnemy) continue;
            
            colEnemy.TakeDamage(bombAmount);
        }

        enemy.TakeDamage(bombAmount * 100);
    }

    
}
