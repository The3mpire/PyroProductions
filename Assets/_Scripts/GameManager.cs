using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

	// Use this for initialization
	void Start () {
	
	}

    void Awake() {
        // First we check if there are any other instances conflicting

        if (instance != null && instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void ExitLevel() {
        SceneManager.LoadScene("MainMenu");
    }

    public static void StartGame() {
        SceneManager.LoadScene(1);
    }

	public static void ExitGame() {
        Application.Quit();
    }

	public static void NextLevel(int level){
		//loads the next scene as long as there is one & saves playerPrefs
		if (SceneManager.GetActiveScene ().buildIndex != level) {
            PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex);
            //TODO put some time of loading thing to display
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene ().buildIndex + 1);
		}
		//TODO otherwise call playerwin
		else {
			
		}
	}
}
