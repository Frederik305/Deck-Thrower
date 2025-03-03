using UnityEngine;

public class Shooter : Enemy
{
    public float shootTime = 1f;
    private float timer;
    public GameObject bullet;
    private GameObject player;
    public float bulletForce = 15f;
    private Transform transform;
    public float deactivationDistance = 20f; // Distance maximale avant désactivation
    public float reactivationDistance = 18f; // Distance minimale avant réactivation
    public bool isActive = true;

    protected override void Start()
    {
        base.Start(); // Appelle la méthode Start() de EnemyBase si nécessaire
        player = GameObject.FindGameObjectWithTag("Player");
        transform = gameObject.GetComponent<Transform>();
        timer = 0;
    }

    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= shootTime)
            {
                Shoot();
                timer = 0;
            }
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

    public void Shoot()
    {
        Vector2 difference = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        difference = difference.normalized;

        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().AddForce(difference * bulletForce);
    }
}
