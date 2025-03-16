using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public enum WeaponType { Sword, Mace, Shovel, NoWeapon, Boss1, Boss2 }
    public WeaponType weaponType;
    public int baseDamage;

    void Start()
    {
        SetDamageByWeaponType();
    }

    void SetDamageByWeaponType()
    {
        switch (weaponType)
        {
            case WeaponType.Sword: baseDamage = 30; break;
            case WeaponType.Mace: baseDamage = 40; break;
            case WeaponType.Shovel: baseDamage = 20; break;
            case WeaponType.NoWeapon: baseDamage = 10; break;
            case WeaponType.Boss1: baseDamage = 50; break;
            case WeaponType.Boss2: baseDamage = 45; break;
            default: baseDamage = 10; break;
        }
    }

    public void DealDamage(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerHealth player = target.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(baseDamage);
                Debug.Log($"{weaponType} gây {baseDamage} sát thương lên Player");
            }
        }
    }
}
