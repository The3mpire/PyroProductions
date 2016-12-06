using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

	public bool playerDead = false;

	public Button continueGame;
	public Image continueText;

	private int points = 0;

	// Use this for initialization
	void Start () {
	}

    void Awake() {
        // First we check if there are any other instances conflicting

        if (instance != null && instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
			
		if (PlayerPrefs.GetInt ("level") != 0 && continueGame != null) {
			continueGame.interactable = true;
			continueText.color = new Color (1, 1, 1);
		}

        // Here we save our singleton instance
        instance = this;

        //DontDestroyOnLoad(gameObject);
    }


	public static void SetPoints(int score){
		instance.points = score;
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
		Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame(){
		Cursor.visible = false;
        SceneManager.LoadScene(0);
    }

	public void ContinueGame(){
		Cursor.visible = false;
		SceneManager.LoadScene(PlayerPrefs.GetInt("level"));
	}

    public void ExitLevel() {
		Cursor.visible = false; 
		SceneManager.LoadScene(0);
    }

    public void StartGame() {
		PlayerPrefs.SetInt ("points", 0);
		PlayerPrefs.SetInt ("level", 0);
		Cursor.visible = false;
		SceneManager.LoadScene(1);
    }

	public void ExitGame() {
        Application.Quit();
    }

	public void NextLevel(){
		//loads the next scene as long as there is one & saves playerPrefs
        PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex + 1);
		PlayerPrefs.SetInt("points", PlayerPrefs.GetInt("points") + points);

		Cursor.visible = false;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        Time.timeScale = 1;
    }
}
