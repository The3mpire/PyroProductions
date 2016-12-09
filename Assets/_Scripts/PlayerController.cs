using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using UnityEditor;

[RequireComponent(typeof(CharacterController2D), typeof(AnimationController2D))]
public class PlayerController : MonoBehaviour {

    private enum Direction {
        left, right, idle
    }

    public GameObject gameCamera;
    
    public ParticleSystem healthBar;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject continuePanel;
	public GameObject pausePanel;

    public ParticleSystem initialExplode;
    public ParticleSystem continuousExplode;

    public AudioClip jumpSound;
	public AudioClip steamSound;
    public AudioClip explodeSound;
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
    [Tooltip("How much damage exploding does to the player")]
    public int explodeDamage = 15;
    [Tooltip("How long the finish animation takes to play")]
    public float burnLength = .75f;

    private CharacterController2D _controller;
    private Animator _animator;
	private AnimationController2D animControl;
    [SerializeField]
    private int currHealth = 0;
    private bool playerControl = true;
    private bool jump = false;
	private bool menu = false;
    private bool damageable = true;
    private bool canExplode = true;
    private float maxParticleSize = 2f;

    private int level;

    private Direction facing = Direction.right;


    void Awake() {
        initialExplode.Stop();
        continuousExplode.Stop();
    }
        
    // Use this for initialization
	void Start () {
      
        Time.timeScale = 1;

		healthBar.Play();

        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
		animControl = GetComponent<AnimationController2D>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);
        currHealth = startHealth;

        updateHealth();

        gameObject.GetComponent<Collider2D>().enabled = true;

        level = SceneManager.sceneCountInBuildSettings;
    }
	
	// Update is called once per frame
	void Update () {
        
		if (Input.GetButtonDown("Cancel") && !menu) {
			pausePanel.SetActive (true);
			menu = true;
			Cursor.visible = true;
			Time.timeScale = 0;
		} else if (Input.GetButtonDown("Cancel") && menu) {
			pausePanel.SetActive (false);
			Cursor.visible = false;
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
            //ParticleSystem.EmissionModule em = initialExplode.emission;
            //em.enabled = true;
            //ParticleSystem.EmissionModule em1 = continuousExplode.emission;
            //em1.enabled = true;
            initialExplode.Play();
            continuousExplode.Play();
            _animator.SetTrigger("isExploding");
            canExplode = false;
            damageable = false;
            playerControl = false;
            StartCoroutine(ExplodeCooldowns(_animator.GetCurrentAnimatorClipInfo(0).Length));
        }
    }

    private IEnumerator ExplodeCooldowns(float time) {
        yield return new WaitForSeconds(time + .1f);
        initialExplode.Stop();
        continuousExplode.Stop();        

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
					SoundManager.instance.PlaySingle(steamSound, steamVolume);
                    damageable = false;
                    StartCoroutine(DamageCoolDown(invincibilityTime));
                }
                break;
            case "NextLevel":
                StartCoroutine(PlayerWinAnim(col, burnLength, false));
				break;
            case "Win":
                StartCoroutine(PlayerWinAnim(col, burnLength, true));
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

    IEnumerator PlayerWinAnim(Collider2D col, float time, bool hasWon) {
        col.gameObject.GetComponent<Animator>().SetTrigger("hasWon");
        yield return new WaitForSeconds(time);
        if (hasWon) {
            PlayerWin();
        }
        else {
            PlayerNextLevel();
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
        SoundManager.instance.PlaySingle(explodeSound);
        PlayerDamage(explodeDamage);
        initialExplode.startSpeed = explodeRadius;
        continuousExplode.startSpeed = explodeRadius;
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

        healthBar.startLifetime = 2 + normalizedHealth;
        healthBar.startSize = normalizedHealth;

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

        GetComponentInChildren<ParticleSystem>().Stop();

		GameManager.SetPlayerDead (true);

        healthBar.Stop();
        gameOverPanel.SetActive(true);
		Cursor.visible = true;
		gameCamera.GetComponent<CameraFollow2D>().stopCameraFollow();
    }

	private void PlayerNextLevel(){
        //ensure the player can't get hurt
		GameManager.SetPoints (currHealth);
        gameObject.GetComponent<Collider2D>().enabled = false;
		continuePanel.GetComponentInChildren<Text> ().text = "Points: " + (PlayerPrefs.GetInt("points") + currHealth);
        continuePanel.SetActive(true);
		Cursor.visible = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Stop time and tell the player they won
    /// </summary>
    public void PlayerWin() {
        //TODO have a last panel?
        gameObject.GetComponent<Collider2D>().enabled = false;

		winPanel.SetActive(true);
		Cursor.visible = true;
        Time.timeScale = 0;
    }
#endregion
}