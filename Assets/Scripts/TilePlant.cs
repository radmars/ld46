using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlant : MonoBehaviour
{
    public Vector3Int location;
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

    enum Phase
    {
        A,
        B,
    }

    enum Type
    {
        Bud,
        Straight,
        Right,
        Left,
        Tee,
    }

    // Growth segment state information.
    private Phase phase = Phase.A;
    private Type nextType = Type.Straight;
    private Vector3Int heading = Vector3Int.up;

    private Grid grid;
    private Tilemap tilemap;
    private Dictionary<Phase, Dictionary<Type, TileBase>> tileLookup;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
        tilemap = GameObject.Find("Tilemap_Plant").GetComponent<Tilemap>();
        tileLookup = new Dictionary<Phase, Dictionary<Type, TileBase>>
        {
            {
               Phase.A, new Dictionary<Type, TileBase>
               {
                   { Type.Bud, budA },
                   { Type.Straight, straightA },
                   { Type.Right, rightA },
                   { Type.Left, leftA },
                   { Type.Tee, teeA },
               }
            },
            {
                Phase.B, new Dictionary<Type, TileBase>
               {
                   { Type.Bud, budB },
                   { Type.Straight, straightB },
                   { Type.Right, rightB },
                   { Type.Left, leftB },
                   { Type.Tee, teeB },
               }
            }
        };
    }

    void Grow()
    {
        Vector3Int newHeading = getNewHeading(heading, nextType);
        Vector3Int newLocation = location + newHeading;
        Phase newPhase = getNewPhase(phase, nextType);

        tilemap.SetTile(location, GetTile(phase, nextType));
        tilemap.SetTransformMatrix(location, Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3Int.up, heading)));
        tilemap.SetTile(newLocation, GetTile(newPhase, Type.Bud));
        tilemap.SetTransformMatrix(newLocation, Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3Int.up, newHeading)));

        location = newLocation;
        heading = newHeading;
        phase = newPhase;
        nextType = Type.Straight;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Phase swaps on straight segments, but outputs of turns and T-junctions are always phase B.
    private Phase getNewPhase(Phase oldPhase, Type nextType)
    {
        if (nextType == Type.Straight)
        {
            return oldPhase == Phase.A ? Phase.B : Phase.A;
        }
        return Phase.B;
    }

    private Vector3Int getNewHeading(Vector3Int oldHeading, Type nextType)
    {
        if (nextType == Type.Straight)
        {
            return oldHeading;
        }

        // There's gotta be a better way to rotate vectors, but Quaternions can't be used with
        // Vector3Ints.
        if (nextType == Type.Right)
        {
            return new Vector3Int(oldHeading.y, -oldHeading.x, 0);
        }
        else if (nextType == Type.Left)
        {
            return new Vector3Int(-oldHeading.y, oldHeading.x, 0);
        }
        else
        {
            throw new System.ArgumentOutOfRangeException("T junctions NYI.");
        }
    }

private TileBase GetTile(Phase phase, Type type)
    {
        return tileLookup[phase][type];
    }

    // Methods that receive from invocations of "SendMessage"
    public void OnBeat()
    {
        Grow();
    }

    public void OnTurn(TurnDirection turn)
    {
        if (turn == TurnDirection.Left)
        {
            if (nextType == Type.Straight)
            {
                nextType = Type.Left;
            }
            // Allow the player to undo a right turn.
            else
            {
                nextType = Type.Straight;
            }
        }
        else
        {
            if (nextType == Type.Straight)
            {
                nextType = Type.Right;
            }
            // Allow the player to undo a left turn.
            else
            {
                nextType = Type.Straight;
            }
        }
    }
}
