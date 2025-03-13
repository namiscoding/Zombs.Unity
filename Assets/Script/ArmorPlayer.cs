using UnityEngine;
using UnityEngine.UI;

public class ArmorPlayer : MonoBehaviour
{
     [SerializeField] private Slider armor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdatePlayerArmor(float currentValue, float maxValue)
    {
        armor.value = currentValue / maxValue;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
