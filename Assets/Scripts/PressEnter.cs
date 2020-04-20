using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressEnter : MonoBehaviour {

    public string nextScene;
    public bool useText = false;

	void Start () {
        if (useText)
        {
            StartCoroutine(Blink());
        }
    }

	private IEnumerator Blink()
	{
		while(true)
		{
			yield return new WaitForSeconds(.5f);
			GetComponent<Text>().enabled = !GetComponent<Text>().enabled;
		}
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Return)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
		}
	}
}
