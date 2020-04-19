using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStage : MonoBehaviour {
    private Tilemap tilemap;
    private Dictionary<Vector3Int, StageTile> tiles;

    // Start is called before the first frame update
    void Start() {
        tiles = new Dictionary<Vector3Int, StageTile>();
        tilemap = GameObject.Find("Tilemap_Stage").GetComponent<Tilemap>();
        var bounds = tilemap.cellBounds;
        foreach (var cell in bounds.allPositionsWithin) {
            var tile = tilemap.GetTile(cell);
            if (tile != null) {
                switch (tile.name) {
                    case "spike":
                        tiles.Add(cell, StageTile.Spike);
                        break;
                    case "dirt":
                        tiles.Add(cell, StageTile.Dirt);
                        break;
                    default:
                        Debug.LogError("Unknown tile: " + tile);
                        break;
                }
            }
        }
    }

    public StageTile TileAt(Vector3Int pos) {
        StageTile tile = StageTile.Blank;
        if (tiles.ContainsKey(pos)) {
            tile = tiles[pos];
        }
        return tile;
    }

    // Update is called once per frame
    void Update() {

    }
}