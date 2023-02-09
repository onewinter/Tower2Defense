using UnityEngine;

public class TileBuildable : MonoBehaviour
{
    public bool Buildable = true;
    [SerializeField] private GameEvent eventNewGame;
    
    private BuildManager buildManager;
    
    // Start is called before the first frame update
    void Start()
    {
        buildManager = FindObjectOfType<BuildManager>();
    }

    // reset the Buildable status for all tiles on a new game
    private void OnEnable() => eventNewGame.RegisterListener(OnNewGame);
    private void OnDisable() => eventNewGame.UnregisterListener(OnNewGame);
    private void OnNewGame() => Buildable = true;
    
    // set the tile we're building on
    private void OnMouseEnter()
    {
        if (!Buildable) return;
        buildManager.SetBuildingTile(this);
    }

    // build at the tile we clicked on
    private void OnMouseDown()
    {
        if (!Buildable) return;
        buildManager.BuildAtTile(this);
    }
}
