using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainSpawner : MonoBehaviour {


    [Tooltip("The number of raindrops available to this spawner")]
    public int rainPool = 20;
    [Tooltip("How fast the drops will spawn(in seconds)")]
    public float spawnRate = .3f;

    public GameObject rainDrop;

    public bool canSpawn = true;

    private bool isSpawning = true;

    [Tooltip("")]
    public float startSpawnDelay = .5f;

    private List<GameObject> rainDrops = new List<GameObject>();

    // Use this for initialization
    void Start () {
        // add all the raindrops into the pool
        for (int i = 0; i < rainPool; i++) {
            GameObject drop = Instantiate(rainDrop);
            drop.transform.parent = gameObject.transform;
            drop.SetActive(false);
            rainDrops.Add(drop);
        }
        StartSpawning();
    }

    public void StartSpawning() {
        if (canSpawn) {
            StartCoroutine(StartSpawning(startSpawnDelay));
            isSpawning = true;
        }
    }

    public void StopSpawning() {
        CancelInvoke("spawnDrops");
        isSpawning = false;
    }

    IEnumerator StartSpawning(float delay) {
        yield return new WaitForSeconds(delay);
        
        //set up spawning
        InvokeRepeating("spawnDrops", 0f, spawnRate);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void spawnDrops() {
        foreach (GameObject drop in rainDrops) {
            if (!drop.activeSelf && isSpawning) {
                drop.transform.position = gameObject.transform.position;
                drop.SetActive(true);
                return;
            }
        }
    }
}
