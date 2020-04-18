using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour {
    private List<Bud> buds;
    // Start is called before the first frame update
    void Start() {
        buds = new List<Bud>();
        buds.AddRange(GetComponentsInChildren<Bud>(false));
    }

    public void OnBeat() {
        foreach(var bud in buds) {
            bud.Move();
        }
    }

    public void OnTurn(TurnDirection turn) {
        foreach (var bud in buds) {
            bud.Turn(turn);
        }
    }

    public void BudDied(object bud) {
        buds.Remove((Bud) bud);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("z")) {
            var newBuds = new List<Bud>();
            foreach(var bud in buds) {
                newBuds.Add(bud.Split(TurnDirection.Left));
            }
            buds.AddRange(newBuds);
        }
    }
}