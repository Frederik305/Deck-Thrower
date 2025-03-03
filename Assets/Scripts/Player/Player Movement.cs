using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    private Animator animator;
    public float speed = 10f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private float diagnolLimiter = 0.7f;
    private float hor, ver;
    private Vector3 mousePos;
    private Vector3 playerPos;

    private Rigidbody2D rb;
    private Transform transform;

    public int health = 5;
    private int maxHealth;
    public float invincibilityTime = 1f;
    private float invincibilityCounter;

    private GameManager gameManager;
    private Health healthScript;

    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        invincibilityCounter = 0;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        healthScript = GameObject.FindGameObjectWithTag("Health Display").GetComponent<Health>();
        maxHealth = health;
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        // Dash mouvement when the player press the space bar (JPL)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (speed < 11f)
            {
                speed = 30f;
            }
        }
        // Gradually remove the dash speed each frame until the speed is normal
        if (speed > 10f)
        {
            speed = speed - 0.30f;
        }

        //Track player and mouse coords
        playerPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = Input.mousePosition;
        invincibilityCounter += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            if (hor != 0 && ver != 0)
            {
                hor *= diagnolLimiter;
                ver *= diagnolLimiter;
            }
            Vector2 movement = new Vector2(hor * speed, ver * speed);
            rb.velocity = movement;
        
           // Active l'animation de marche seulement si le joueur bouge
        if (animator != null)
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
                animator.Play("Idle"); 
            }
        }

        float playerAngle = Mathf.Atan2(mousePos.x - playerPos.x, mousePos.y - playerPos.y) * Mathf.Rad2Deg * -1;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, playerAngle));
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        rb.linearVelocity = new Vector2(hor, ver).normalized * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (invincibilityCounter < invincibilityTime)
        {
            return;
        }

        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Bullet" || other.gameObject.tag == "Enemy")
        {
            health--;
            healthScript.lowerHealth(health);
            FindObjectOfType<AudioManager>().playSound("Hurt");

            if (other.gameObject.tag == "Bullet")
            {
                Destroy(other.gameObject);
            }
            resetInvincibilityCounter();
        }

        if (health <= 0)
        {
            gameManager.GameOver();
            gameObject.SetActive(false);
        }
    }

    public void resetInvincibilityCounter()
    {
        invincibilityCounter = 0;
    }

    public void raiseHealth()
    {
        if (health < maxHealth)
        {
            healthScript.raiseHealth(health);
            health++;
        }
    }
}
