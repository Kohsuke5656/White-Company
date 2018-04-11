using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour {

	public int currentlyAtScene;

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000;
	private float alpha = 1.0f;
	private int fadeDir = -1;

	private bool switchingFlg;

    public int maxScene;

	void Awake() {
		DontDestroyOnLoad (gameObject);
		SceneManager.sceneLoaded += OnLevelFinishedLoading;

		currentlyAtScene = 0;
		switchingFlg = false;
	}

	// Use this for initialization
	void Start () {
		currentlyAtScene = 1;
		SceneManager.LoadScene (1, LoadSceneMode.Single);
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextScene();
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            BackToTitle();
        }
    }

    public void ReloadScene()
    {
        SwitchScene(currentlyAtScene);
    }

	public void SwitchScene(string s) {
		if (!switchingFlg) {
			switchingFlg = true;
			StartCoroutine (DoLoadScene (s));
		}
	}

    public void SwitchScene(int s)
    {
        if (!switchingFlg)
        {
            switchingFlg = true;
            StartCoroutine(DoLoadScene(s));
        }
    }

    public void BackToTitle() {
		if (!switchingFlg) {
			switchingFlg = true;
			currentlyAtScene = 1;
			StartCoroutine (DoLoadScene ("Title"));
		}
	}

	public void NextScene() {
		if (!switchingFlg) {
			switchingFlg = true;
            if (currentlyAtScene >= maxScene - 1)
                currentlyAtScene = 0;
			StartCoroutine (DoLoadScene (++currentlyAtScene));
		}
	}

	private IEnumerator DoLoadScene(string s) {
		BeginFade (1);
		yield return new WaitForSeconds (fadeSpeed);
		SceneManager.LoadScene (s, LoadSceneMode.Single);
	}
	private IEnumerator DoLoadScene(int s) {
		BeginFade (1);
		yield return new WaitForSeconds (fadeSpeed);
		SceneManager.LoadScene (s, LoadSceneMode.Single);
	}

	void OnGUI()
	{
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	public float BeginFade(int direction)
	{
		fadeDir = direction;
		return fadeSpeed;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		BeginFade (-1);
		switchingFlg = false;
	}

}
