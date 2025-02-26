using UnityEngine;

public class Bouncer : Enemy
{
    public float speed;
    public float torque;
    public float deactivationDistance = 20f; // Distance maximale avant désactivation
    public float reactivationDistance = 18f; // Distance minimale avant réactivation
    public bool isActive = true;
    private GameObject player;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de EnemyBase si nécessaire

        rb = GetComponent<Rigidbody2D>();

        transform.Rotate(0, 0, Random.Range(0, 360));
        rb.linearVelocity = transform.up * speed;
        rb.AddTorque(torque);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        // Ensure the speed remains constant
        if (isActive)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
        
        if (player.transform != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance > deactivationDistance)
            {
                isActive = false;
            }
            else if (distance < reactivationDistance)
            {
                isActive = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FindObjectOfType<AudioManager>().playSound("Bounce");
    }
}

