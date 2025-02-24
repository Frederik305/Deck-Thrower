using UnityEngine;

public class Bouncer : Enemy
{
    public float speed;
    public float torque;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de EnemyBase si nécessaire

        rb = GetComponent<Rigidbody2D>();

        transform.Rotate(0, 0, Random.Range(0, 360));
        rb.linearVelocity = transform.up * speed;
        rb.AddTorque(torque);
    }

    private void FixedUpdate()
    {
        // Ensure the speed remains constant
        rb.linearVelocity = rb.linearVelocity.normalized * speed / 1.5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FindObjectOfType<AudioManager>().playSound("Bounce");
    }
}

