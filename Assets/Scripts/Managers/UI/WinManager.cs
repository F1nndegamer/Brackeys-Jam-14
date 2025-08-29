using UnityEngine;

public class WinManager : MonoBehaviour
{

    public void RestartLevel()
    {
        UIManager.Instance.CloseImmediate<CanvasShop>();
        GridRoomGenerator.Instance.GenerateRooms();
        ObstaclesManagers.Instance.ReCrateGrid();
        Player.Instance.Respawn();
        ObjectiveManager.Instance.Reset();
        WinCondition.Instance.hasWon = false;
        CanvasGameplay cg = UIManager.Instance.GetCanvas<CanvasGameplay>();
        cg.Reset();
        EndGame eg = FindFirstObjectByType<EndGame>();
        eg.Unfade();
    }
}
