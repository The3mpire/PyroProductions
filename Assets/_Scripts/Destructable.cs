using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {
	// Use this for initialization
	void Start () {


	}

	// Update is called once per frame
	void Update () {

	}

	public void Burn(){
		//TODO call burn
		Debug.Log("in burn");
	}

	public void Disintegrate(){
		//TODO animate ash
		Debug.Log("in disinte");

		gameObject.SetActive (false);
	}
}
