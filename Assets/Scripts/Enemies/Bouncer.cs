using UnityEngine;

public class Bouncer : Enemy
{
    public float speed;
    public float torque;

    private Transform transform;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de EnemyBase si nécessaire

        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        transform.Rotate(0, 0, Random.Range(0, 360));
        rb.linearVelocity = transform.up * speed;
        rb.AddTorque(torque);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FindObjectOfType<AudioManager>().playSound("Bounce");
    }

}
