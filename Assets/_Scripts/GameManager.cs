using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private int levelCount = SceneManager.sceneCountInBuildSettings ();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitLevel() {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

	public void ExitGame() {
        Application.Quit();
    }

	public void NextLevel(){
		//loads the next scene as long as there is one
		if (SceneManager.GetActiveScene ().buildIndex != levelCount) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex++);
			PlayerPrefs.SetInt ("level", SceneManager.GetActiveScene().buildIndex);
		}
		//TODO otherwise call playerwin
		else {
			//GameObject.find ("Player");
		}
	}
}
