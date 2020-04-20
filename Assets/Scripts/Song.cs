using System.Collections;
using UnityEngine;

public class Song : MonoBehaviour {
    bool playing = false;

    public AudioSource songAudioSource;
    public AudioSource gameOverAudioSource;
    float lastTime = 0;
    float start = 0;
    float beatTime = .8f;

    // Start is called before the first frame update
    void Start() {
        StartSong();
    }

    void StartSong() {
        StartCoroutine(PlaySong());
    }

    private IEnumerator PlaySong() {
        yield return new WaitForSecondsRealtime(1.0f);
        songAudioSource.Play();
        start = Time.fixedUnscaledTime;
        lastTime = start;
        playing = true;
    }

    void TriggerGameOver() {
        playing = false;
        songAudioSource.Stop();
        gameOverAudioSource.Play();
    }

    // Update is called once per frame
    void Update() {
        float timeDiff = Time.fixedUnscaledTime - lastTime;
        // lil bit of fudge factor to allow for frames that land just before the beat
        if (playing && timeDiff > (beatTime - 0.015))
        {
            lastTime = Time.fixedUnscaledTime - (timeDiff - beatTime);
            float timeSinceStart = Time.fixedUnscaledTime - start;
            gameObject.SendMessage("OnBeat");
        }

        if (Input.GetKeyDown("a")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Left);
        }
        if (Input.GetKeyDown("d")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Right);
        }
    }
}