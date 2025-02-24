using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float speed;
    public float torque;

    private Transform transform;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
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
