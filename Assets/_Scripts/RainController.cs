using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainController : MonoBehaviour {

	public int rainPool = 100;

	public GameObject rainDrop;

	private List<GameObject> rainDrops = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < rainPool; i++) {
			GameObject drop = Instantiate(rainDrop);
			drop.transform.parent = gameObject.transform;
			drop.SetActive (false);
			rainDrops.Add (drop);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
