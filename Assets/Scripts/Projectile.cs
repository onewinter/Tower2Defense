using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float hitAtDistance = .2f;
    [SerializeField]private float timeToTarget = .75f;
    [SerializeField]protected int damage = 35;
    [SerializeField] private bool isHoming;
    [SerializeField] private GameObject impactEffect;

    protected Enemy target;
    
    private SpriteRenderer spriteRenderer;
    private bool initialized;
    private Vector3 targetPosition;
    private float moveTime;

    // set the bullet's target at initialization
    public void SetTarget(Enemy newTarget)
    {
        target = newTarget;
        targetPosition = target.transform.position;
        initialized = true;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // don't do anything til initialized by parent
        if (!initialized) return;
        
        // if the target died, what do we do
        if (!target && isHoming)
        {
            // if we're a homing bullet, quietly die
            DestroyBullet();
        }
        // homing bullets track the target's position; dumb bullets do not
        else if (isHoming)
        {
            // while the target is alive, track their position
            targetPosition = target.transform.position;
        }

        // move the bullet towards the target
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveTime / timeToTarget);
        moveTime += Time.deltaTime;
        
        // keep moving until target is close enough to hit
        if ((targetPosition - transform.position).sqrMagnitude > hitAtDistance) return; 
            
        // deal damage to the target and kill the bullet
        spriteRenderer.enabled = false;
        OnImpact();
        DestroyBullet();
    }

    // damage the target
    protected virtual void OnImpact()
    {
        if(impactEffect) Instantiate(impactEffect, targetPosition, Quaternion.identity);
        
        if (!target) return;
        target.TakeDamage(damage);
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

}
