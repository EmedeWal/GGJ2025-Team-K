using UnityEngine;

public class ShootingEnemy : BaseEnemy
{
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireRate = 1f;
    private float fireTimer = 0f;

    
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        staysOnPlatform = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (ThisStatus)
        {
            case Status.roaming:
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate)
                {
                    Shoot();
                    fireTimer = 0;
                }
                break;
            case Status.bubble:
                break;
            case Status.stunned:
                break;
            default:
                break;
        }
    }

    void Shoot()
    {
        var offset = transform.position.x + 0.75f * Direction;
        Vector3 pos = new Vector3(offset, transform.position.y, 0);
        GameObject bullet = Instantiate(bulletPrefab, pos, bulletPrefab.transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(bulletSpeed * Direction, 0));
    }
}
