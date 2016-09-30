using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainController : MonoBehaviour {

    [Tooltip("The number of raindrops for the level")]
	public int rainPool = 100;
    [Tooltip("The leftmost bound for rain drop spawns")]
    public float beginRange = 0;
    [Tooltip("The rightmost bound for rain drop spawns")]
    public float endRange = 10;
    [Tooltip("The height all drops will spawn at")]
    public float height = 6;
    [Tooltip("How fast the drops will spawn(in seconds)")]
    public float spawnRate = .3f;

	public GameObject rainDrop;

	private List<GameObject> rainDrops = new List<GameObject> ();

	// Use this for initialization
	void Start () {
        // add all the raindrops into the pool
		for (int i = 0; i < rainPool; i++) {
			GameObject drop = Instantiate(rainDrop);
			drop.transform.parent = gameObject.transform;
			drop.SetActive (false);
			rainDrops.Add (drop);
		}

        //set up the spawning
        InvokeRepeating("spawnDrops", 0f, spawnRate);
	}
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate() {
        
    }

    void spawnDrops() {

        foreach (GameObject drop in rainDrops) {
            if (!drop.activeSelf) {
                float rand = Random.Range(beginRange, endRange);
                drop.transform.position = new Vector3(rand, height, 0);

                drop.SetActive(true);
                return;
            }
        }
    }
}
