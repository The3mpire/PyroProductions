using UnityEngine;
using System.Collections;

public class RainDrop : MonoBehaviour {

	[Tooltip("At what 'y' the raindrop will despawn")]
	public float despawn = -7;

	private Vector3 startPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!gameObject.active) {
			spawn (1);
		}
	}


	void OnTriggerEnter2D(Collider2D col){
		if(col.CompareTag("Ground")){
			gameObject.SetActive(false);
		}
	}


	void FixedUpdate(){
	}

	public void spawn(int xLoc){
		transform.position = new Vector3 (xLoc, 7, 0);
		gameObject.SetActive (true);
	}
}
// rain handler that has a list of raindrops
// each raindrop just turns itself off
// rain handler turns raindrops on and then sets them at a random x
