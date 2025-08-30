using UnityEngine;

public class WinManager : MonoBehaviour
{

    public void RestartLevel()
    {
        UIManager.Instance.CloseImmediate<CanvasShop>();
        ObjectiveManager.Instance.Reset();
        ObstaclesManagers.Instance.Cleaner();
        GridRoomGenerator.Instance.GenerateRooms();
        ObstaclesManagers.Instance.ReCrateGrid();
        Player.Instance.Respawn();
        UIManager.Instance.GetCanvas<CanvasGameplay>().ResetStrikes();
        WinCondition.Instance.hasWon = false;
        CanvasGameplay cg = UIManager.Instance.GetCanvas<CanvasGameplay>();
        cg.Reset();
        EndGame eg = FindFirstObjectByType<EndGame>();
        eg.Unfade();
    }
}
