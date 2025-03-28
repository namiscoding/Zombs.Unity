using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    private GameObject projectilePrefab; // The prefab to pool
    private List<Projectile> pool; // The pool of projectiles
    private int poolSize; // Initial size of the pool

    public void Initialize(GameObject projectilePrefab, int poolSize)
    {
        this.projectilePrefab = projectilePrefab;
        this.poolSize = poolSize;
        pool = new List<Projectile>();

        // Pre-instantiate the projectiles
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            projectileObj.SetActive(false);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetPool(this);
                pool.Add(projectile);
            }
            else
            {
                Debug.LogError("Projectile prefab does not have a Projectile component!");
                Destroy(projectileObj);
            }
        }
    }

    public Projectile GetProjectile(Vector3 position, Quaternion rotation)
    {
        // Find an inactive projectile in the pool
        foreach (Projectile projectile in pool)
        {
            if (!projectile.gameObject.activeSelf)
            {
                projectile.gameObject.SetActive(true);
                projectile.transform.position = position;
                projectile.transform.rotation = rotation;
                return projectile;
            }
        }

        // If no inactive projectile is found, expand the pool
        GameObject newProjectileObj = Instantiate(projectilePrefab, position, rotation);
        Projectile newProjectile = newProjectileObj.GetComponent<Projectile>();
        if (newProjectile != null)
        {
            newProjectile.SetPool(this);
            pool.Add(newProjectile);
        }
        else
        {
            Debug.LogError("Newly instantiated projectile does not have a Projectile component!");
            Destroy(newProjectileObj);
        }
        return newProjectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
}