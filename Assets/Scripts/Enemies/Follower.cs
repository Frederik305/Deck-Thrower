using UnityEngine;

public class Follower : Enemy
{
    private GameObject player;

    private Transform transform;
    private Rigidbody2D rb;
    public float deactivationDistance = 20f; // Distance maximale avant désactivation
    public float reactivationDistance = 18f; // Distance minimale avant réactivation
    public bool isActive = true;

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
        if (isActive)
        {
            Vector2 difference = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
            //Add a force in the direction of the player
            rb.AddForce(difference * homingAmount * Time.deltaTime);
        }
        
        if (player.transform != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance > deactivationDistance)
            {
                rb.linearVelocity = Vector2.zero; // Arrêter l'ennemi
                rb.angularVelocity = 0f;   // Arrêter la rotation
            
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
        FindObjectOfType<AudioManager>().playSound("Light Bounce");
    }

}
