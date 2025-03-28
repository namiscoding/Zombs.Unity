using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Buildings/Tower Data")]
public class TowerData : BuildingData
{
    public float baseRange; // Base range of the tower (circular)
    public float[] rangeMultipliers; // Size 5 for levels 1-5
    public float baseFireRate; // Shots per second
    public float[] fireRateMultipliers; // Size 5 for levels 1-5
    public int baseDamage; // Base damage per shot
    public float[] damageMultipliers; // Size 5 for levels 1-5
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed; // Speed of the projectile
    // BombTower-specific
    public float explosionRadius; // Explosion radius for BombTower (0 for other towers)
    // MageTower-specific
    public float angleBetweenBullets; // Angle between the three bullets for MageTower (0 for other towers)
}