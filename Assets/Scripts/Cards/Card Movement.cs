using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardMovement : MonoBehaviour
{
    public float cardSpeed = 5f;
    public float cardTorque = 30f;

    private Rigidbody2D rb;
    private Transform transform;

    //This is the index of this card in the inventory
    private int inventoryIndex;

    //How long it will take before player can pick the card up again
    public float collisionTimer = .5f;
    
    //How long the card has been thrown out for
    private float timeAlive;

    //How much damage a card deals to an enemy
    public int damage = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();

        //Have it fly forward in the direction the player is facing
        rb.linearVelocity = transform.up * cardSpeed;
        //Have it also spin
        rb.AddTorque(cardTorque);

        timeAlive = 0;

        StartCoroutine(DestroyAfterTime(4f));
    }

    private void Update() {
        timeAlive += Time.deltaTime;
        if (timeAlive > collisionTimer) {
            //After enough time, enable collisions with the player for picking up
            gameObject.layer = 8; //Layer 8 is "Pickup"
        }
    }

    private void OnCollisionEnter2D (Collision2D other) {
        //If the player touches the card, they pick it up and put it back in their inventory
        if (other.gameObject.tag == "Player")
        {
           
            GameObject.Destroy(gameObject);
        }
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            Debug.Log("OUCHHHH");
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Barrel")
        {
            other.gameObject.GetComponent<EnemyBarrel>().Explode();
            Destroy(other.gameObject);
            Debug.Log("Explosion");
        }

        FindObjectOfType<AudioManager>().playSound("Card Bounce");
    }

    public void SetIndex (int index)
    {
        inventoryIndex = index;
    }
    IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
