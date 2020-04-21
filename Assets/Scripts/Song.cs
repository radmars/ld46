using System.Collections;
using UnityEngine;

public class Song : MonoBehaviour {
    bool playing = false;

    public AudioSource songAudioSource;
    public AudioSource gameOverAudioSource;
    float lastTime = 0;
    float start = 0;
    float beatTime = .8f;

    // end = lose player controls & fade, win = go directly to win screen
    float endTimeSeconds = 345.6f;
    float winTimeSeconds = 361.0f;

    float gameOverTime = 9.0f;

    SpriteRenderer fadeOverlay;

    // Start is called before the first frame update
    void Start() {
        StartSong();

        fadeOverlay = GameObject.Find("FadeOverlay").GetComponent<SpriteRenderer>();
        Color fadeColor = fadeOverlay.color;
        fadeColor.a = 0.0f;
        fadeOverlay.color = fadeColor;
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
        StartCoroutine(BackToSplash());
    }

    private IEnumerator BackToSplash() {
        yield return new WaitForSecondsRealtime(gameOverTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene("splash-menu");
    }

    // Update is called once per frame
    void Update() {
        float timeSinceStart = Time.fixedUnscaledTime - start;
        if (timeSinceStart >= endTimeSeconds) {
            playing = false;

            Color fadeColor = fadeOverlay.color;
            fadeColor.a = (timeSinceStart - endTimeSeconds) / (winTimeSeconds - endTimeSeconds);
            fadeOverlay.color = fadeColor;
        }
        if (timeSinceStart >= winTimeSeconds) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("win");
        }

        float timeDiff = Time.fixedUnscaledTime - lastTime;

        // lil bit of fudge factor to allow for frames that land just before the beat
        if (playing && timeDiff > (beatTime - 0.015)) {
            lastTime = Time.fixedUnscaledTime - (timeDiff - beatTime);
            gameObject.SendMessage("OnBeat");
        }

        if (Input.GetKeyDown("a") || Input.GetKeyDown("left")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Left);
        }
        if (Input.GetKeyDown("d") || Input.GetKeyDown("right")) {
            gameObject.SendMessage("OnTurn", TurnDirection.Right);
        }
    }
}