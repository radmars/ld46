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

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
            Debug.Log("Lef");
        }
        var newBuds = new List<Bud>();
        foreach (var bud in buds) {
            if (Input.GetKeyDown("z")) {
                newBuds.Add(bud.Split(Bud.Turn.Left));
            }
            if (Input.GetKeyDown("c")) {
                newBuds.Add(bud.Split(Bud.Turn.Right));
            }
        }
        buds.AddRange(newBuds);
    }
}