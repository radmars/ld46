using UnityEngine;
using UnityEngine.Tilemaps;

public class Bud {
    /// I'm too lazy to just do this with vector shit and rotations.
    public TravelDirection travel = TravelDirection.Up;

    private PlantTileType nextType = PlantTileType.Straight;

    private PlantTilePhase phase = PlantTilePhase.A;

    public Vector3Int location;

    public void Grow(TilePlant plant) {
        // TODO Add self collision.
        var newHeading = GetNewHeading(nextType);
        var newLocation = Travel(newHeading);
        var newPhase = GetNewPhase();

        plant.UpdateLocation(location, phase, nextType, travel);
        plant.UpdateLocation(newLocation, newPhase, PlantTileType.Bud, newHeading);

        location = newLocation;
        travel = newHeading;
        phase = newPhase;
        nextType = PlantTileType.Straight;
    }

    // Phase swaps on straight segments, but outputs of turns and T-junctions are always phase B.
    private PlantTilePhase GetNewPhase() {
        if (nextType == PlantTileType.Straight) {
            return phase == PlantTilePhase.A ? PlantTilePhase.B : PlantTilePhase.A;
        }
        return PlantTilePhase.B;
    }

    private Vector3Int Travel(TravelDirection direction) {
        return location + GetDirectionVector(direction);
    }

    private TravelDirection GetNewHeading(PlantTileType nextType) {
        switch (nextType) {
            case PlantTileType.Straight:
                return travel;
            case PlantTileType.Left:
                return GetTurnDirection(TurnDirection.Left);
            case PlantTileType.Right:
                return GetTurnDirection(TurnDirection.Right);
            case PlantTileType.Tee:
                return GetTurnDirection(TurnDirection.Right);
            default:
                throw new System.ArgumentOutOfRangeException(string.Format("How did we get here: {0}", nextType));
        }
    }

    private Vector3Int GetDirectionVector(TravelDirection dir) {
        switch (dir) {
            case TravelDirection.Up:
                return Vector3Int.up;
            case TravelDirection.Down:
                return Vector3Int.up * -1;
            case TravelDirection.Left:
                return Vector3Int.right * -1;
            // case TravelDirection.Right:
            default:
                return Vector3Int.right;
        }
    }

    private void SetDirection(TravelDirection direction) {
        this.travel = direction;
    }

    public void Turn(TurnDirection turn) {
        this.nextType = GetNextType(turn);
    }

    private PlantTileType GetNextType(TurnDirection input) {
        switch (input) {
            case TurnDirection.Left:
                switch (nextType) {
                    case PlantTileType.Straight:
                        return PlantTileType.Left;
                    default:
                        return PlantTileType.Straight;
                }
            default:
                switch (nextType) {
                    case PlantTileType.Straight:
                        return PlantTileType.Right;
                    default:
                        return PlantTileType.Straight;
                }
        }
    }

    // Split a new bud to the left. "This" one will go right.
    public Bud Split() {
        var bud = new Bud();
        var direction = GetTurnDirection(TurnDirection.Left);
        this.Turn(TurnDirection.Right);
        bud.location = GetDirectionVector(direction) + location;
        bud.SetDirection(direction);
        nextType = PlantTileType.Tee;
        return bud;
    }

    /// Figure out what the direction we'd be facing if we turned the given direction.
    public TravelDirection GetTurnDirection(TurnDirection dir) {
        switch (this.travel) {
            case TravelDirection.Up:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Left;
                    // case TurnDirection.Right:
                    default: 
                        return TravelDirection.Right;
                }
            case TravelDirection.Left:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Down;
                    // case TurnDirection.Right:
                    default:
                        return TravelDirection.Up;
                }
            case TravelDirection.Down:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Right;
                    // case TurnDirection.Right:
                    default:
                        return TravelDirection.Left;
                }
            // case TravelDirection.Right:
            default:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Up;
                    // case TurnDirection.Right:
                    default:
                        return TravelDirection.Down;
                }
        }
    }
}