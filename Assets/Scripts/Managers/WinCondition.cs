using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance;
    public List<GameObject> Walls;
    public float DifficultyMultiplier = 1;
    public bool hasWon;
    void Awake()
    {
        Instance = this;
        GridRoomGenerator.Instance.Difficulty = PlayerPrefs.GetInt("Difficulty", 0);
    }
    void Start()
    {

    }
    public void WinGame()
    {
        Debug.Log("Win condition met");
        hasWon = true;
        foreach (var wall in Walls)
        {
            Destroy(wall);
        }
    }
    void ExitRoom()
    {
        EndGame eg = FindFirstObjectByType<EndGame>();
        MusicManager.Instance.PlayMusic(GameState.Shop);
        eg.Fade();
        Player.Instance.CommitTempCollectibles();
        float Difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        Difficulty += DifficultyMultiplier;
        PlayerPrefs.SetInt("Difficulty", (int)Difficulty);
        PlayerPrefs.Save();
        print("excecuting fade");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasWon) ExitRoom();
            else Debug.Log("player has not completed objective");
        }
    }
}
