using UnityEngine;
using System.Collections;

public class ShrinkingDestructable : MonoBehaviour {

	 [Tooltip("The main collider that will deactivate when player jumps on object")]
	 public BoxCollider2D main;
	 [Tooltip("The left collider that will activate and shrink when player jumps on object")]
	 public BoxCollider2D left;
	 [Tooltip("The right collider that will activate and shrink when player jumps on object")]
	 public BoxCollider2D right;
     [Tooltip("How fast a burnable object shrinks in the x direction")]
	 public float xBurnSpeed = .4f;
	 [Tooltip("How fast a burnable object shrinks in the y direction")]
	 public float yBurnSpeed = .4f;

	// Use this for initialization
	void Start () {


	}

	// Update is called once per frame
	void Update () {

	}


	public IEnumerator Shrink() {

		left.enabled = true;
		right.enabled = true;
		main.enabled = false;

		//shrink the boxes at the same rate
		while (left.size.x > 0.01 && left.size.y > 0.01) {
			left.size = new Vector2(left.size.x - (xBurnSpeed * Time.deltaTime), left.size.y - (yBurnSpeed * Time.deltaTime));
			right.size = new Vector2(right.size.x - (xBurnSpeed * Time.deltaTime), right.size.y - (yBurnSpeed * Time.deltaTime));
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		//kill the leaf
		gameObject.SetActive(false);

	}
}
