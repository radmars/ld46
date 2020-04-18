﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{
    bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        StartSong();
    }

    void StopSong() {
        playing = false;
    }

    void StartSong() {
        StartCoroutine(PlaySong());
    }

    private IEnumerator PlaySong() {
        var start = Time.fixedUnscaledTime;
        playing = true;
        while(Time.fixedUnscaledTime - start < 10 && playing) {
            yield return new WaitForSecondsRealtime(.7f);
            gameObject.SendMessage("OnBeat");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Left);
        }
        if (Input.GetKeyDown("d")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Right);
        }
    }
}
