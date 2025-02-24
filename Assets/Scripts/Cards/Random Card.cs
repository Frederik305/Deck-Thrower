using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCard : MonoBehaviour
{
    //The ranges of each variable value of random card
    public float speedLow, speedHigh, sizeLow, sizeHigh;
    public int damageLow, damageHigh;

    private Rigidbody2D rb;
    private Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();

        rb.linearVelocity = transform.up * Random.Range(speedLow, speedHigh);

        transform.localScale *= Random.Range(sizeLow, sizeHigh);

        GetComponent<cardMovement>().damage = Random.Range(damageLow, damageHigh);
    }
}
