using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bud : MonoBehaviour {
    public enum TravelDirection {
        Up,
        Down,
        Left,
        Right,
    }

    public enum Turn {
        Left,
        Right,
    }

    private const float GROWTH_RATE = 1.20f;

    /// I'm too lazy to just do this with vector shit and rotations.
    public TravelDirection travel = TravelDirection.Up;

    public void Start() {
        SetDirection(this.travel);
    }

    public Vector3 GetDirectionVector(TravelDirection dir) {
        switch (dir) {
            case TravelDirection.Up:
                return Vector3.up;
            case TravelDirection.Down:
                return Vector3.up * -1;
            case TravelDirection.Left:
                return Vector3.right * -1;
            case TravelDirection.Right:
                return Vector3.right;
            default:
                throw new System.ArgumentOutOfRangeException("This is impossible");
        }
    }

    private void SetDirection(TravelDirection direction) {
        this.travel = direction;
        GetComponent<Rigidbody>().velocity = GetDirectionVector(direction) * GROWTH_RATE;
    }

    public Bud Split(Turn split) {
        var newObj = GameObject.Instantiate(this.gameObject);
        var newBud = newObj.GetComponent<Bud>();
        newBud.gameObject.transform.position = transform.position + GetOffset(split);
        newBud.SetDirection(GetTurnDirection(split));
        return newBud;
    }

    public Vector3 GetOffset(Turn split) {
        var dir = GetDirectionVector(GetTurnDirection(split));
        return dir * 1.1f;
    }

    /// Figure out what the direction we'd be facing if we turned the given direction.
    public TravelDirection GetTurnDirection(Turn dir) {
        switch (this.travel) {
            case TravelDirection.Up:
                switch (dir) {
                    case Turn.Left:
                        return TravelDirection.Left;
                    case Turn.Right:
                        return TravelDirection.Right;
                }
                break;
            case TravelDirection.Left:
                switch (dir) {
                    case Turn.Left:
                        return TravelDirection.Down;
                    case Turn.Right:
                        return TravelDirection.Up;
                }
                break;
            case TravelDirection.Down:
                switch (dir) {
                    case Turn.Left:
                        return TravelDirection.Right;
                    case Turn.Right:
                        return TravelDirection.Left;
                }
                break;
            case TravelDirection.Right:
                switch (dir) {
                    case Turn.Left:
                        return TravelDirection.Up;
                    case Turn.Right:
                        return TravelDirection.Down;
                }
                break;
        }
        throw new System.ArgumentOutOfRangeException("This is impossible");
    }

    // Update is called once per frame
    void Update() {

    }
}