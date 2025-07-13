using UnityEngine;

public class CubeController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Vector3 shootDirection = Vector3.forward;
    public float projectileVelocity = 10f;
    public int shootCooldown = 5;
    private float lastShootTime = -Mathf.Infinity;

    void Update()
    {
        PlayerRollingMovement[] players = FindObjectsByType<PlayerRollingMovement>(FindObjectsSortMode.None);
        foreach (PlayerRollingMovement player in players)
        {
            if (player.died)
            {
                return;
            }
        }
        if (Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection.normalized * projectileVelocity;
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on projectile prefab!");
        }
    }
}