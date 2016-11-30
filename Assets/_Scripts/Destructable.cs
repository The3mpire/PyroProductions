using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

   // public AnimationClip destructAnim;
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
        GetComponent<Animator>().SetBool("hasEntered", false);
        
        StartCoroutine(ashAnim(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length));
	}

    private IEnumerator ashAnim(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
