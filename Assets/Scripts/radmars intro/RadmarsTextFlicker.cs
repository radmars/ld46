using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class RadmarsTextFlicker : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	public Sprite mars;
	public Sprite intro1;
	public Sprite intro2;
	int counter = 0;
	public string nextScene;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetActive(mars);
	}

	void Update()
	{
		if (Input.anyKey)
		{
			FinishIntro();
		}
	}

	void SetActive(Sprite r)
	{
		spriteRenderer.sprite = r;
	}

	public void FixedUpdate()
	{
		if (this.counter < 130) SetActive(mars);
		else if (this.counter < 135) SetActive(intro2);
		else if (this.counter < 140) SetActive(intro1);
		else if (this.counter < 145) SetActive(intro2);
		else if (this.counter < 150) SetActive(intro1);
		else if (this.counter < 155) SetActive(intro2);
		else if (this.counter < 160) SetActive(intro1);
		else if (this.counter < 165) SetActive(intro2);
		else SetActive(intro1);
		this.counter++;
		if (counter > 300)
		{
			FinishIntro();
		}
	}

	void FinishIntro()
	{
		SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
	}
}
