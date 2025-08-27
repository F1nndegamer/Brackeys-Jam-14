using UnityEngine;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private CollectibleSOArray collectibleSOArray;

    private CollectibleSO currentObjective;
    private void Start()
    {
        RandomizeObjective();
    }
    private void RandomizeObjective()
    {
        currentObjective = collectibleSOArray.GetRandomCollectible();
    }
    public CollectibleSO GetCurrentObjective()
    {
        return currentObjective;
    }
    public void CheckCurrentObjective(CollectibleSO collectibleSO)
    {
        if (collectibleSO == currentObjective)
        {
            UIManager.Instance.GetCanvas<CanvasGameplay>().FinishObjective();
            RandomizeObjective();
        }
    }
}
