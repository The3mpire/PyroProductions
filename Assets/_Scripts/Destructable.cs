using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public AudioClip burnSound;
    public float burnLength = 0.3f;

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
      
        //TODO isSpawning??
        GetComponentInChildren<RainSpawner>().gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<RainSpawner>().gameObject.GetComponent<Collider2D>().enabled = false;
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
        
		GetComponent<SpriteRenderer> ().enabled = true;
		GetComponent<Collider2D> ().enabled = true;
		GetComponentInChildren<Collider2D> ().enabled = true;
        GetComponentInChildren<RainSpawner>().gameObject.GetComponent<SpriteRenderer>().enabled = true;
        GetComponentInChildren<RainSpawner>().gameObject.GetComponent<Collider2D>().enabled = true;

    }

    public IEnumerator BurnThenDisintegrate() {
        Burn();
        yield return new WaitForSeconds(burnLength);
        Disintegrate();
    }
}
