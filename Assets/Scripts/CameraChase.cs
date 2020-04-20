using UnityEngine;

public class CameraChase : MonoBehaviour {

    public new Camera camera;
    private Vector3 desiredPosition;
    private float updateStart = -1;
    private float startingHeight = 0;

    private TilePlant vine;

    // Start is called before the first frame update
    void Start() {
        vine = GetComponent<TilePlant>();
        startingHeight = camera.transform.position.y;
    }

    public void BudsMoved() {
        var buds = vine.GetBuds();
        if(buds.Count > 0) {
            desiredPosition = new Vector3();
            foreach (var bud in buds) {
                desiredPosition += new Vector3(bud.location.x, bud.location.y, 0) / (float)buds.Count;
            }

            desiredPosition = new Vector3(
                desiredPosition.x,
                Mathf.Max(startingHeight, desiredPosition.y),
                camera.transform.position.z
            );
            updateStart = Time.fixedUnscaledTime;
        }
    }

    // Update is called once per frame
    void Update() {
        if (desiredPosition != camera.transform.position && updateStart > 0) {
            camera.SendMessage("MoveCamera", Vector3.Slerp(
                camera.transform.position,
                desiredPosition,
                (Time.fixedUnscaledTime - updateStart) / .65f
            ));
        }
    }
}