using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;


    private Camera camera;
    [SerializeField]
          private Transform target;
    private Vector3 offset = new Vector3(0, -9, 0);

  
    void Start()
    {
        camera = Camera.main;
        if (camera == null)
        {
            camera = FindAnyObjectByType<Camera>();
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}

