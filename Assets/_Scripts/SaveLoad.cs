using UnityEngine;
using System.Collections;

public class SaveLoad : MonoBehaviour {

	// Static singleton property
	public static SaveLoad Instance { get; private set; }

	//the level the user is on
	public int level = 0;

	void Awake()
	{
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}

		// Here we save our singleton instance
		Instance = this;

		// Furthermore we make sure that we don't destroy between scenes (this is optional)
		DontDestroyOnLoad(gameObject);
	}
}
