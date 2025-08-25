using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleSOArray", menuName = "Scriptable Objects/CollectibleSOArray")]
public class CollectibleSOArray : ScriptableObject
{
    public CollectibleSO[] collectibleSOArray;
    public int GetIndex(CollectibleSO collectibleSO)
    {
        for (int i = 0; i < collectibleSOArray.Length; i++)
        {
            if (collectibleSOArray[i] == collectibleSO) return i;
        }
        return -1;
    }
}
