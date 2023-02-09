using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private bool isEndBoss;
    [SerializeField] private float nextTileAtDistance = .02f;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private int money = 5;
    [SerializeField] private EnemyAction OnUpdateAction;
    public EnemyAction OnUpdateOverride;
    [SerializeField] private EnemyAction OnDieAction;

    [Header("Events")]
    [SerializeField] private GameEventInt eventHealth;
    [SerializeField] private GameEventInt eventEnemies;
    [SerializeField] private GameEventInt eventMoney;
    [SerializeField] private GameEvent eventWin;

    private PathManager pathManager;
    private Rigidbody2D rig2D;
    private List<TilePath> fullPath;
    private List<TilePath> path;
    private TilePath startTile;
    private TilePath nextTile;
    private Vector3 movementDir;
    private MaterialPropertyBlock matBlock;
    private MeshRenderer meshRenderer;
    [SerializeField] private float damageModifier = 1f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lastAction;

    #region Unity Lifecycle & Physics Methods
    private void Start()
    {
        // init components
        rig2D = GetComponent<Rigidbody2D>();
        pathManager = FindObjectOfType<PathManager>();
        
        // build the path from the spawner to the home tower
        MakeStartingPath();
        
        // init the healthbar
        meshRenderer = healthBar.GetComponent<MeshRenderer>();
        matBlock = new MaterialPropertyBlock();
        UpdateParams();
    }

    private void Update()
    {
        // don't do anything if no destination (bombed enemies that made it back home)
        if (!nextTile)
        {
            OnEnemyDie(true);
            return;
        }
        
        // run our defined action every tick, if we have one
        if(OnUpdateAction || OnUpdateOverride)
        {
            EnemyAction action;
            action = OnUpdateOverride ? OnUpdateOverride : OnUpdateAction;
            lastAction = action.TickAction(this, lastAction, Time.deltaTime);
        }
        
        // if close enough to destination, switch to the next destination
        if ((nextTile.transform.position - transform.position).sqrMagnitude < nextTileAtDistance) GetNextTile();
    }

    private void FixedUpdate()
    {
        // move in the direction of the destination
        if (!nextTile) return;
        rig2D.MovePosition(transform.position + movementDir * (Time.deltaTime * speed));
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        // find out what tile we collided with
        var tile = col.GetComponent<TilePath>();
        if (!tile) return;
        
        // only run if we hit the home tower tile
        if (!tile.IsHome) return;
        
        // hurt the player and destroy the enemy
        OnEnemyDie(false);
    }
    
    #endregion

    public void AdjustDamageModifier(float value) => damageModifier += value;
    
    // build the path to the player's home tower
    private void MakeStartingPath()
    {
        fullPath = pathManager.PathTiles.ToList();
        startTile = GetClosestTile();
        BuildPath(startTile, pathManager.GetHomeTile());
    }

    // tell the enemy to walk towards the spawner
    public void ReversePath()
    {
        fullPath = pathManager.PathTiles.ToList();
        BuildPath(GetClosestTile(), pathManager.GetSpawnerTile());
    }

    // update health bar
    private void UpdateParams() {
        meshRenderer.GetPropertyBlock(matBlock);
        matBlock.SetFloat("_Fill", currentHealth / (float)maxHealth);
        meshRenderer.SetPropertyBlock(matBlock);
    }

    // when enemy dies, either by being shot or hitting the home tower
    void OnEnemyDie(bool killed)
    {
        if (killed && OnDieAction) OnDieAction.DoAction(this);
        if (killed) eventMoney.Raise(money); else eventHealth.Raise(-damage);
        eventEnemies.Raise(-1);
        gameObject.SetActive(false);
        Destroy(gameObject,1);
    }

    // take damage, check for death
    public void TakeDamage(int value)
    {
        // don't run if already dead (overkills)
        if (currentHealth <= 0) return;
        
        // update health and healthbar
        currentHealth -= value * damageModifier;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateParams();
        
        // don't run if alive 
        if (currentHealth > 0) return;
        
        // if end boss dies, raise win event
        if (isEndBoss) eventWin.Raise();
        
        // give the player money and destroy the enemy
        OnEnemyDie(true);
    }

    // used by tower targeting logic to pick best target
    public int DistanceToTower() => path.Count;

    public float GetSpeed() => speed;

    // used by spawner to increase enemy difficulty over time
    public void ChangeSpeed(float percent) => speed *= percent;

    public void ChangeHealth(float percent)
    { 
        maxHealth *= percent;
        currentHealth = maxHealth;
    }

    private void BuildPath(TilePath startTile, TilePath endTile)
    {
        path = new List<TilePath>();

        // work backwards from the home tower to build a path to this enemy's tile
        // (in case the enemy starts in the middle of the path)
        path.Add(endTile);
        
        // remove path tiles as we go so we can only go in one direction each loop
        fullPath.Remove(endTile);

        TilePath currentTile = endTile;
        
        // loop max the full amount of tiles in the path
        var loopCount = fullPath.Count;
        for (var i = 0; i < loopCount; i++)
        {
            // find the closest tile each loop and add it to the path
            currentTile = GetClosestTile(currentTile);
            path.Add(currentTile);
            
            // then pick it up behind us so we only go forward
            fullPath.Remove(currentTile);
            
            // when we get to the enemy's tile, the path is complete
            if (currentTile == startTile) break;
        }
        
        // reverse the path to start with the enemy, not the home tower
        path.Reverse();
        
        // set the enemy's starting path tile
        nextTile = currentTile;
        
        // get their first destination
        GetNextTile();
    }

    // closest tile to this enemy

    private TilePath GetClosestTile()
    {
        return fullPath.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
    }

    // closest tile to the passed tile

    private TilePath GetClosestTile(TilePath tile)
    {
        return fullPath.OrderBy(x => (x.transform.position - tile.transform.position).sqrMagnitude).FirstOrDefault();
    }

    private void GetNextTile()
    {
        // remove our last tile from our path
        path.Remove(nextTile);
        
        // get the next tile in the ordered list
        nextTile = path.FirstOrDefault();
        
        // if we got to the end, stop
        if (!nextTile) return;
        
        // calculate our normalized movement direction
        movementDir = (nextTile.transform.position - transform.position).normalized;
    }


}
