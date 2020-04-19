using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlant : MonoBehaviour
{
    // I feel like there's got to be a better way to bulk set or retrieve these values. Also, this
    // is going to get _interesting_ when we try to add animations. Additionally, straightA and
    // straightB are currently reversed, so either the labels are wrong or the implementation is.
    public TileBase budA;
    public TileBase budB;
    public TileBase straightA;
    public TileBase straightB;
    public TileBase rightA;
    public TileBase rightB;
    public TileBase leftA;
    public TileBase leftB;
    public TileBase teeA;
    public TileBase teeB;

    // Growth segment state information.
    private List<Bud> buds;
    private Grid grid;

    internal Tilemap plantTilemap { get; private set; }
    internal Tilemap stageTilemap { get; private set; }
    private Dictionary<PlantTilePhase, Dictionary<PlantTileType, TileBase>> tileLookup;

    // Start is called before the first frame update
    void Start()
    {
        buds = new List<Bud>();
        buds.Add(new Bud());

        grid = GetComponent<Grid>();
        plantTilemap = GameObject.Find("Tilemap_Plant").GetComponent<Tilemap>();
        stageTilemap = GameObject.Find("Tilemap_Stage").GetComponent<Tilemap>();

        tileLookup = new Dictionary<PlantTilePhase, Dictionary<PlantTileType, TileBase>> {
            {
            PlantTilePhase.A, new Dictionary<PlantTileType, TileBase> { { PlantTileType.Bud, budA },
            { PlantTileType.Straight, straightA },
            { PlantTileType.Right, rightA },
            { PlantTileType.Left, leftA },
            { PlantTileType.Tee, teeA },
            }
            },
            {
            PlantTilePhase.B,
            new Dictionary<PlantTileType, TileBase> { { PlantTileType.Bud, budB },
            { PlantTileType.Straight, straightB },
            { PlantTileType.Right, rightB },
            { PlantTileType.Left, leftB },
            { PlantTileType.Tee, teeB },
            }
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            var newBuds = new List<Bud>();
            foreach (var bud in buds)
            {
                newBuds.Add(bud.Split());
            }
            buds.AddRange(newBuds);
        }
    }

    private TileBase GetTile(PlantTilePhase phase, PlantTileType type)
    {
        return tileLookup[phase][type];
    }

    // Methods that receive from invocations of "SendMessage"
    public void OnBeat()
    {
        var died = new List<Bud>();
        var budsToCheck = new List<Bud>(buds);

        foreach (var bud in budsToCheck)
        {
            bool success = bud.TryToGrow(this);
            if (!success)
            {
                died.Add(bud);
            }
        }

        foreach (var dead in died)
        {
            buds.Remove(dead);
        }

        gameObject.SendMessage("BudsMoved");
    }

    public void AddBud(Bud bud)
    {
        buds.Add(bud);
    }

    public List<Bud> GetBuds()
    {
        return buds;
    }

    public void OnTurn(TurnDirection turn)
    {
        foreach (var bud in buds)
        {
            bud.Turn(turn);
        }
    }

    public void UpdateLocation(Vector3Int location, PlantTilePhase phase, PlantTileType type, TravelDirection direction)
    {
        plantTilemap.SetTile(location, GetTile(phase, type));
        plantTilemap.SetTransformMatrix(
            location,
            Matrix4x4.Rotate(
                Quaternion.Euler(0, 0, (int)direction)
            )
        );
    }
}