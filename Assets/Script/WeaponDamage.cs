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
            case WeaponType.Sword: baseDamage = 7; break;
            case WeaponType.Mace: baseDamage = 8; break;
            case WeaponType.Shovel: baseDamage = 9; break;
            case WeaponType.NoWeapon: baseDamage = 10; break;
            case WeaponType.Boss1: baseDamage = 11; break;
            case WeaponType.Boss2: baseDamage = 12; break;
            default: baseDamage = 10; break;
        }
    }

    
}