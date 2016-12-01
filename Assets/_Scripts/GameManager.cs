using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

	public bool playerDead = false;

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

	public static void SetPlayerDead(bool status){
		instance.playerDead = status;
	}

	public static bool GetPlayerDead(){
		return instance.playerDead;
	}

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame(){
        SceneManager.LoadScene("LevelOne");
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
		//loads the next scene as long as there is one & saves playerPrefs
        PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex);
        //TODO put some type of loading thing to display
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        instance.NextLevel();

        Time.timeScale = 1;


    }
}
