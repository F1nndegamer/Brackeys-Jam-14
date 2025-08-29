using UnityEngine;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private CollectibleSOArray collectibleSOArray;

    private CollectibleSO currentObjective;
    private bool ObjectiveCompleted;
    void Awake()
    {
        RandomizeObjective();
    }

    private void Start()
    {
    }
    private void RandomizeObjective()
    {
        currentObjective = collectibleSOArray.GetRandomCollectible();
    }
    public void Reset()
    {
        ObjectiveCompleted = false;
        RandomizeObjective();
    }
    public CollectibleSO GetCurrentObjective()
    {
        return currentObjective;

    }
    public void CheckCurrentObjective(CollectibleSO collectibleSO)
    {
        if (collectibleSO == currentObjective && !ObjectiveCompleted)
        {
            UIManager.Instance.GetCanvas<CanvasGameplay>().FinishObjective();
            WinCondition.Instance.WinGame();
            RandomizeObjective();
        }
    }
}
