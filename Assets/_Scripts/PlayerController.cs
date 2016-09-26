using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController2D), typeof(AnimationController2D))]
public class PlayerController : MonoBehaviour {

    private enum Direction {
        left, right, idle
    }

    public GameObject gameCamera;
    public GameObject healthBar;
    public GameObject gameOverPanel;
    public GameObject winText;

    public float walkSpeed = 3;
    public float jumpHeight = 2;
    public float gravity = -35;
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
    public float minScale = .4f;

    private CharacterController2D _controller;
    private AnimationController2D _animator;
    [SerializeField]
    private int currHealth = 0;
    private bool playerControl = true;
    private bool jump = false;

    private Direction facing = Direction.right;

        
    // Use this for initialization
	void Start () {
        Time.timeScale = 1;

        healthBar.SetActive(true);

        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<AnimationController2D>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);
        currHealth = startHealth;

        updateHealth();
    }
	
	// Update is called once per frame
	void Update () {

        // is player dead
        if (playerControl) {
            playerInput();

        }
        Vector3 velocity = _controller.velocity;

        velocity.x = 0;

        /*
        //platform
        if (_controller.ground != null && _controller.ground.tag == "MovingPlatform") {
            this.transform.parent = _controller.ground.transform;
        }
        else {
            if (this.transform.parent != null) {
                this.transform.parent = null;
            }
        }
        */

        if (facing == Direction.left) {
            velocity.x = -1 * walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Left");
        }
        else if (facing == Direction.right) {
            velocity.x = walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Right");
        }
        else {
            _animator.setAnimation("Idle");
        }

        if (jump && facing == Direction.right) {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);

            //jump anim
            _animator.setAnimation("Jump");

            jump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        if (playerControl) {
            _controller.move(velocity * Time.deltaTime);
        }

    }

    //TODO ask matt why this doesn't work :(
    void FixedUpdate() {
        /*
         Vector3 velocity = _controller.velocity;

        velocity.x = 0;

        if (facing == Direction.left) {
            velocity.x = -1 * walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Left");
        }
        else if (facing == Direction.right) {
            velocity.x = walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Right");
        }
        else {
            _animator.setAnimation("Idle");
        }

        if (jump && facing == Direction.right) {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);

            //jump anim
            _animator.setAnimation("Jump");

            jump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        if (playerControl) {
            _controller.move(velocity * Time.deltaTime);
        }
        */
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

        if (Input.GetAxis("Jump") > 0 && _controller.isGrounded) {
            jump = true;
        }
    }


   void OnTriggerEnter2D(Collider2D col) {

        switch (col.tag) {
            case "KillZ":
                PlayerFallDeath();
                break;
            case "Damaging":
                PlayerDamage(damage);
                break;
            case "Win":
                // change to win condition
                if (currHealth >= maxHealth) {
                    PlayerWin();
                }
                break;
            case "Growing":
                col.gameObject.SetActive(false);
                PlayerGrow(food);
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

        updateHealth();

        if(currHealth <= 0) {
            PlayerDeath();
        }
    }
    
    /// <summary>
    /// Grows the player
    /// </summary>
    /// <param name="food"></param>
    private void PlayerGrow(int food) {
        currHealth += food;

        if(currHealth <= maxHealth) {
            updateHealth();
        }

    }

    /// <summary>
    /// Updates player health
    /// </summary>
    private void updateHealth() {
        float normalizedHealth = (float)currHealth / (float)maxHealth;

        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(normalizedHealth * 256, 32);

        Vector3 prevScale = transform.localScale;

        float newScale = minScale + (normalizedHealth * (maxScale - minScale));

        GetComponent<Transform>().localScale = new Vector3(newScale, newScale, 0);

        transform.position = new Vector3(transform.position.x, 
            transform.position.y + (newScale - prevScale.y), 0);
    }

    /// <summary>
    /// Kill the player and show game over
    /// </summary>
    private void PlayerDeath() {
        playerControl = false;
        _animator.setAnimation("Death");
        healthBar.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Kill the player through falling off the map
    /// </summary>
    private void PlayerFallDeath() {
        currHealth = 0;
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32);
        gameOverPanel.SetActive(true);

        gameCamera.GetComponent<CameraFollow2D>().stopCameraFollow();
    }

    /// <summary>
    /// Stop time and tell the player they won
    /// </summary>
    private void PlayerWin() {
        winText.SetActive(true);
        Time.timeScale = 0;
    }
#endregion
}
