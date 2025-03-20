using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Center center;
    public int CenterLevel => center != null ? center.GetLevel() : 0;
    public bool HasCenter => center != null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCenter(Center centerInstance)
    {
        if (center == null)
        {
            center = centerInstance;
            // Trigger enemy spawning here
            Debug.Log("Center built, enemies triggered!");
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Center destroyed.");
        // Implement game over logic (e.g., scene reload)
    }
}