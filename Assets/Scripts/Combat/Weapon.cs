using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // Assign a child empty GameObject where bullets spawn
    public float bulletSpeed = 20f;

    public void Shoot(Vector3 direction)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction, bulletSpeed);
        }

        AudioManager.instance.PlayerShoot();
    }
}
