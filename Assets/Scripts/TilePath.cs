using UnityEngine;

public class TilePath : MonoBehaviour
{
    public bool IsHome;
    private PathManager pathManager;
    
    // load ourselves into the path manager's list at start
    void Awake()
    {
        pathManager = FindObjectOfType<PathManager>();
        pathManager.PathTiles.Add(this);
    }
}
