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

        GetComponent<Animator>().SetBool("hasEntered", true);

    }

	public void Disintegrate(){
        //TODO animate ash
        GetComponent<Animator>().SetBool("hasEntered", false);

        gameObject.SetActive (false);
	}
}
