using UnityEngine;
public class Shooter : Enemy
{
    public float shootTime = 1f;
    private float timer;
    public GameObject bullet;
    public float bulletForce = 15f;

    protected override void Start()
    {
        base.Start();
        isActive = true;
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

        HandleActivation();
    }

    public void Shoot()
    {
        Vector2 difference = (player.position - transform.position).normalized;
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().AddForce(difference * bulletForce);
    }
}
