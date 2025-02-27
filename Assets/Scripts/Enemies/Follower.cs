using UnityEngine;

public class Follower : Enemy
{
    private GameObject player;

    private Transform transform;
    private Rigidbody2D rb;

    public float homingAmount = 1f;

    protected override void Start() {
        base.Start(); // Appel au `Start()` de la classe de base Enemy
        player = GameObject.FindGameObjectWithTag("Player");

        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Finds the difference between card's coords and player's coords
        Vector2 difference = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);

        //Add a force in the direction of the player
        rb.AddForce(difference * homingAmount * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        FindObjectOfType<AudioManager>().playSound("Light Bounce");
    }

}
