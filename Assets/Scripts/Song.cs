using System.Collections;
using UnityEngine;

public class Song : MonoBehaviour {
    bool playing = false;

    public AudioSource songAudioSource;

    // Start is called before the first frame update
    void Start() {
        StartSong();
    }

    void StopSong() {
        playing = false;
    }

    void StartSong() {
        songAudioSource.Play();
        StartCoroutine(PlaySong());
    }

    private IEnumerator PlaySong() {
        var start = Time.fixedUnscaledTime;
        playing = true;
        while (Time.fixedUnscaledTime - start < 1000 && playing) {
            yield return new WaitForSecondsRealtime(.8f);
            gameObject.SendMessage("OnBeat");
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("a")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Left);
        }
        if (Input.GetKeyDown("d")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Right);
        }
    }
}