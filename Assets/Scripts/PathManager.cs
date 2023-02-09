using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public List<TilePath> PathTiles;
    private SpawnManager spawnManager;
    
    public TilePath GetHomeTile()
    {
        return PathTiles.FirstOrDefault(x => x.IsHome);
    }

    public TilePath GetSpawnerTile()
    {
        return PathTiles.OrderBy(x => (spawnManager.transform.position - x.transform.position).sqrMagnitude)
            .FirstOrDefault();
    }


    private void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
    }

}
