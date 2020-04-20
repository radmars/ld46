using System.Collections.Generic;
using UnityEngine;

public class Bud {
    /// I'm too lazy to just do this with vector shit and rotations.
    public TravelDirection travel = TravelDirection.Up;

    private PlantTileType nextType = PlantTileType.Straight;

    private PlantTilePhase phase = PlantTilePhase.B;

    public Vector3Int location;

    private Stack<TravelDirection> pendingSplit = new Stack<TravelDirection>();

    // Attempt to grow this bud, taking into account possible collisions with the bounding box,
    // existing plants, and the stage. Returns true if the growth was successful, false if the
    // bud died after growing.
    public bool TryToGrow(TilePlant plant) {
        TravelDirection newHeading = pendingSplit.Count > 0 ? pendingSplit.Pop() : GetNewHeading(nextType);

        var newLocation = Travel(newHeading);
        var newPhase = GetNewPhase();

        plant.UpdateLocation(location, phase, nextType, travel);
        while (pendingSplit.Count > 0)
        {
            var splitDirection = pendingSplit.Pop();
            var splitLocation = location + GetDirectionVector(splitDirection);

            if (!FatalLocation(plant, splitLocation, false)) {
                plant.AddBud(new Bud
                {
                    location = splitLocation,
                    travel = splitDirection,
                });
                plant.UpdateLocation(splitLocation, newPhase, PlantTileType.Bud, splitDirection);
            }
        }

        nextType = PlantTileType.Straight; // May be overridden by CheckCollisions.
        if (FatalLocation(plant, newLocation, true))
        {
            return false;
        }
        CheckForSplitter(plant, newLocation);

        plant.UpdateLocation(newLocation, newPhase, PlantTileType.Bud, newHeading);

        location = newLocation;
        travel = newHeading;
        phase = newPhase;

        return true;
    }

    // Returns true if the specified location will kill buds. If playSound is true, will play an
    // appropriate sound for the poor, poor plant.
    private bool FatalLocation(TilePlant plant, Vector3Int location, bool playSound)
    {
        // First, worry about the world boundaries...
        if (
            location.x < StageConstants.leftLimit
            || location.x > StageConstants.rightLimit
            || location.y < StageConstants.bottomLimit
            )
        {
            if (playSound)
            {
                plant.hitLowAudioSource.Play();
            }
            return true;
        }
        // Next, plant self-collisions...
        if (plant.plantTilemap.GetTile(location) != null)
        {
            if (playSound)
            {
                plant.hitAudioSource.Play();
            }
            return true;
        }

        // Finally, stage collisions.
        var tile = plant.stageTilemap.GetTile(location);
        if (!tile || tile.name != "spike")
        {
            return false;
        }
        // tile.name == "spike"
        if (playSound)
        {
            plant.hitAudioSource.Play();
        }
        return true;
    }

    private void CheckForSplitter(TilePlant plant, Vector3Int location)
    {
        var tile = plant.stageTilemap.GetTile(location);
        if (!tile)
        {
            return;
        }
        if (tile.name.StartsWith("splitter_"))
        {
            plant.branchAudioSource.Play();
            nextType = PlantTileType.Tee;
        }
        switch (tile.name)
        {
            case "splitter_dr":
                pendingSplit.Push(TravelDirection.Down);
                pendingSplit.Push(TravelDirection.Right);
                return;
            case "splitter_ld":
                pendingSplit.Push(TravelDirection.Left);
                pendingSplit.Push(TravelDirection.Down);
                return;
            case "splitter_lr":
                pendingSplit.Push(TravelDirection.Left);
                pendingSplit.Push(TravelDirection.Right);
                return;
            case "splitter_lu":
                pendingSplit.Push(TravelDirection.Left);
                pendingSplit.Push(TravelDirection.Up);
                return;
            case "splitter_ud":
                pendingSplit.Push(TravelDirection.Up);
                pendingSplit.Push(TravelDirection.Down);
                return;
            case "splitter_ur":
                pendingSplit.Push(TravelDirection.Up);
                pendingSplit.Push(TravelDirection.Right);
                return;
            default:
                Debug.LogError("Missing collision case");
                return;
        }
    }

    // Phase swaps on straight segments, but outputs of turns and T-junctions are always phase A???
    private PlantTilePhase GetNewPhase() {
        if (nextType == PlantTileType.Straight) {
            return phase == PlantTilePhase.A ? PlantTilePhase.B : PlantTilePhase.A;
        }
        return PlantTilePhase.A;
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
        // If there's a pending split, the player has no control over this bud.
        if (pendingSplit.Count > 0)
        {
            return;
        }
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