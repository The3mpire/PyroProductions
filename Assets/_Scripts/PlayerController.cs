using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

[RequireComponent(typeof(CharacterController2D), typeof(AnimationController2D))]
public class PlayerController : MonoBehaviour {

    private enum Direction {
        left, right, idle
    }

    public GameObject gameCamera;
    public GameObject healthBar;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject continuePanel;
	public GameObject pausePanel;

    public AudioClip jumpSound;
	public AudioClip steamSound;
    [Tooltip("How loud the steam sound plays")]
    public float steamVolume = .1f;


    public float walkSpeed = 3f;
    public float jumpHeight = 2f;
    public float gravity = -35f;
    [Tooltip("Player's maximum health")]
    public int maxHealth = 100;
    [Tooltip("Player's starting health")]
    public int startHealth = 20;
    [Tooltip("How much damage the player will take")]
    public int damage = 10;
    [Tooltip("The amount of health the player gets when consuming something")]
    public int food = 10;
    [Tooltip("The maximum size the player will be")]
    public float maxScale = 2;
    [Tooltip("The minimum size the player will be")]
    public float minScale = 0.4f;
	[Tooltip("The size of the particle based on the player.")]
	public float particleSize = 1f;
    [Tooltip("How long the player is invincible after taking damage")]
    public float invincibilityTime = 0.2f;
    [Tooltip("The base size of explode")]
    public float explodeRadius = 3f;

    private CharacterController2D _controller;
    private Animator _animator;
	private AnimationController2D animControl;
    //[SerializeField]
    private int currHealth = 0;
    private bool playerControl = true;
    private bool jump = false;
	private bool menu = false;
    private bool damageable = true;
    private bool canExplode = true;

    private int level;

    private Direction facing = Direction.right;

        
    // Use this for initialization
	void Start () {
        Time.timeScale = 1;

		healthBar.SetActive(true);

        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
		animControl = GetComponent<AnimationController2D>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);
        currHealth = startHealth;

        updateHealth();

        level = SceneManager.sceneCountInBuildSettings;
    }
	
	// Update is called once per frame
	void Update () {
        
		if (Input.GetButtonDown("Cancel") && !menu) {
			pausePanel.SetActive (true);
			menu = true;
			Time.timeScale = 0;
		} else if (Input.GetButtonDown("Cancel") && menu) {
			pausePanel.SetActive (false);
			menu = false;
			Time.timeScale = 1;
		}   

        // is player dead
        if (playerControl) {
            playerInput();
        }
        Vector3 velocity = _controller.velocity;

        velocity.x = 0;

        if (facing == Direction.left) {
			velocity.x = -1 * walkSpeed;
            if (_controller.isGrounded) {
				_animator.SetBool("isGrounded", true);
				_animator.SetBool ("isMoving", true);
            }
			animControl.setFacing("Left");
        }
        else if (facing == Direction.right) {
            velocity.x = walkSpeed;
            if (_controller.isGrounded) {
				_animator.SetBool("isGrounded", true);
				_animator.SetBool ("isMoving", true);
            }
			animControl.setFacing("Right");
        }
        else {
			_animator.SetBool("isMoving", false);
        }

        if (jump) {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);

            //jump anim
            _animator.SetBool("isGrounded", false);
            SoundManager.instance.PlaySingle(jumpSound);

            jump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        if (playerControl) {
            _controller.move(velocity * Time.deltaTime);
        }

		if (_controller.isGrounded) {
			_animator.SetBool ("isGrounded", true);
		}

    }
		
    void FixedUpdate() {

    }

    private void playerInput() {

        //Animation
        if (Input.GetAxis("Horizontal") < 0) {
            facing = Direction.left;
        }
        else if (Input.GetAxis("Horizontal") > 0) {
            facing = Direction.right;            
        }
        else {
            //play idle animation
            facing = Direction.idle;
        }
        //jump
        if (Input.GetAxis("Jump") > 0 && _controller.isGrounded) {
            jump = true;
        }
        //explode
        else if (Input.GetButtonDown("Explode") && canExplode && _controller.isGrounded) {
            PlayerExplode();
            canExplode = false;
            damageable = false;
            playerControl = false;
            StartCoroutine(ExplodeCooldowns(_animator.GetCurrentAnimatorClipInfo(0).Length));
        }
    }

    private IEnumerator ExplodeCooldowns(float time) {
        yield return new WaitForSeconds(time);
        canExplode = true;
        damageable = true;
        playerControl = true;
    }


   void OnTriggerEnter2D(Collider2D col) {
        switch (col.tag) {
            case "KillZ":
                PlayerDeath();
                break;
            case "Damaging":
                if (damageable) {
                    PlayerDamage(damage);
                    damageable = false;
                    StartCoroutine(DamageCoolDown(invincibilityTime));
                }
                break;
			case "NextLevel":
				PlayerNextLevel ();
				break;
            case "Win":
                PlayerWin();
                break;
            case "Growing":
                col.gameObject.SetActive(false);
                PlayerGrow(food);
	            break;
			case "Destructable":	
				col.GetComponentInParent<Destructable>().Burn();					
				break;
        }
    }

    private IEnumerator DamageCoolDown(float time) {
        yield return new WaitForSeconds(time);
        damageable = true;
    }

    void OnTriggerStay2D(Collider2D col) {
		switch (col.tag) {
		case "Damaging":
			if (damageable) {
				PlayerDamage(damage);
				damageable = false;
				StartCoroutine(DamageCoolDown(invincibilityTime));
			}
			break;
		}
    }

    void OnTriggerExit2D(Collider2D col){
		switch (col.tag) {
		case "Destructable":
			col.GetComponentInParent<Destructable>().Disintegrate();
			break;
		}
	}
	  

    #region PlayerMethods
    /// <summary>
    /// Have the player take damage
    /// </summary>
    /// <param name="dmg"></param>
    private void PlayerDamage(int dmg) {
        currHealth -= dmg;
		SoundManager.instance.PlaySingle(steamSound, steamVolume);
        if(currHealth > 0) {
            updateHealth();
        }
        //turn off the player collider so he doesn't keep dying
        else {
            GetComponent<BoxCollider2D>().enabled = false;
			PlayerDeath();
        }
    }

    //void OnDrawGizmos() {
    //    Gizmos.DrawSphere(transform.position, explodeRadius * transform.localScale.x);
    //}

    private void PlayerExplode() {
        // create the circle
        Collider2D[] destructables = Physics2D.OverlapCircleAll(transform.position, explodeRadius * transform.localScale.x, -LayerMask.NameToLayer("Platform"));
        foreach(Collider2D col in destructables) {
            if(col.tag == "Destructable") {
                Destructable d = col.GetComponent<Destructable>();
                // if it is actually a Destructable
                if (d) {
                    StartCoroutine(d.BurnThenDisintegrate());
                }
            }
        }
    }
    
    /// <summary>
    /// Grows the player
    /// </summary>
    /// <param name="food"></param>
	private void PlayerGrow(int food) {
		currHealth += food;
		//make sure our health doesn't overflow
		if (currHealth > maxHealth) {
			currHealth = maxHealth;
		}
		updateHealth();
    }

    /// <summary>
    /// Updates player health
    /// </summary>
    private void updateHealth() {

		//update healthbar
		float normalizedHealth = (float)currHealth / (float)maxHealth;
		healthBar.GetComponent<RectTransform> ().sizeDelta = new Vector2 (normalizedHealth * 256, 32);


		//update robot size
        Vector3 prevScale = transform.localScale;
        float newScale = minScale + (normalizedHealth * (maxScale - minScale));

        GetComponent<Transform>().localScale = new Vector3(newScale, newScale, 0);

        transform.position = new Vector3(transform.position.x, 
            transform.position.y + (newScale - prevScale.y), 0);

		//update particle size
		GetComponentInChildren<ParticleSystem>().startSize = transform.localScale.x * particleSize;
    }

    /// <summary>
    /// Kill the player and show game over
    /// </summary>
    private void PlayerDeath() {
        playerControl = false;
		//this.gameObject.SetActive (false);

		_animator.SetBool("isDead", true);

		GetComponentInChildren<ParticleSystem>().enableEmission = false;

		GameManager.SetPlayerDead (true);

        healthBar.SetActive(false);
        gameOverPanel.SetActive(true);
		gameCamera.GetComponent<CameraFollow2D>().stopCameraFollow();
    }

	private void PlayerNextLevel(){
        continuePanel.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Stop time and tell the player they won
    /// </summary>
    public void PlayerWin() {
        //TODO have a last panel?
        winPanel.SetActive(true);
        Time.timeScale = 0;
    }
#endregion
}