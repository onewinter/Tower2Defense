using Unity.Mathematics;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject buildingMarker;
    [SerializeField] private Transform buildingMarkerRange;
    [SerializeField] private Tower gunTowerPrefab;
    [SerializeField] private Tower missileTowerPrefab;
    [SerializeField] private Tower glueTowerPrefab;
    [SerializeField] private Tower lookoutTowerPrefab;
    [SerializeField] private Tower bombTowerPrefab;

    [Header("Events")]
    [SerializeField] private GameEventInt eventMoney;
    [SerializeField] private GameEvent eventNewGame;

    private Tower currentlyBuilding;
    private bool building;
    private GameManager gameManager;
    
    // new game event registration
    private void OnEnable() => eventNewGame.RegisterListener(OnNewGame);
    private void OnDisable() => eventNewGame.UnregisterListener(OnNewGame);

    private void OnNewGame()
    {
        // delete all towers
        var towers = FindObjectsOfType<Tower>();
        foreach (var tower in towers)
        {
            Destroy(tower.gameObject);
        }
    }
    
    public void SetBuildingTile(TileBuildable tile)
    {
        // only execute in building mode
        if (!building) return;
        
        // move building marker to hovered tile and set range marker scale to show tower's range
        buildingMarker.transform.position = tile.transform.position;
        buildingMarkerRange.transform.localScale = new Vector3(currentlyBuilding.Range*2f, currentlyBuilding.Range*2f, 1);
    }

    public void BuildAtTile(TileBuildable tile)
    {
        // only execute in building mode
        if (!building) return;
        
        // only execute if we currently have a tower set to build
        if (!currentlyBuilding) return;
        
        // make sure we can't build on the tile again
        tile.Buildable = false;
        
        // build our tower and take the money from the player
        Instantiate(currentlyBuilding, tile.transform.position, quaternion.identity);
        eventMoney.Raise(-currentlyBuilding.Cost);
        
        // clear the build process
        ClearCurrentlyBuilding();
    }

    // set building mode off and move the building marker off screen
    private void ClearCurrentlyBuilding()
    {
        currentlyBuilding = null;
        building = false;
        buildingMarker.transform.position = new Vector3(-999, -999, 0);
    }

    // build a gun tower
    public void ClickGunTowerButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < gunTowerPrefab.Cost) return;
        building = true;
        currentlyBuilding = gunTowerPrefab;
    }
    
    // build a glue tower
    public void ClickGlueTowerButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < glueTowerPrefab.Cost) return;
        building = true;
        currentlyBuilding = glueTowerPrefab;
    }
    
    // build a missile tower
    public void ClickMissileTowerButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < missileTowerPrefab.Cost) return;
        building = true;
        currentlyBuilding = missileTowerPrefab;
    }
    
    // build a lookout tower
    public void ClickLookoutTowerButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < lookoutTowerPrefab.Cost) return;
        building = true;
        currentlyBuilding = lookoutTowerPrefab;
    }
    
    // build a bomb tower
    public void ClickBombTowerButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < bombTowerPrefab.Cost) return;
        building = true;
        currentlyBuilding = bombTowerPrefab;
    }

    public void ClickMoneyButton()
    {
        // make sure we can afford it
        if (gameManager.GetHealth() <= 3) return;
        gameManager.AddToHealth(-3);
        gameManager.AddToMoney(200);

    }
    
    public void ClickHealthButton()
    {
        // make sure we can afford it
        if (gameManager.GetMoney() < 200) return;
        gameManager.AddToHealth(1);
        gameManager.AddToMoney(-200);

    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        // right click to clear building mode
        if (Input.GetMouseButtonDown(1)) ClearCurrentlyBuilding();
    }
}
