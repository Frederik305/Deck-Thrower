using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    //The speed that the player moves at
    public float speed = 10f;

    //Prevents diagnol movement from going faster than regular movement
    public float diagnolLimiter = .7f;

    //Handles movement input
    private float hor, ver;
    //Holds the mouse coords (for player rotation)
    private Vector3 mousePos;

    //Holds the screen coords of the player (Transforms holds world coords, but since the mouse is on the screen, we must base our position on the screen)
    private Vector3 playerPos;

    private Rigidbody2D rb;
    private Transform transform;

    public int health = 5;
    private int maxHealth;
    //Invincibilty frames after getting hit
    public float invincibilityTime = 1f;
    private float invincibilityCounter;

    private GameManager gameManager;

    private Health healthScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();

        invincibilityCounter = 0;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        healthScript = GameObject.FindGameObjectWithTag("Health Display").GetComponent<Health>();

        //Keeps track of the max health that the player has
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        //Get movement input
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        // Dash mouvement when the player press the space bar (JPL)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (speed < 11f)
            {
                speed = 50f;
            }
        }
        // Gradually remove the dash speed each frame until the speed is normal
        if (speed > 10f)
        {
            speed = speed - 0.25f;
        }

        //Track player and mouse coords
        playerPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = Input.mousePosition;

        invincibilityCounter += Time.deltaTime;
    }

    void FixedUpdate()
    {
        //If both inputs active, we are moving diagnolly, so limit the velocity
        if (hor != 0 && ver != 0) {
            hor *= diagnolLimiter;
            ver *= diagnolLimiter;
        }

        //Sets the player's velocity based on input (setting velocity means that we won't move through walls)
        rb.linearVelocity = new Vector2(hor * speed, ver * speed);

        //Calculates the angle from the player to the mouse (Atan2 returns a radian, so we must convert it to degrees)
        float playerAngle = Mathf.Atan2(mousePos.x - playerPos.x, mousePos.y - playerPos.y) * Mathf.Rad2Deg * -1;

        //Rotate the player to face the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, playerAngle));
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        //Only take damage when you are out of invincibility
        if (invincibilityCounter < invincibilityTime)
        {
            return;
        }

        //If player hits an enemy or bullet, take damage
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Bullet")
        {
            health--;
            //Update the health bar
            healthScript.lowerHealth(health);

            //Play hurt sound
            FindObjectOfType<AudioManager>().playSound("Hurt");

            //If it is a bullet, destroy it on impact
            if (other.gameObject.tag == "Bullet")
            {
                Destroy(other.gameObject);
            }

            resetInvincibilityCounter();
        }

        //If out of health, GAME OVER
        if (health <= 0)
        {
            gameManager.gameOver();
            gameObject.SetActive(false);
        }
    }

    public void resetInvincibilityCounter ()
    {
        invincibilityCounter = 0;
    }

    public void raiseHealth ()
    {
        if (health < maxHealth)
        {
            healthScript.raiseHealth(health);
            health++;
        }
    }
}
