using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance;
    public List<GameObject> Walls;
    public bool hasWon;
    void Awake()
    {
        Instance = this;
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
        eg.Fade();
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
