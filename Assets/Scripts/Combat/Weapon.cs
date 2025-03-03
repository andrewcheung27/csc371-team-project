using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // Assign a child empty GameObject where bullets spawn
    public float bulletSpeed = 20f;

    public void Shoot(Vector3 direction)
    {
        // can't shoot if game is over
        if (!GameManager.instance.GameIsRunning()) {
            return;
        }

        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bulletInstance.transform.Rotate(new Vector3(90f, 0f, 0f));  // make bullet horizontal
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction, bulletSpeed);
        }

        AudioManager.instance.PlayerShoot();
    }
}
