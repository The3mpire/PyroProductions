using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {
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
        left.enabled = false;
        right.enabled = false;

        StartCoroutine(Shrink(null));
    }
	
	// Update is called once per frame
	void Update () {
	    
	}


    public IEnumerator Shrink(BoxCollider2D box) {
        //shrink the boxes at the same rate
        while (box.transform.localScale.x > 0) {
            box.transform.localScale = new Vector3(box.transform.localScale.x - (xBurnSpeed * Time.deltaTime), box.transform.localScale.y - (yBurnSpeed * Time.deltaTime));
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        //kill the leaf
        box.GetComponentInParent<GameObject>().SetActive(false);

    }
}
