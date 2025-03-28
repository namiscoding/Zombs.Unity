using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private Center center;
    public int CenterLevel => center != null ? center.GetLevel() : 0;
    public bool HasCenter => center != null;
    
    // Add reference to the enemy spawner

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCenter(Center centerInstance)
    {
        if (center == null)
        {
            center = centerInstance;
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Center destroyed.");
        // Stop enemy spawning when game is over
       
    }

}