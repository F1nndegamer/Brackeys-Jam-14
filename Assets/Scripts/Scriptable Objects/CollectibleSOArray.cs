using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleSOArray", menuName = "Scriptable Objects/CollectibleSOArray")]
public class CollectibleSOArray : ScriptableObject
{
    [Header("BISCUIT must have last index")]
    public CollectibleSO[] collectibleSOArray;
    public int GetIndex(CollectibleSO collectibleSO)
    {
        for (int i = 0; i < collectibleSOArray.Length; i++)
        {
            if (collectibleSOArray[i] == collectibleSO) return i;
        }
        return -1;
    }
    public CollectibleSO GetRandomCollectible()
    {
        return collectibleSOArray[Random.Range(0, collectibleSOArray.Length - 1)]; // Minus 1 to exclude BISCUIT
    }
}
