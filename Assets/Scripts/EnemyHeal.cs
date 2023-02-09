using UnityEngine;

[CreateAssetMenu (fileName = "EnemyHeal", menuName = "Enemy Actions/Heal", order = 51)]
public class EnemyHeal : EnemyAction
{
    [SerializeField] private float healRadius;
    [SerializeField] private int healAmount;
    [SerializeField] private GameObject healEffectPrefab;
    
    public override void DoAction(Enemy enemy)
    {
        if (healEffectPrefab) Instantiate(healEffectPrefab, enemy.transform);
        
        var col = Physics2D.OverlapCircleAll(enemy.transform.position, healRadius, LayerMask.GetMask("Enemy"));
        foreach (var collider2D in col)
        {
            var colEnemy = collider2D.GetComponent<Enemy>();
            if (!colEnemy) continue;
            
            colEnemy.TakeDamage(-healAmount);
        }
    }

    
}
