using UnityEngine;
using UnityEngine.UI;
public class HealthPlayer : MonoBehaviour
{
    [SerializeField] private Slider health;
    [SerializeField] private Slider health2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdatePlayerHealth(float currentValue, float maxValue)
    {
        health.value = currentValue / maxValue;
        health2.value = currentValue / maxValue;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
