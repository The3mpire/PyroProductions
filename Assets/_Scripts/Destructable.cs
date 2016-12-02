using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public AudioClip burnSound;

	[Tooltip("How long the leaf will take to respawn")]	
	public float leafRespawn = 1f;

   // public AnimationClip destructAnim;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

	}

	public void Burn(){
        GetComponent<Animator>().SetBool("hasEntered", true);
		SoundManager.instance.PlaySingle(burnSound);
    }

	public void Disintegrate(){
        GetComponent<Animator>().SetBool("hasEntered", false);
		StopAllCoroutines ();
        StartCoroutine(ashAnim(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length));
	}

    private IEnumerator ashAnim(float time)
    {
		GetComponent<Collider2D> ().enabled = false;
		GetComponentInChildren<Collider2D> ().enabled = false;

        yield return new WaitForSeconds(time);

		GetComponent<SpriteRenderer> ().enabled = false;
		GetComponent<Animator> ().SetTrigger ("reset");
		StartCoroutine (LeafCooldown (leafRespawn));
    }

	private IEnumerator LeafCooldown(float time){
		yield return new WaitForSeconds(time);

		//TODO reset the object (use prefabs?)
		GetComponent<SpriteRenderer> ().enabled = true;
		GetComponent<Collider2D> ().enabled = true;
		GetComponentInChildren<Collider2D> ().enabled = true;

	}
}
