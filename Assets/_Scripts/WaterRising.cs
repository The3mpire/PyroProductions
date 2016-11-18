using UnityEngine;
using System.Collections;

public class WaterRising : MonoBehaviour {

	[Tooltip("how much the water rises per second")]
	public float raiseWater = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		//raise the water
		if (!GameManager.GetPlayerDead()) {			
			transform.position = new Vector2 (transform.position.x, transform.position.y + raiseWater);
		}
	}
}
