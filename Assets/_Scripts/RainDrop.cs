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
        // stop falling if we're past the ground
        if(gameObject.transform.position.y < despawn) {
            gameObject.SetActive(false);
        }
	}


	void OnTriggerEnter2D(Collider2D col){
		switch (col.tag) {
			case "Ground":
				gameObject.SetActive (false);
				// plop animation?
				break;
			case "Player":
				gameObject.SetActive (false);
				//TODO trigger steam animation
				break;
		}
	}
		
	void FixedUpdate(){
	}

	public void spawn(int xLoc){
		transform.position = new Vector3 (xLoc, 7, 0);
		gameObject.SetActive (true);
	}
}