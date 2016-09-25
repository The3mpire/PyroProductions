using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public GameObject gameCamera;
    public GameObject healthBar;
    public GameObject gameOverPanel;
    public GameObject winText;

    public float walkSpeed = 3;
    public float jumpHeight = 2;
    public float gravity = -35;

    public int health = 100;

    private CharacterController2D _controller;
    private AnimationController2D _animator;
    private int currHealth = 0;
    private bool playerControl = true;

    private readonly int _DAMAGE = 25;
        
    // Use this for initialization
	void Start () {
        Time.timeScale = 1;

        _controller = gameObject.GetComponent<CharacterController2D>();
        _animator = gameObject.GetComponent<AnimationController2D>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(gameObject);
        currHealth = health;
    }
	
	// Update is called once per frame
	void Update () {
        
        // is player dead
        if (playerControl) {
            Vector3 velocity = playerInput();
            _controller.move(velocity * Time.deltaTime);
        }

    }

    private Vector3 playerInput() {

        //Platform
        Vector3 velocity = _controller.velocity;

        velocity.x = 0;

        if (_controller.ground != null && _controller.ground.tag == "MovingPlatform") {
            this.transform.parent = _controller.ground.transform;
        }
        else {
            if (this.transform.parent != null) {
                this.transform.parent = null;
            }
        }

        //Animation
        if (Input.GetAxis("Horizontal") < 0) {
            velocity.x = -1 * walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Left");
        }
        else if (Input.GetAxis("Horizontal") > 0) {
            velocity.x = walkSpeed;
            if (_controller.isGrounded) {
                _animator.setAnimation("Run");
            }
            _animator.setFacing("Right");
        }
        else {
            //play idle animation
            _animator.setAnimation("Idle");
        }

        if (Input.GetAxis("Jump") > 0 && _controller.isGrounded) {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);

            //jump anim
            _animator.setAnimation("Jump");
        }

        velocity.y += gravity * Time.deltaTime;

        return velocity;
    }


   void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("KillZ")) {
            PlayerFallDeath();
        }
        else if (col.CompareTag("Damaging")) {
            PlayerDamage(_DAMAGE);
        }
        else if (col.CompareTag("Win")) {
            PlayerWin();
        }
    }

    private void PlayerDamage(int dmg) {
        currHealth -= dmg;

        float normalizedHealth = (float)currHealth /(float) health;

        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(normalizedHealth * 256, 32);

        if(currHealth <= 0) {
            PlayerDeath();
        }
    }

    private void PlayerDeath() {
        playerControl = false;
        _animator.setAnimation("Death");
        gameOverPanel.SetActive(true);
    }

    private void PlayerFallDeath() {
        currHealth = 0;
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 32);
        gameOverPanel.SetActive(true);

        gameCamera.GetComponent<CameraFollow2D>().stopCameraFollow();
    }

    private void PlayerWin() {
        winText.SetActive(true);
        Time.timeScale = 0;
    }
}
