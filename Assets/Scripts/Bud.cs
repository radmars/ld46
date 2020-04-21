using System.Collections.Generic;
using UnityEngine;

public class Bud {
    /// I'm too lazy to just do this with vector shit and rotations.
    public TravelDirection travel = TravelDirection.Up;

    private PlantTileType nextType = PlantTileType.Straight;

    private PlantTilePhase phase = PlantTilePhase.B;

    public Vector3Int location;

    class Split {
        public TravelDirection split1;
        public TravelDirection split2;
        public TravelDirection rotation;
        public PlantTileType type;

        public Split(TravelDirection split1, TravelDirection split2, TravelDirection rotation, PlantTileType type) {
            this.split1 = split1;
            this.split2 = split2;
            this.rotation = rotation;
            this.type = type;
        }
    }

    private Split pendingSplit;

    // Attempt to grow this bud, taking into account possible collisions with the bounding box,
    // existing plants, and the stage. Returns true if the growth was successful, false if the
    // bud died after growing.
    public bool TryToGrow(TilePlant plant) {
        // If we split, hijack this update to show the new split tile.
        if(pendingSplit != null) {
            nextType = pendingSplit.type;
            travel = pendingSplit.rotation;
        }
        plant.UpdateLocation(location, phase, nextType, travel);

        var newHeading = GetNewHeading(nextType);
        var newLocation = Travel(newHeading);
        var newPhase = GetNewPhase();

        // If we split, try to grow split1, replace heading for the main bud.
        if (pendingSplit != null) {
            var splitDirection = pendingSplit.split1;
            var splitLocation = location + GetDirectionVector(splitDirection);

            if (!FatalLocation(plant, splitLocation, false)) {
                plant.AddBud(new Bud {
                    location = splitLocation,
                    travel = splitDirection,
                    phase = newPhase,
                });
                plant.UpdateLocation(splitLocation, newPhase, PlantTileType.Bud, splitDirection);
            }

            newHeading = pendingSplit.split2;
            newLocation = Travel(newHeading);
            pendingSplit = null;
        }

        nextType = PlantTileType.Straight; // May be overridden by CheckCollisions.
        if (FatalLocation(plant, newLocation, true)) {
            return false;
        }

        // Needed for checking splitter rotations.
        location = newLocation;
        travel = newHeading;
        phase = newPhase;

        CheckForSplitter(plant, newLocation);

        plant.UpdateLocation(newLocation, newPhase, PlantTileType.Bud, newHeading);

        return true;
    }

    // Returns true if the specified location will kill buds. If playSound is true, will play an
    // appropriate sound for the poor, poor plant.
    private bool FatalLocation(TilePlant plant, Vector3Int location, bool playSound) {
        // First, worry about the world boundaries...
        if (
            location.x < StageConstants.leftLimit ||
            location.x > StageConstants.rightLimit ||
            location.y < StageConstants.bottomLimit
        ) {
            if (playSound) {
                plant.hitLowAudioSource.Play();
            }
            return true;
        }
        // Next, plant self-collisions...
        if (plant.plantTilemap.GetTile(location) != null) {
            if (playSound) {
                plant.hitAudioSource.Play();
            }
            return true;
        }

        // Finally, stage collisions.
        var tile = plant.stageTilemap.GetTile(location);
        if (!tile || tile.name != "spike") {
            return false;
        }
        // tile.name == "spike"
        if (playSound) {
            plant.hitAudioSource.Play();
        }
        return true;
    }

    private Split GetSplit(string splitter, TravelDirection incoming) {
        switch (incoming) {
            case TravelDirection.Up:
                switch (splitter) {
                    case "splitter_lr":
                        return new Split(TravelDirection.Left, TravelDirection.Right, TravelDirection.Up, PlantTileType.Tee);
                    case "splitter_lu":
                        return new Split(TravelDirection.Left, TravelDirection.Up, TravelDirection.Right, PlantTileType.Tee);
                        // case "splitter_ur":
                    default:
                        return new Split(TravelDirection.Up, TravelDirection.Right, TravelDirection.Left, PlantTileType.Tee);
                }
            case TravelDirection.Down:
                switch (splitter) {
                    case "splitter_lr":
                        return new Split(TravelDirection.Left, TravelDirection.Right, TravelDirection.Down, PlantTileType.Tee);
                    case "splitter_lu":
                        return new Split(TravelDirection.Left, TravelDirection.Up, TravelDirection.Down, PlantTileType.Right);
                    // case "splitter_ur":
                    default:
                        return new Split(TravelDirection.Up, TravelDirection.Right, TravelDirection.Down, PlantTileType.Left);
                }
            case TravelDirection.Left:
                switch (splitter) {
                    case "splitter_lr":
                        return new Split(TravelDirection.Left, TravelDirection.Right, TravelDirection.Left, PlantTileType.Straight);
                    case "splitter_lu":
                        return new Split(TravelDirection.Left, TravelDirection.Up, TravelDirection.Down, PlantTileType.Tee);
                        // case "splitter_ur":
                    default:
                        return new Split(TravelDirection.Up, TravelDirection.Right, TravelDirection.Left, PlantTileType.Right);
                }
            default:
                switch (splitter) {
                    case "splitter_lr":
                        return new Split(TravelDirection.Left, TravelDirection.Right, TravelDirection.Right, PlantTileType.Straight);
                    case "splitter_lu":
                        return new Split(TravelDirection.Left, TravelDirection.Up, TravelDirection.Right, PlantTileType.Left);
                        // case "splitter_ur":
                    default:
                        return new Split(TravelDirection.Up, TravelDirection.Right, TravelDirection.Down, PlantTileType.Tee);
                }
        }
    }

    private bool CheckForSplitter(TilePlant plant, Vector3Int location) {
        var tile = plant.stageTilemap.GetTile(location);
        if (!tile) {
            return false;
        }

        if (tile.name.StartsWith("splitter_")) {
            plant.branchAudioSource.Play();
            nextType = PlantTileType.Tee;
            pendingSplit = GetSplit(tile.name, travel);
            return true;
        }
        return false;
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
        if (pendingSplit != null) {
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