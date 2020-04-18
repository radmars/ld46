using UnityEngine;

public class Bud : MonoBehaviour {
    public enum TravelDirection {
        Up = 0,
        Down = 180,
        Left = 90,
        Right = 270,
    }

    private const float GRID_SIZE = 1.20f;

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
        transform.localRotation = Quaternion.Euler(0, 0, (int) direction);
    }

    public void Turn(TurnDirection turn) {
        SetDirection(GetTurnDirection(turn));
    }

    public Bud Split(TurnDirection split) {
        var newObj = GameObject.Instantiate(this.gameObject, transform.parent);
        var newBud = newObj.GetComponent<Bud>();
        newBud.gameObject.transform.position = transform.position + GetOffset(split);
        newBud.SetDirection(GetTurnDirection(split));
        return newBud;
    }

    public void Move() {
        this.transform.position += this.GetDirectionVector(this.travel) * GRID_SIZE;
    }

    public Vector3 GetOffset(TurnDirection split) {
        return GetDirectionVector(GetTurnDirection(split)) * GRID_SIZE;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("fatal")) {
            Kill();
        }
    }

    public void Kill() {
        Debug.Log(string.Format("{0} bud died", gameObject.name));
        gameObject.transform.parent.SendMessage("BudDied", this);
    }

    /// Figure out what the direction we'd be facing if we turned the given direction.
    public TravelDirection GetTurnDirection(TurnDirection dir) {
        switch (this.travel) {
            case TravelDirection.Up:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Left;
                    case TurnDirection.Right:
                        return TravelDirection.Right;
                }
                break;
            case TravelDirection.Left:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Down;
                    case TurnDirection.Right:
                        return TravelDirection.Up;
                }
                break;
            case TravelDirection.Down:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Right;
                    case TurnDirection.Right:
                        return TravelDirection.Left;
                }
                break;
            case TravelDirection.Right:
                switch (dir) {
                    case TurnDirection.Left:
                        return TravelDirection.Up;
                    case TurnDirection.Right:
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